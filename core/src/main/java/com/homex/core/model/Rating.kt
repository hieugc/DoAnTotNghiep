package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class Rating(
    val updatedDate: String? = null,
    val createdDate: String? = null,
    val content: String? = null,
    val rating: Int? = null,
    val ratingUser: Int? = null,
    val id: Int? = null
): Parcelable