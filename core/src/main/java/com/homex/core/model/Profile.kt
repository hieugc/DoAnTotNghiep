package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize
import java.text.SimpleDateFormat
import java.util.*

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

    public fun getDobDDMMYYYY(): String {
        val formatter = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")
        formatter.timeZone = TimeZone.getTimeZone("UTC")
        val value = formatter.parse(birthDay)
        val dateFormatter = SimpleDateFormat("dd/MM/yyyy") //this format changeable
        dateFormatter.timeZone = TimeZone.getDefault()
        val dob = dateFormatter.format(value)
        return dob
    }
}