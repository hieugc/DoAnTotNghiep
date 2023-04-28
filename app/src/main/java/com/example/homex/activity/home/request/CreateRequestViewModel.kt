package com.example.homex.activity.home.request

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.homex.core.model.CalendarDate
import com.homex.core.model.DateRange
import com.homex.core.model.Home

class CreateRequestViewModel: ViewModel() {
    val house = MutableLiveData<Home>(null)
    val houseSwap = MutableLiveData<Home>(null)
    val type = MutableLiveData(2)
    val startDate = MutableLiveData<CalendarDate>(null)
    val endDate = MutableLiveData<CalendarDate>(null)
    val inValidRangeDates = MutableLiveData<ArrayList<DateRange>>()
    val myInValidRangeDates = MutableLiveData<ArrayList<DateRange>>()
}