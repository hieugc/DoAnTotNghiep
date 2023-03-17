package com.example.homex.activity.home.addhome

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import okhttp3.MultipartBody
import java.io.File

class AddHomeViewModel: ViewModel() {
    val name = MutableLiveData("")
    val option = MutableLiveData(1)
    val description = MutableLiveData("")
    val people = MutableLiveData(1)
    val bedroom = MutableLiveData(1)
    val bathroom = MutableLiveData(1)
    val square = MutableLiveData(0)
    val location = MutableLiveData("")
    val lat = MutableLiveData(0.0)
    val lng = MutableLiveData(0.0)
    val price = MutableLiveData(0)
    val utilities = MutableLiveData<List<Int>>(listOf())
    val rules = MutableLiveData<List<Int>>(listOf())
    val files =  MutableLiveData<MutableList<File>?>(mutableListOf())

    fun addFile(file: File){
        viewModelScope.launch {
            val tmp = files.value
            tmp?.add(file)
            files.postValue(tmp)
        }
    }
}