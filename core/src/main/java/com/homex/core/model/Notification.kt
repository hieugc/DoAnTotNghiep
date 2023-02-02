package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Notification(
    val id: String? = null,
    val title: String? = null,
    val content: String? = null,
    val type: String? = null,
    val isRead: Boolean? = null
): Parcelable