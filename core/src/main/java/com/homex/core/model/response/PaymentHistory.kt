package com.homex.core.model.response

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class PaymentHistory (
    val amount: Long,
    val status: Boolean,
    val content: String,
    val createdDate: String
): Parcelable