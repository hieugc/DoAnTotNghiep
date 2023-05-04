package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Notification(
    val id: String? = null,
    val title: String? = null,
    val content: String? = null,
    val type: Int? = null,
    val isSeen: Boolean? = null,
    val imageUrl: String? = null,
    val idType: Int? = null,
    val createdDate: String
) : Parcelable