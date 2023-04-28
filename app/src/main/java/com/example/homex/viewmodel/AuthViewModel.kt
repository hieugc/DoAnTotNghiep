package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.CheckEmailExisted
import com.homex.core.model.Token
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.UserResponse
import com.homex.core.param.auth.*
import com.homex.core.repository.AuthRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class AuthViewModel(private val repository: AuthRepository): ViewModel() {
    val loginLiveData = MediatorLiveData<UserResponse?>()
    val signupLiveData = MediatorLiveData<Token?>()
    val checkEmailLiveData = MediatorLiveData<CheckEmailExisted?>()
    val otpLiveData = MediatorLiveData<Token?>()
    val userInfoLiveData = MediatorLiveData<UserResponse?>()
    val forgotLiveData = MediatorLiveData<Token?>()

    fun login(param: LoginParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            loginLiveData.addSource(repository.login(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessLogin", "${it.data}")
                        loginLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun signup(param: LoginParam){
        viewModelScope.launch {
            signupLiveData.addSource(repository.signup(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessSignup", "${it.data}")
                        signupLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun checkEmailExisted(param: EmailParam){
        viewModelScope.launch {
            checkEmailLiveData.addSource(repository.checkMailExisted(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessCheckEmail", "${it.data}")
                        checkEmailLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun checkOTPSignUp(param: OTPParam){
        viewModelScope.launch {
            otpLiveData.addSource(repository.checkOTPSignUp(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessCheckOTP", "${it.data}")
                        otpLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun updateInformation(param: UpdateInfoParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            userInfoLiveData.addSource(repository.updateInformation(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessUpdateInfo", "${it.data}")
                        userInfoLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun forgotPassword(param: EmailParam){
        viewModelScope.launch {
            forgotLiveData.addSource(repository.forgotPassword(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessForgot", "${it.data}")
                        forgotLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun checkOTPForgotPassword(param: OTPParam){
        viewModelScope.launch {
            otpLiveData.addSource(repository.checkOTPForgotPassword(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessOTPForgot", "${it.data}")
                        otpLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

}