package com.example.homex.activity.home.addhome

import android.net.Uri
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import java.io.File

class FileViewModel: ViewModel() {
    val file = MutableLiveData<MutableList<Pair<Uri, Boolean>>?>(mutableListOf())
    val tmpFiles = MutableLiveData<MutableList<File>?>()

    fun addFile(uri: Uri, f: Boolean){
        viewModelScope.launch {
            val tmp = file.value
            tmp?.add(Pair(uri, f))
            file.postValue(tmp)
        }
    }
}