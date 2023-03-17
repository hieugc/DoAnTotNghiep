package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class UserMessage(
    val userName: String? = null,
    val imageUrl: String? = null,
    val userAccess: String? = null
): Parcelable