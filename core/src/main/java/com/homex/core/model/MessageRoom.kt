package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class MessageRoom(
    val idRoom: Int? = null,
    val userMessages: ArrayList<UserMessage>? = null,
    val messages: ArrayList<Message>? = null
): Parcelable