package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class Request(
    val idHouse: Int? = null,
    val type: Int? = null,
    val price: Int? = null,
    val idSwapHouse: Int? = null,
    val startDate: String? = null,
    val endDate: String? = null,
    val id: Int? = null,
    val status: Int? = null,
    val user: UserMessage? = null,
    val isOwner: Boolean? = null
): Parcelable
