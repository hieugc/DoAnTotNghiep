package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class MessageBox(
    val id: String? = null,
    val user: String? = null,
    val preview: String? = null
) : Parcelable