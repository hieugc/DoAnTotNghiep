package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class TransHistory(
    val title: String? = null,
) : Parcelable