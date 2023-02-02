package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Message(
    val id: String? = null,
    val message: String? = null,
    val isMyMessage: Boolean? = null
): Parcelable
