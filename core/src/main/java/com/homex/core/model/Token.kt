package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Token(
    val token: String? = null,
    val expire: String? = null,
): Parcelable