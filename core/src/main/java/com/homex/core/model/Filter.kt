package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class Filter(
    val option: Int,
    val priceStart: Int,
    val priceEnd: Int,
    val utils: ArrayList<Int>
): Parcelable