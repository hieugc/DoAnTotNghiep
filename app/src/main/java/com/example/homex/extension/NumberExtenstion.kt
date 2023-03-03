package com.example.homex.extension

import android.content.Context
import java.text.SimpleDateFormat
import java.util.*

fun Float.dpToPx(context: Context): Int{
    return (this * context.resources.displayMetrics.density).toInt()
}

fun Long.longToDate(): String{
    val date = Date(this)
    val format = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
    return format.format(date)
}