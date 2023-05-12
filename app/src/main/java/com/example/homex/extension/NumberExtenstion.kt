package com.example.homex.extension

import android.content.Context
import java.text.DecimalFormat
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

fun Float.dpToPx(context: Context): Int{
    return (this * context.resources.displayMetrics.density).toInt()
}

fun Long.longToDate(): String{
    val date = Date(this)
    val format = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
    return format.format(date)
}
fun Long.longToFormat(format: String): String{
    val date = Date(this)
    val formatter = SimpleDateFormat(format, Locale.getDefault())
    return formatter.format(date)
}

fun Long.thousandSeparator(): String{
    this.let {
        val numberFormat = DecimalFormat("#,###")
        return numberFormat.format(it)
    }
}