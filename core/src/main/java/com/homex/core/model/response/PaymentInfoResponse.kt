package com.homex.core.model.response

import android.os.Parcelable
import com.homex.core.model.Home
import com.homex.core.model.Rating
import com.homex.core.model.Request
import kotlinx.parcelize.Parcelize

@Parcelize
class PaymentInfoResponse(
    val redirect: String = ""
): Parcelable