package com.homex.core.model.response

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class CircleRequest(
    val prevNode: CircleNode? = null,
    val myNode: CircleNode? = null,
    val nextNode: CircleNode? = null,
    val startDate: String? = null,
    val endDate: String? = null,
    val status: Int? = null,
    val id: Int? = null
) : Parcelable