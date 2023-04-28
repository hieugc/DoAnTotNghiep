package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
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
    val paymentHistoryLiveData = MediatorLiveData<ArrayList<PaymentHistory>?>()

    fun updatePassword(param: PasswordParam){
        viewModelScope.launch {
            passwordLiveData.addSource(repository.updatePassword(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessPassword", "${it.data}")
                        passwordLiveData.value = it.data
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

    fun updateProfile(body: RequestBody){
        AppEvent.showPopUp()
        viewModelScope.launch {
            updateProfileLiveData.addSource(repository.updateProfile(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessEditProfile", "${it.data}")
                        updateProfileLiveData.value = it.data
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

    fun topUpPoint(param: TopUpPointParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            topUpLiveData.addSource(repository.topUpPoint(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("Success Topup", "${it.data}")
                        topUpLiveData.value = it.data
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

    fun getPoint(){
        AppEvent.showPopUp()
        viewModelScope.launch {
            pointLiveData.addSource(repository.getPoint()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        pointLiveData.value = it.data
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

    fun getHistoryAll(){
        AppEvent.showPopUp()
        viewModelScope.launch {
            paymentHistoryLiveData.addSource(repository.getHistoryAll()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        paymentHistoryLiveData.value = it.data
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

    fun getHistoryReceived(){
        AppEvent.showPopUp()
        viewModelScope.launch {
            paymentHistoryLiveData.addSource(repository.getHistoryReceived()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        paymentHistoryLiveData.value = it.data
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

    fun getHistoryUsed(){
        AppEvent.showPopUp()
        viewModelScope.launch {
            paymentHistoryLiveData.addSource(repository.getHistoryUsed()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        paymentHistoryLiveData.value = it.data
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