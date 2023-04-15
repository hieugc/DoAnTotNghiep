package com.homex.core.model

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class Home(
    val id: Int? = null,
    val name: String? = null,
    val option: Int? = null,
    val description: String? = null,
    val people: Int? = null,
    val bathRoom: Int? =null,
    val bedRoom: Int? =null,
    val square: Int? = null,
    val location: String? =null,
    val lat: Double? = null,
    val lng:Double? = null,
    val idCity: Int? = null,
    val idDistrict: Int? = null,
    val idWard: Int? = null,
    val price: Int? = null,
    val utilities: List<Int>? = null,
    val rules: List<Int>? = null,
    val images: List<ImageBase>? = null,
    val status: Int? = null,
    val rating: Double? = null,
    val request : Int? = null,
    val userAccess: String? = null,
    val user: Profile? = null,
    val numberRating: Int? = null,
    val bed: Int? = null,
    val ratings: ArrayList<Rating>? = null,
    val cityName: String? = null,
    val districtName: String? = null,
    val inValidRangeDates: ArrayList<DateRange>? = null


): Parcelable{
    fun getHomeType(): String{
        return if (option == 1) "Căn nhà"  else "Căn hộ"
    }
    fun getHomeStatus(): String{
        return when(status){
            HomeStatus.VALID.ordinal-> "Đang hoạt động"
            HomeStatus.PENDING.ordinal-> "Đang kiểm duyệt"
            HomeStatus.SWAPPED.ordinal-> "Đang trao đổi"
            HomeStatus.DISABLE.ordinal-> "Đang ẩn"
            else->""
        }
    }
}

@Parcelize
data class DateRange(
    val startDate: String? = null,
    val endDate: String? = null
): Parcelable

enum class HomeStatus{
    VALID,
    PENDING,
    SWAPPED,
    ENABLE,
    DISABLE,
    ACCEPT,
    REJECT,
    SEEN,
    UNSEEN
}
