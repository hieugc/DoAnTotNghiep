package com.example.homex.activity.home.search

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.homex.core.model.CalendarDate

class SearchViewModel: ViewModel() {
    val idCity = MutableLiveData(0)
    val idDistrict = MutableLiveData(0)
    val people = MutableLiveData(2)
    val startDate = MutableLiveData<CalendarDate>(null)
    val endDate = MutableLiveData<CalendarDate>(null)
}