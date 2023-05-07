package com.example.homex.activity.home.search

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.homex.core.model.CalendarDate
import com.homex.core.model.LocationSuggestion

class SearchViewModel: ViewModel() {
    val idCity = MutableLiveData(0)
    val idDistrict = MutableLiveData(0)
    val people = MutableLiveData(1)
    val location = MutableLiveData("")
    val startDate = MutableLiveData<CalendarDate>(null)
    val endDate = MutableLiveData<CalendarDate>(null)
    val search = MutableLiveData<LocationSuggestion>(null)
}