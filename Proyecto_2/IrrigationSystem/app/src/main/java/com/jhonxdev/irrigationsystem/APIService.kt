package com.jhonxdev.irrigationsystem

import retrofit2.Call
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.Field
import retrofit2.http.GET
import retrofit2.http.Headers
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Url

interface APIService {
    @GET("verEstadoSimple")
    suspend fun getStates():Response<HmdResponse>

    @GET("apagarBomba")
    suspend fun apagar():Response<Unit>

    @PUT("encenderBomba")
    fun sendTime(@Body tiempo: TimeResponse):Call<Unit>
}