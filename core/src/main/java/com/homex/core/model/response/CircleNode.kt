package com.homex.core.model.response

import android.os.Parcelable
import com.homex.core.model.Home
import com.homex.core.model.Profile
import com.homex.core.model.Rating
import kotlinx.parcelize.Parcelize

@Parcelize
class CircleNode(
    val house: Home? = null,
    val idRequest: Int? = null,
    val rating: Rating? = null,
    val user: Profile? = null,
    val imageHouse: ImgHouse? = null,
    val status: Int? = null
) : Parcelable

@Parcelize
class ImgHouse(
    val name: String? = null,
    val id: Int? = null,
    val data: String? = null,
) : Parcelable