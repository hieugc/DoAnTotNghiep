package com.homex.core.model.response

import android.os.Parcelable
import com.homex.core.model.Profile
import kotlinx.parcelize.Parcelize

@Parcelize
class UserResponse(
    val token: String? = null,
    var userInfo: Profile? = null,
    val expire: String? = null
): Parcelable