package com.homex.core.model.response

import android.os.Parcelable
import com.homex.core.model.Home
import com.homex.core.param.chat.Pagination
import kotlinx.parcelize.Parcelize

@Parcelize
class SearchHomeResponse(
    val houses: ArrayList<Home>? = null,
    val pagination: Pagination? = null
): Parcelable