package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.response.RequestResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.param.request.EditRequestParam
import com.homex.core.repository.RequestRepository
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class RequestViewModel(private val repository: RequestRepository): ViewModel() {
    val messageLiveData = MediatorLiveData<JsonObject?>()
    val requestResponseLiveData = MediatorLiveData<RequestResponse?>()
    val requestResponseListLiveDate = MediatorLiveData<ArrayList<RequestResponse>?>()

    fun createNewRequest(param: CreateRequestParam){
        viewModelScope.launch {
            messageLiveData.addSource(repository.createNewRequest(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessCreateRequest", "${it.data}")
                        messageLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessCreateRequest", "hello")
                    }
                }
            }
        }
    }

    fun editRequest(param: EditRequestParam){
        viewModelScope.launch {
            messageLiveData.addSource(repository.updateRequest(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessEditRequest", "${it.data}")
                        messageLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessEditRequest", "hello")
                    }
                }
            }
        }
    }

    fun deleteRequest(body: RequestBody){
        viewModelScope.launch {
            messageLiveData.addSource(repository.deleteRequest(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessDeleteRequest", "${it.data}")
                        messageLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessDeleteRequest", "hello")
                    }
                }
            }
        }
    }

    fun getRequestDetail(id: Int){
        viewModelScope.launch {
            requestResponseLiveData.addSource(repository.getRequestById(id)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetRequest", "${it.data}")
                        requestResponseLiveData.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetRequest", "hello")
                    }
                }
            }
        }
    }

    fun getRequestHistory(){
        viewModelScope.launch {
            requestResponseListLiveDate.addSource(repository.getRequestSent()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHistory", "${it.data}")
                        requestResponseListLiveDate.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetHistory", "hello")
                    }
                }
            }
        }
    }

    fun getRequestByHouse(id: Int){
        viewModelScope.launch {
            requestResponseListLiveDate.addSource(repository.getRequestByHouse(id)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetByHouse", "${it.data}")
                        requestResponseListLiveDate.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetByHouse", "hello")
                    }
                }
            }
        }
    }
}