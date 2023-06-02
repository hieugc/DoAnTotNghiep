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
    var userRating: Double? = null,
    var numberOfHouses: Int? = null

): Parcelable {
    fun getFullName(): String{
        return "${lastName?:""} ${firstName?:""}"
    }

    fun getDobDDMMYYYY(): String {
        val formatter = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())
        formatter.timeZone = TimeZone.getTimeZone("UTC")
        val value = formatter.parse(birthDay)
        val dateFormatter =
            SimpleDateFormat("dd/MM/yyyy", Locale.getDefault()) //this format changeable
        dateFormatter.timeZone = TimeZone.getDefault()
        return dateFormatter.format(value)
    }
}