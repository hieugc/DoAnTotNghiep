package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.BingLocation
import com.homex.core.model.Home
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MyHomeResponse
import com.homex.core.repository.YourHomeRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class YourHomeViewModel(private val repository: YourHomeRepository): ViewModel() {
    val myHomesLiveData = MediatorLiveData<MyHomeResponse?>()
    val messageLiveData =  MediatorLiveData<JsonObject?>()
    val editMessageLiveData = MediatorLiveData<JsonObject?>()
    val homeDetailsLiveData = MediatorLiveData<Home?>()
    val listHomeLiveData = MediatorLiveData<ArrayList<Home>?>()
    val cityLiveData = MediatorLiveData<ArrayList<BingLocation>?>()
    val districtLiveData = MediatorLiveData<ArrayList<BingLocation>?>()
    val wardLiveData = MediatorLiveData<ArrayList<BingLocation>?>()
    val predictLiveData = MediatorLiveData<Int?>()

    fun createHome(body: RequestBody){
        AppEvent.showPopUp()
        viewModelScope.launch {
            messageLiveData.addSource(repository.createHome(body)){
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

    fun editHome(body: RequestBody){
        AppEvent.showPopUp()
        viewModelScope.launch {
            editMessageLiveData.addSource(repository.editHome(body)){
                when (it) {
                    is ResultResponse.Success -> {
                        editMessageLiveData.value = it.data
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

    fun deleteHome(id: Int){
        AppEvent.showPopUp()
        viewModelScope.launch {
            messageLiveData.addSource(repository.deleteHome(id)){
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

    fun getMyHomes(page: Int){
        viewModelScope.launch {
            myHomesLiveData.addSource(repository.getMyHomes(page)){
                when (it) {
                    is ResultResponse.Success -> {
                        myHomesLiveData.value = it.data
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

    fun getHomeDetails(id: Int){
        AppEvent.showPopUp()
        viewModelScope.launch {
            homeDetailsLiveData.addSource(repository.getHomeByDetails(id)){
                when (it) {
                    is ResultResponse.Success -> {
                        homeDetailsLiveData.value = it.data
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

    fun getHomeByUser(userAccess: String){
        viewModelScope.launch {
            listHomeLiveData.addSource(repository.getHomeByUser(userAccess)){
                when (it) {
                    is ResultResponse.Success -> {
                        listHomeLiveData.value = it.data
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

    fun getCity(){
        viewModelScope.launch {
            cityLiveData.addSource(repository.getCity()){
                when (it) {
                    is ResultResponse.Success -> {
                        cityLiveData.value = it.data
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

    fun getDistrict(id: Int){
        viewModelScope.launch {
            districtLiveData.addSource(repository.getDistrict(id)){
                when (it) {
                    is ResultResponse.Success -> {
                        districtLiveData.value = it.data
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

    fun getWard(id: Int){
        viewModelScope.launch {
            wardLiveData.addSource(repository.getWard(id)){
                when (it) {
                    is ResultResponse.Success -> {
                        wardLiveData.value = it.data
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

    fun predictHouse(idCity: Int, lat: Double, lng: Double, rating: Double, area: Int){
        viewModelScope.launch {
            predictLiveData.addSource(repository.predictHouse(idCity, lat, lng, rating, area)){
                when (it) {
                    is ResultResponse.Success -> {
                        predictLiveData.value = it.data
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