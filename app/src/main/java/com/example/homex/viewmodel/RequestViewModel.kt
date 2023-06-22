package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.CircleRequest
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.request.CreateRatingParam
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.param.request.EditRequestParam
import com.homex.core.param.request.UpdateRatingParam
import com.homex.core.param.request.UpdateStatusParam
import com.homex.core.repository.RequestRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class RequestViewModel(private val repository: RequestRepository): ViewModel() {
    val messageLiveData = MediatorLiveData<JsonObject?>()
    val requestResponseLiveData = MediatorLiveData<RequestResponse?>()
    val requestResponseListLiveDate = MediatorLiveData<ArrayList<RequestResponse>?>()
    val requestSentResponseListLiveDate = MediatorLiveData<ArrayList<RequestResponse>?>()
    val circleRequestResponseListLiveDate = MediatorLiveData<ArrayList<CircleRequest>?>()

    fun createNewRequest(param: CreateRequestParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            messageLiveData.addSource(repository.createNewRequest(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        messageLiveData.value = it.data
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

    fun editRequest(param: EditRequestParam){
        viewModelScope.launch {
            messageLiveData.addSource(repository.updateRequest(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        messageLiveData.value = it.data
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

    fun deleteRequest(body: RequestBody){
        viewModelScope.launch {
            messageLiveData.addSource(repository.deleteRequest(body)){
                when (it) {
                    is ResultResponse.Success -> {
                        messageLiveData.value = it.data
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

    fun getRequestDetail(id: Int){
        AppEvent.showPopUp()
        viewModelScope.launch {
            requestResponseLiveData.addSource(repository.getRequestById(id)){
                when (it) {
                    is ResultResponse.Success -> {
                        requestResponseLiveData.value = it.data
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

    fun getRequestHistory(){
        viewModelScope.launch {
            requestSentResponseListLiveDate.addSource(repository.getRequestSent()){
                when (it) {
                    is ResultResponse.Success -> {
                        requestSentResponseListLiveDate.value = it.data
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

    fun getRequestByHouse(id: Int){
        viewModelScope.launch {
            requestResponseListLiveDate.addSource(repository.getRequestByHouse(id)){
                when (it) {
                    is ResultResponse.Success -> {
                        requestResponseListLiveDate.value = it.data
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

    fun getPendingRequest(){
        viewModelScope.launch {
            requestResponseListLiveDate.addSource(repository.getPendingRequest()){
                when (it) {
                    is ResultResponse.Success -> {
                        requestResponseListLiveDate.value = it.data
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

    fun updateStatus(param: UpdateStatusParam){
        viewModelScope.launch {
            messageLiveData.addSource(repository.updateStatus(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        messageLiveData.value = it.data
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

    fun createRating(param: CreateRatingParam){
        AppEvent.showPopUp()
        viewModelScope.launch {
            messageLiveData.addSource(repository.createRating(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        messageLiveData.value = it.data
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

    fun updateRating(param: UpdateRatingParam){
        viewModelScope.launch {
            messageLiveData.addSource(repository.updateRating(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        messageLiveData.value = it.data
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

    fun getCircleRequest(){
        viewModelScope.launch {
            circleRequestResponseListLiveDate.addSource(repository.getCircleRequest()){
                when (it) {
                    is ResultResponse.Success -> {
                        circleRequestResponseListLiveDate.value = it.data
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