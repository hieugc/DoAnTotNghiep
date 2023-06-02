package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Message(
    val message: String? = null,
    val idReply: Int? = 0,
    var isSeen: Boolean? = null,
    val idSend: String? = null,
    val id: Int? = null,
    val createdDate: String? = null,
    val isDateItem: Boolean? = null
): Parcelable