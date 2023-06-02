package com.homex.core.model.response

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class PaymentInfoResponse(
    val orderurl: String = "",
    val zptranstoken: String = ""
): Parcelable