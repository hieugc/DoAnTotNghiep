package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class OldMessage(
    val id: String? = null,
    val message: String? = null,
    val isMyMessage: Boolean? = null,
    val date: String? = null,
    val isDateItem: Boolean? = null,
    val userID: String? = null
): Parcelable
