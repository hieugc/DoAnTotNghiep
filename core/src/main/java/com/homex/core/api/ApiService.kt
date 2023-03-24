package com.homex.core.api

import com.google.gson.JsonObject
import com.homex.core.model.*
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.response.MessageResponse
import com.homex.core.model.response.MyHomeResponse
import com.homex.core.model.response.RequestResponse
import com.homex.core.model.response.UserResponse
import com.homex.core.param.auth.*
import com.homex.core.param.chat.ConnectToRoomParam
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.chat.GetMessagesParam
import com.homex.core.param.chat.SendMessageParam
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.param.request.EditRequestParam
import okhttp3.RequestBody
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
    suspend fun createNewHome(
        @Body body: RequestBody
    ) : Response<ObjectResponse<JsonObject>>

    @POST("api/House/Update")
    suspend fun editHome(
        @Body body: RequestBody
    ) : Response<ObjectResponse<JsonObject>>

    @POST("api/House/Delete")
    suspend fun deleteHome(
        @Query("id") id: Int
    ) : Response<ObjectResponse<JsonObject>>

    @GET("api/House/GetMyHome?limit=20")
    suspend fun getMyHome(
        @Query("page") page: Int
    ) : Response<ObjectResponse<MyHomeResponse>>

    @GET("api/House/Details")
    suspend fun getHomeDetails(
        @Query("id") id: Int
    ): Response<ObjectResponse<Home>>

    //--------------------MESSAGE-----------------------------

    @POST("api/Message/Send")
    suspend fun sendMessage(
        @Body param: SendMessageParam
    ): Response<ObjectResponse<MessageRoom>>

    @POST("api/Message/ContactToUser")
    suspend fun contactUser(
        @Body param: ContactUserParam
    ): Response<ObjectResponse<MessageRoom>>

    @POST("api/ConnectAllRoom")
    suspend fun connectChat(
        @Body body: RequestBody
    ): Response<ObjectResponse<JsonObject>>

    @POST("api/Message/ConnectToRoom")
    suspend fun connectToRoom(
        @Body param: ConnectToRoomParam
    ): Response<ObjectResponse<MessageRoom>>

    @POST("api/Message/Seen")
    suspend fun seenAll(
        @Body body: RequestBody
    ): Response<ObjectResponse<JsonObject>>

    @GET("api/ChatRoom?limit=20")
    suspend fun getChatRoom(@Query("page") page: Int): Response<ObjectResponse<MessageResponse>>

    @POST("api/MessagesInChatRoom")
    suspend fun getMessagesInChatRoom(@Body param: GetMessagesParam): Response<ObjectResponse<MessageRoom>>

    //--------------------REQUEST-----------------------------

    @POST("api/Request/Create")
    suspend fun createNewRequest(@Body param: CreateRequestParam): Response<ObjectResponse<JsonObject>>

    @POST("api/Request/Delete")
    suspend fun deleteRequest(@Body body: RequestBody): Response<ObjectResponse<JsonObject>>

    @GET("api/Request/Detail")
    suspend fun getRequestById(@Query("Id") id: Int): Response<ObjectResponse<RequestResponse>>

    @GET("api/Request/GetByHouse")
    suspend fun getRequestByHouse(@Query("idHouse") id: Int): Response<ListResponse<RequestResponse>>

    @GET("api/Request/GetRequestSent")
    suspend fun getRequestSent(): Response<ListResponse<RequestResponse>>

    @POST("api/Request/Update")
    suspend fun editRequest(@Body param: EditRequestParam): Response<ObjectResponse<JsonObject>>
}