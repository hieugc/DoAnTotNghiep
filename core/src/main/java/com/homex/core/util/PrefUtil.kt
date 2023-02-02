package com.homex.core.util

import android.content.Context
import android.content.SharedPreferences
import com.google.gson.Gson

class PrefUtil constructor(
    private val context: Context,
    private val prefs: SharedPreferences,
    private val gson: Gson
) {
}