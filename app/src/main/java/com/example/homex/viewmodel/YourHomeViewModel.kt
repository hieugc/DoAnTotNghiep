package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.Home
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MyHomeResponse
import com.homex.core.param.yourhome.IdParam
import com.homex.core.repository.YourHomeRepository
import kotlinx.coroutines.launch
import okhttp3.MultipartBody
import okhttp3.RequestBody

class YourHomeViewModel(private val repository: YourHomeRepository): ViewModel() {
    val myHomesLiveData = MediatorLiveData<MyHomeResponse?>()
    val messageLiveData =  MediatorLiveData<JsonObject?>()
    val homeDetailsLiveData = MediatorLiveData<Home?>()

    fun createHome(body: RequestBody){
        viewModelScope.launch {
            messageLiveData.addSource(repository.createHome(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessCreateHome", "${it.data}")
                        messageLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessCreateHome", "hello")
                    }
                }
            }
        }
    }

    fun editHome(body: RequestBody){
        viewModelScope.launch {
            messageLiveData.addSource(repository.editHome(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessEditHome", "${it.data}")
                        messageLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessEditHome", "hello")
                    }
                }
            }
        }
    }

    fun deleteHome(id: Int){
        viewModelScope.launch {
            messageLiveData.addSource(repository.deleteHome(id)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessDeleteHome", "${it.data}")
                        messageLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessDeleteHome", "hello")
                    }
                }
            }
        }
    }

    fun getMyHomes(page: Int){
        viewModelScope.launch {
            myHomesLiveData.addSource(repository.getMyHomes(page)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHome", "${it.data}")
                        myHomesLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetHome", "hello")
                    }
                }
            }
        }
    }

    fun getHomeDetails(id: Int){
        viewModelScope.launch {
            homeDetailsLiveData.addSource(repository.getHomeByDetails(id)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHomeDetails", "${it.data}")
                        homeDetailsLiveData.value = it.data
                    }
                    else -> {
                        Log.e("FailedGetHomeDetails", "hello")
                    }
                }
            }
        }
    }
}