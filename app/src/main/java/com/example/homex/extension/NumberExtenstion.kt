package com.example.homex.extension

import android.content.Context

fun Float.dpToPx(context: Context): Int{
    return (this * context.resources.displayMetrics.density).toInt()
}