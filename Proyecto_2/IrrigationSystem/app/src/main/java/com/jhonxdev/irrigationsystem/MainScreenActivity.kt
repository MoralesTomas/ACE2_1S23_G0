package com.jhonxdev.irrigationsystem

import android.app.Dialog
import android.content.res.ColorStateList
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.widget.AppCompatButton
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.DrawableCompat
import androidx.core.view.ViewCompat
import com.google.android.material.slider.RangeSlider
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import okhttp3.Dispatcher
import okhttp3.OkHttpClient
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.io.IOException
import java.text.DecimalFormat
import java.util.Timer
import java.util.TimerTask

class MainScreenActivity : AppCompatActivity() {

    private lateinit var btn_on: AppCompatButton
    private lateinit var btnHost: AppCompatButton
    private lateinit var rangeSliderTime:RangeSlider
    private lateinit var tvTime:TextView
    private lateinit var tvPercentage:TextView
    private lateinit var tvPercentageHum:TextView
    private lateinit var timer:Timer
    private var colorBtnOn:Boolean = false
    private var alertShowed:Boolean = false
    private var tiempoSlider:Int = 5
    private var newIP: String = "192.168.1.10"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main_screen)

        initComponents()
        initListeners()
        initUI()

    }

    private fun initComponents(){
        btn_on = findViewById(R.id.btn_on)
        btnHost = findViewById(R.id.btn_host)
        rangeSliderTime = findViewById(R.id.rangeSliderTime)
        tvTime = findViewById(R.id.tvTime)
        tvPercentage = findViewById(R.id.tvPercentage)
        tvPercentageHum = findViewById(R.id.tvPercentageHum)
    }

    private fun initListeners(){
        btn_on.setOnClickListener {
            colorBtnOn = !colorBtnOn
            changeColor()
            if (colorBtnOn){
                //Mandar dato de tiempo y estado
                sendDataTime(tiempoSlider)
            }else{
                //Hacer peticion GET
                apagarRiego()
                alertShowed = false
            }

        }

        rangeSliderTime.addOnChangeListener { _, value, _ ->
            val df = DecimalFormat("#.##")
            val result = df.format(value)
            tvTime.text = "$result s"
            tiempoSlider = result.toInt()
        }

        btnHost.setOnClickListener {
            showNewHostDialog()
        }
    }

    private fun initUI(){
        timer = Timer()
        timer.scheduleAtFixedRate(object : TimerTask(){
            override fun run() {
                getLivePct()
            }
        }, 0, 2000)
    }

    private fun getAlert():AlertDialog{
        return AlertDialog.Builder(this).apply {
            setTitle("Alerta !")
            setMessage("Porcentaje de humedad al 80%. Por favor corte el flujo de agua")
            setPositiveButton("Aceptar", null)
        }.show()
    }

    private fun showAlert(pct: Int){
        if (pct >= 80 && !alertShowed && colorBtnOn){
            getAlert()
            alertShowed = true
        }
    }

    private fun showNewHostDialog(){
        val dialog = Dialog(this)
        dialog.setContentView(R.layout.dialog_host)

        val btnAddHost: Button = dialog.findViewById(R.id.btnAddHost)
        val editText: EditText = dialog.findViewById(R.id.etHost)

        btnAddHost.setOnClickListener {
            newIP = editText.text.toString()
            dialog.hide()
        }

        dialog.show()
    }

    private fun changeColor(){

        if (colorBtnOn){
            DrawableCompat.setTint(DrawableCompat.wrap(btn_on.compoundDrawables[0]), ContextCompat.getColor(this, R.color.btn_on))
            btn_on.setBackgroundResource(R.drawable.btn_redondo)
        }else{
            DrawableCompat.setTint(DrawableCompat.wrap(btn_on.compoundDrawables[0]), ContextCompat.getColor(this, R.color.bg_app))
            btn_on.setBackgroundResource(R.drawable.btn_redondo_off)
        }
    }

    private fun getRetrofit():Retrofit{
        return Retrofit.Builder()
            .baseUrl("http://$newIP:5000/")
            .addConverterFactory(GsonConverterFactory.create())
            .client(getClient())
            .build()
    }

    private fun getClient():OkHttpClient{
        return OkHttpClient.Builder().addInterceptor(HeaderInterceptor()).build()
    }

    private fun getLivePct(){
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val call = getRetrofit().create(APIService::class.java).getStates()
                val res = call.body()
                runOnUiThread {
                    if (call.isSuccessful){
                        val pct = res?.percentWater ?: 0
                        val pctHum = res?.internalHum ?: 0
                        val estadoBtn = res?.estadoRiego ?: false
                        tvPercentage.text = "$pct%"
                        tvPercentageHum.text = "$pctHum%"
                        //!El estado no lo uso, preguntar
                        if (colorBtnOn != estadoBtn){
                            colorBtnOn = estadoBtn
                            changeColor()
                        }
                        showAlert(pctHum.toString().toInt())
                    }else{
                        showError()
                    }
                }
            }catch (e: IOException){
                runOnUiThread {
                    showToastMessage("ERROR: " + e.message.toString())
                }
            }

        }
    }

    private fun sendDataTime(tiempo: Int){
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val call = getRetrofit().create(APIService::class.java).sendTime(TimeResponse(tiempo, true))
                val res = call.execute()
                runOnUiThread {
                    if (res.isSuccessful){
                        showToastMessage("Riego encendido")
                    }else{
                        showError()
                    }
                }
            }catch (e: IOException){
                runOnUiThread {
                    showToastMessage("ERROR: " + e.message.toString())
                }
            }
        }
    }

    private fun apagarRiego(){
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val call = getRetrofit().create(APIService::class.java).apagar()
//            val res = call.body()
                runOnUiThread {
                    if (call.isSuccessful){
                        showToastMessage("Riego apagado")
                    }else{
                        showError()
                    }
                }
            }catch (e: IOException){
                runOnUiThread {
                    showToastMessage("ERROR: " + e.message.toString())
                }
            }
        }
    }

    private fun showError(){
        Toast.makeText(this, "Ha ocurrido un error", Toast.LENGTH_SHORT).show()
    }

    private fun showToastMessage(texto: String){
        Toast.makeText(this, texto, Toast.LENGTH_SHORT).show()
    }

}