package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class LocationSuggestion(
    val idCity: Int? = null,
    val idDistrict: Int? = null,
    val cityName: String? = null,
    val districtName: String? = null
): Parcelable