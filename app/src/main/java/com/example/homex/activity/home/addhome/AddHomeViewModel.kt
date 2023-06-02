package com.example.homex.activity.home.addhome

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.homex.core.model.HomeStatus
import com.homex.core.model.ImageBase
import java.io.File

class AddHomeViewModel: ViewModel() {
    val name = MutableLiveData("")
    val option = MutableLiveData(1)
    val description = MutableLiveData("")
    val people = MutableLiveData(1)
    val bed = MutableLiveData(1)
    val bedroom = MutableLiveData(1)
    val bathroom = MutableLiveData(1)
    val square = MutableLiveData(0)
    val location = MutableLiveData("")
    val lat = MutableLiveData(0.0)
    val lng = MutableLiveData(0.0)
    val price = MutableLiveData(0)
    val idCity = MutableLiveData(0)
    val idDistrict = MutableLiveData(0)
    val idWard = MutableLiveData(0)
    val utilities = MutableLiveData<List<Int>>(listOf())
    val rules = MutableLiveData<List<Int>>(listOf())
    val files =  MutableLiveData<MutableList<File>?>(mutableListOf())
    val predict = MutableLiveData<Int>()
    //For edit
    val id = MutableLiveData(0)
    val status = MutableLiveData(HomeStatus.VALID.ordinal)
    val images = MutableLiveData<List<ImageBase>>()
    val idRemove = MutableLiveData<MutableList<Int>>()

    val showMap = MutableLiveData(false)
}