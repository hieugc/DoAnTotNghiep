package com.example.homex.extension

import java.text.SimpleDateFormat
import java.util.*

const val MILLIS_IN_A_DAY = 1000 * 60 * 60 * 24


fun String?.betweenDays(endDate: String?): Int?{
    val format = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
    val start = this?.let { format.parse(it) }
    val end = endDate?.let { format.parse(it) }
    if (start != null && end != null) {
        return ((end.time-start.time)/ MILLIS_IN_A_DAY).toInt()
    }
    return null
}