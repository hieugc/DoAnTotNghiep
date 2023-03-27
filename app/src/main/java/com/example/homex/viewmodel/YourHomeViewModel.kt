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
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch
import okhttp3.MultipartBody
import okhttp3.RequestBody

class YourHomeViewModel(private val repository: YourHomeRepository): ViewModel() {
    val myHomesLiveData = MediatorLiveData<MyHomeResponse?>()
    val messageLiveData =  MediatorLiveData<JsonObject?>()
    val editMessageLiveData = MediatorLiveData<JsonObject?>()
    val homeDetailsLiveData = MediatorLiveData<Home?>()
    val listHomeLiveData = MediatorLiveData<ArrayList<Home>?>()

    fun createHome(body: RequestBody){
        AppEvent.showPopUp()
        viewModelScope.launch {
            messageLiveData.addSource(repository.createHome(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessCreateHome", "${it.data}")
                        messageLiveData.value = it.data
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

    fun editHome(body: RequestBody){
        AppEvent.showPopUp()
        viewModelScope.launch {
            editMessageLiveData.addSource(repository.editHome(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessEditHome", "${it.data}")
                        editMessageLiveData.value = it.data
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

    fun deleteHome(id: Int){
        AppEvent.showPopUp()
        viewModelScope.launch {
            messageLiveData.addSource(repository.deleteHome(id)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessDeleteHome", "${it.data}")
                        messageLiveData.value = it.data
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

    fun getMyHomes(page: Int){
        viewModelScope.launch {
            myHomesLiveData.addSource(repository.getMyHomes(page)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHome", "${it.data}")
                        myHomesLiveData.value = it.data
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

    fun getHomeDetails(id: Int){
        AppEvent.showPopUp()
        viewModelScope.launch {
            homeDetailsLiveData.addSource(repository.getHomeByDetails(id)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHomeDetails", "${it.data}")
                        homeDetailsLiveData.value = it.data
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

    fun getHomeByUser(userAccess: String){
        viewModelScope.launch {
            listHomeLiveData.addSource(repository.getHomeByUser(userAccess)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHomeUser", "${it.data}")
                        listHomeLiveData.value = it.data
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