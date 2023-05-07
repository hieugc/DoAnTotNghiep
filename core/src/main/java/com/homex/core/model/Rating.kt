package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize


@Parcelize
class UserRating(
    val user: Profile? = null,
    val feedBack: Rating? = null
): Parcelable

@Parcelize
class Rating(
    val updatedDate: String? = null,
    val createdDate: String? = null,
    val content: String? = null,
    val rating: Int? = null,
    val ratingUser: Int? = null,
    val userName: String? = null,
    val id: Int? = null,
    val idRequest: Int? = null,
    val isOwner: Boolean? = null
): Parcelable