package com.homex.core.model.response

import android.os.Parcelable
import com.homex.core.model.Home
import com.homex.core.model.Rating
import com.homex.core.model.Request
import kotlinx.parcelize.Parcelize

@Parcelize
class RequestResponse(
    val house: Home? = null,
    val swapHouse: Home? = null,
    val request: Request? = null,
    val myRating: Rating? = null
): Parcelable