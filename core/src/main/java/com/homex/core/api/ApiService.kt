package com.homex.core.api

import com.google.gson.JsonObject
import com.homex.core.model.CheckEmailExisted
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.model.Token
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.response.UserResponse
import com.homex.core.param.auth.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {
    //--------------------POPULAR-----------------------------
    @GET("api/GetPopularHouse")
    suspend fun getPopularHome(): Response<ListResponse<Home>>

    @GET("api/GetPopularCity")
    suspend fun getPopularCity(): Response<ListResponse<Location>>

    //--------------------AUTH-----------------------------
    @POST("api/SignUp/CheckMail")
    suspend fun checkMailExisted(@Body param: EmailParam): Response<ObjectResponse<CheckEmailExisted>>

    @POST("api/SignIn")
    suspend fun login(@Body param: LoginParam): Response<ObjectResponse<UserResponse>>

    @POST("api/SignUp/Password")
    suspend fun signup(@Body param: LoginParam): Response<ObjectResponse<Token>>

    @POST("api/SignUp/OTP")
    suspend fun checkOTPSignUp(@Body param: OTPParam): Response<ObjectResponse<Token>>

    @POST("api/SignUp/UpdateInfo")
    suspend fun updateInformation(@Body param: UpdateInfoParam): Response<ObjectResponse<UserResponse>>

    @POST("api/SignUp/ResendOTP")
    suspend fun resendOTP(): Response<ObjectResponse<JsonObject>>

    @POST("api/Forgot/CheckMail")
    suspend fun forgotPassword(@Body param: EmailParam) : Response<ObjectResponse<Token>>

    @POST("api/Forgot/OTP")
    suspend fun checkOTPForgotPassword(@Body param: OTPParam) : Response<ObjectResponse<Token>>

    @POST("api/Forgot/Password")
    suspend fun updateNewPassword(@Body param: PasswordParam) : Response<ObjectResponse<JsonObject>>

    @POST("api/Forgot/ResendOTP")
    suspend fun resendOTPForgotPassword() : Response<ObjectResponse<JsonObject>>

    //--------------------HOUSE-----------------------------

    @POST("api/House/Create")
    suspend fun createNewHome() : Response<ObjectResponse<JsonObject>>

    @PUT("api/House/Update")
    suspend fun editHome() : Response<ObjectResponse<JsonObject>>

    @DELETE("api/House/Delete")
    suspend fun deleteHome() : Response<ObjectResponse<JsonObject>>

    @GET("api/House/GetMyHome")
    suspend fun getMyHome() : Response<ListResponse<Home>>
}