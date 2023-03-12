package com.homex.core.model

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
class CheckEmailExisted(
    val isExisted: Boolean
): Parcelable