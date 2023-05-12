package com.example.homex.viewmodel

import android.os.CountDownTimer
import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.homex.core.model.CheckEmailExisted
import com.homex.core.model.Token
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.UserResponse
import com.homex.core.param.auth.EmailParam
import com.homex.core.param.auth.LoginParam
import com.homex.core.param.auth.OTPParam
import com.homex.core.param.auth.UpdateInfoParam
import com.homex.core.repository.AuthRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch

class AuthViewModel(private val repository: AuthRepository): ViewModel() {
    private lateinit var timer: CountDownTimer
    var seconds = MutableLiveData<Int>()
    val loginLiveData = MediatorLiveData<UserResponse?>()
    val signupLiveData = MediatorLiveData<Token?>()
    val checkEmailLiveData = MediatorLiveData<CheckEmailExisted?>()
    val otpLiveData = MediatorLiveData<Token?>()
    val userInfoLiveData = MediatorLiveData<UserResponse?>()
    val forgotLiveData = MediatorLiveData<Token?>()
    val resendLiveData = MediatorLiveData<Token?>()

    fun resendOTPForgotPassword(){
        viewModelScope.launch {
            resendLiveData.addSource(repository.resendOTPForgotPassword()){
                when (it) {
                    is ResultResponse.Success -> {
                        resendLiveData.value = it.data
                        startTimer()
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun resendOTP(){
        viewModelScope.launch {
            resendLiveData.addSource(repository.resendOTPSignup()){
                when (it) {
                    is ResultResponse.Success -> {
                        resendLiveData.value = it.data
                        startTimer()
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    private fun startTimer(){
        timer = object : CountDownTimer(60000, 1000){
            override fun onTick(p0: Long) {
                val timeLeft = p0/1000

                seconds.value = timeLeft.toInt()
            }

            override fun onFinish() {
            }
        }.start()
    }

    fun login(param: LoginParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            loginLiveData.addSource(repository.login(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        loginLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun signup(param: LoginParam){
        viewModelScope.launch {
            signupLiveData.addSource(repository.signup(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        signupLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun checkEmailExisted(param: EmailParam){
        viewModelScope.launch {
            checkEmailLiveData.addSource(repository.checkMailExisted(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        checkEmailLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun checkOTPSignUp(param: OTPParam){
        viewModelScope.launch {
            otpLiveData.addSource(repository.checkOTPSignUp(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        otpLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun updateInformation(param: UpdateInfoParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            userInfoLiveData.addSource(repository.updateInformation(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        userInfoLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun forgotPassword(param: EmailParam){
        viewModelScope.launch {
            forgotLiveData.addSource(repository.forgotPassword(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        forgotLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun checkOTPForgotPassword(param: OTPParam){
        viewModelScope.launch {
            otpLiveData.addSource(repository.checkOTPForgotPassword(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        otpLiveData.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

}