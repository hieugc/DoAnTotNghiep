package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
class Profile(
    var _id: String? = null,
    var email: String,
    var urlImage: String? = null,
    val active: Boolean? = null,
    var firstName: String? = null,
    var lastName: String? = null,
    var birthDay: String? = null,
    var gender: Boolean? = null,
    var phoneNumber: String? = null,
    var userAccess: String? = null,
    var point: Int? = null,
    var userRating: Int? = null,
    var numberOfHouses: Int? = null

): Parcelable {
    fun getFullName(): String{
        return "${lastName?:""} ${firstName?:""}"
    }

}