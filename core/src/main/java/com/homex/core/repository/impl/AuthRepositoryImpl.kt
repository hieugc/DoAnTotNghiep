package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.CheckEmailExisted
import com.homex.core.model.Token
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.UserResponse
import com.homex.core.param.auth.*
import com.homex.core.repository.AuthRepository
import okhttp3.RequestBody
import retrofit2.Response

class AuthRepositoryImpl(private val api: ApiService): AuthRepository {
    override suspend fun login(param: LoginParam): LiveData<ResultResponse<UserResponse>> {
        return object : NetworkBoundResource<ObjectResponse<UserResponse>, UserResponse>(){
            override fun processResponse(response: ObjectResponse<UserResponse>): UserResponse? = response.data
            override suspend fun createCall(): Response<ObjectResponse<UserResponse>> = api.login(param)
        }.build().asLiveData()
    }

    override suspend fun signup(param: LoginParam): LiveData<ResultResponse<Token>> {
        return object : NetworkBoundResource<ObjectResponse<Token>, Token>(){
            override fun processResponse(response: ObjectResponse<Token>): Token? = response.data
            override suspend fun createCall(): Response<ObjectResponse<Token>> = api.signup(param)
        }.build().asLiveData()
    }

    override suspend fun checkMailExisted(param: EmailParam): LiveData<ResultResponse<CheckEmailExisted>> {
        return object : NetworkBoundResource<ObjectResponse<CheckEmailExisted>, CheckEmailExisted>(){
            override fun processResponse(response: ObjectResponse<CheckEmailExisted>): CheckEmailExisted? = response.data
            override suspend fun createCall(): Response<ObjectResponse<CheckEmailExisted>>  = api.checkMailExisted(param)
        }.build().asLiveData()
    }

    override suspend fun checkOTPSignUp(param: OTPParam): LiveData<ResultResponse<Token>> {
        return object : NetworkBoundResource<ObjectResponse<Token>, Token>(){
            override fun processResponse(response: ObjectResponse<Token>): Token? = response.data
            override suspend fun createCall(): Response<ObjectResponse<Token>> = api.checkOTPSignUp(param)
        }.build().asLiveData()
    }

    override suspend fun updateInformation(param: UpdateInfoParam): LiveData<ResultResponse<UserResponse>> {
        return object : NetworkBoundResource<ObjectResponse<UserResponse>, UserResponse>(){
            override fun processResponse(response: ObjectResponse<UserResponse>): UserResponse? = response.data
            override suspend fun createCall(): Response<ObjectResponse<UserResponse>> = api.updateInformation(param)
        }.build().asLiveData()
    }

    override suspend fun forgotPassword(param: EmailParam): LiveData<ResultResponse<Token>> {
        return object : NetworkBoundResource<ObjectResponse<Token>, Token>(){
            override fun processResponse(response: ObjectResponse<Token>): Token? = response.data
            override suspend fun createCall(): Response<ObjectResponse<Token>> = api.forgotPassword(param)
        }.build().asLiveData()
    }

    override suspend fun checkOTPForgotPassword(param: OTPParam): LiveData<ResultResponse<Token>> {
        return object : NetworkBoundResource<ObjectResponse<Token>, Token>(){
            override fun processResponse(response: ObjectResponse<Token>): Token? = response.data
            override suspend fun createCall(): Response<ObjectResponse<Token>> = api.checkOTPForgotPassword(param)
        }.build().asLiveData()
    }

}