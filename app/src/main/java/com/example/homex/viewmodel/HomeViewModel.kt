package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.model.LocationSuggestion
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.SearchHomeResponse
import com.homex.core.repository.HomeRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch

class HomeViewModel(private val repository: HomeRepository): ViewModel() {
    val popularHome = MediatorLiveData<ArrayList<Home>?>()
    val popularLocation = MediatorLiveData<ArrayList<Location>?>()
    val searchList = MediatorLiveData<SearchHomeResponse?>()
    val suggestion = MediatorLiveData<ArrayList<LocationSuggestion>?>()

    fun getSuggestion(query: String){
        viewModelScope.launch {
            suggestion.addSource(repository.getLocationSuggestion(query)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetSuggestion", "${it.data}")
                        suggestion.value = it.data
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

    fun getPopularHome(){
        viewModelScope.launch {
            popularHome.addSource(repository.getPopularHome()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetLocation", "${it.data}")
                        popularHome.value = it.data
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

    fun getPopularLocation(){
        viewModelScope.launch {
            popularLocation.addSource(repository.getPopularLocation()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetHome", "${it.data}")
                        popularLocation.value = it.data
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

    fun searchHome(
        idCity: Int,
        people: Int? = null,
        idDistrict: Int? = null,
        startDate: String? = null,
        endDate: String? = null,
        startPrice: Int? = null,
        endPrice: Int? = null,
        utilities: ArrayList<Int>? = null,
        sortBy: Int,
        page: Int,
        limit: Int
    ){
        AppEvent.showPopUp()
        viewModelScope.launch {
            searchList.addSource(repository.searchHome(idCity, people, idDistrict, startDate, endDate, startPrice, endPrice, utilities, sortBy, page, limit)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessSearchHome", "${it.data}")
                        searchList.value = it.data
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