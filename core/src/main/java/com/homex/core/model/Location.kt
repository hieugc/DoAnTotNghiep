package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Location(
    val name: String? = null,
    val id: Int? = null,
    val location: Point? = null,
    val imageUrl: String? = null,
    val isDeleted: Boolean? = null,
): Parcelable

@Parcelize
data class Point(
    val lat: Double? = null,
    val lng: Double? = null
): Parcelable

@Parcelize
data class BingLocation(
    val id: Int? = null,
    val name: String? = null,
    val bingName: String? = null
): Parcelable