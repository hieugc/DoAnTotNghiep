package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize
import java.util.*

@Parcelize
data class CalendarDate(
    val time: Date?  = null,
    val dateOfMonth: String? = ""
): Parcelable