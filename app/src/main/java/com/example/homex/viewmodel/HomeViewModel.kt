package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.model.general.ResultResponse
import com.homex.core.repository.HomeRepository
import kotlinx.coroutines.launch

class HomeViewModel(private val repository: HomeRepository): ViewModel() {
    val popularHome = MediatorLiveData<ArrayList<Home>?>()
    val popularLocation = MediatorLiveData<ArrayList<Location>?>()

    fun getPopularHome(){
        viewModelScope.launch {
            popularHome.addSource(repository.getPopularHome()){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetLocation", "${it.data}")
                        popularHome.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetLocation", "hello")
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
                    else -> {
                        Log.e("NotSuccessGetHome", "hello")
                    }
                }
            }
        }
    }
}