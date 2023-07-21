package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class CheckEmailExisted(
    val isExisted: Boolean
): Parcelable