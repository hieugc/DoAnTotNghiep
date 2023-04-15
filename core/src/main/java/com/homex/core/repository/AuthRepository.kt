package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.model.CheckEmailExisted
import com.homex.core.model.Token
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.UserResponse
import com.homex.core.param.auth.*
import okhttp3.RequestBody

interface AuthRepository {
    suspend fun login(param: LoginParam): LiveData<ResultResponse<UserResponse>>

    suspend fun signup(param: LoginParam): LiveData<ResultResponse<Token>>

    suspend fun checkMailExisted(param: EmailParam): LiveData<ResultResponse<CheckEmailExisted>>

    suspend fun checkOTPSignUp(param: OTPParam): LiveData<ResultResponse<Token>>

    suspend fun updateInformation(param: UpdateInfoParam): LiveData<ResultResponse<UserResponse>>

    suspend fun forgotPassword(param: EmailParam): LiveData<ResultResponse<Token>>

    suspend fun checkOTPForgotPassword(param: OTPParam): LiveData<ResultResponse<Token>>

    suspend fun updatePassword(param: PasswordParam): LiveData<ResultResponse<JsonObject>>

    suspend fun updateProfile(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>>
}