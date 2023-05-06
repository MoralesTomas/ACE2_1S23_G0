package com.jhonxdev.irrigationsystem

import com.google.gson.annotations.SerializedName

data class HmdResponse (
    @SerializedName("porcentajeAguaDisponible") var percentWater:Int,
    @SerializedName("estadoRiego") var estadoRiego:Boolean,
    @SerializedName("valorHumedadInterna") var internalHum:Int
)

data class TimeResponse(val tiempoRiego: Int, val estadoRiego: Boolean)