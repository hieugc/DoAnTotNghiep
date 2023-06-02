package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.Profile
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.PaymentHistory
import com.homex.core.model.response.PaymentInfoResponse
import com.homex.core.param.auth.PasswordParam
import com.homex.core.param.profile.TopUpPointParam
import com.homex.core.repository.ProfileRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class ProfileViewModel(private val repository: ProfileRepository): ViewModel() {
    val passwordLiveData = MediatorLiveData<JsonObject?>()
    val updateProfileLiveData = MediatorLiveData<JsonObject?>()
    val topUpLiveData = MediatorLiveData<PaymentInfoResponse?>()
    val pointLiveData = MediatorLiveData<Long?>()
    val paymentHistoryAllLiveData = MediatorLiveData<ArrayList<PaymentHistory>?>()
    val paymentHistoryReceivedLiveData = MediatorLiveData<ArrayList<PaymentHistory>?>()
    val paymentHistoryUsedLiveData = MediatorLiveData<ArrayList<PaymentHistory>?>()
    val userInfoLiveData = MediatorLiveData<Profile?>()

    fun updatePassword(param: PasswordParam){
        viewModelScope.launch {
            passwordLiveData.addSource(repository.updatePassword(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        passwordLiveData.value = it.data
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

    fun updateProfile(body: RequestBody){
        AppEvent.showPopUp()
        viewModelScope.launch {
            updateProfileLiveData.addSource(repository.updateProfile(body)){
                when (it) {
                    is ResultResponse.Success -> {
                        updateProfileLiveData.value = it.data
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

    fun topUpPoint(param: TopUpPointParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            topUpLiveData.addSource(repository.topUpPoint(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        topUpLiveData.value = it.data
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

    fun getPoint(){
        AppEvent.showPopUp()
        viewModelScope.launch {
            pointLiveData.addSource(repository.getPoint()){
                when (it) {
                    is ResultResponse.Success -> {
                        pointLiveData.value = it.data
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

    fun getHistoryAll(){
        viewModelScope.launch {
            paymentHistoryAllLiveData.addSource(repository.getHistoryAll()){
                when (it) {
                    is ResultResponse.Success -> {
                        paymentHistoryAllLiveData.value = it.data
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

    fun getHistoryReceived(){
        viewModelScope.launch {
            paymentHistoryReceivedLiveData.addSource(repository.getHistoryReceived()){
                when (it) {
                    is ResultResponse.Success -> {
                        paymentHistoryReceivedLiveData.value = it.data
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

    fun getHistoryUsed(){
        viewModelScope.launch {
            paymentHistoryUsedLiveData.addSource(repository.getHistoryUsed()){
                when (it) {
                    is ResultResponse.Success -> {
                        paymentHistoryUsedLiveData.value = it.data
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

    fun getUserInfo(){
        AppEvent.showPopUp()
        viewModelScope.launch {
            userInfoLiveData.addSource(repository.getUserInfo()){
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
}