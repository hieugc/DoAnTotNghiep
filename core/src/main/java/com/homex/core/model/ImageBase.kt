package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class ImageBase(
    val name: String? = null,
    val folder: String? = null,
    val id: Int?  = null,
    val data : String?  = null
): Parcelable