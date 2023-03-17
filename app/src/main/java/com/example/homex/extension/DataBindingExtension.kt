package com.example.homex.extension

import androidx.appcompat.widget.AppCompatTextView
import androidx.core.content.ContextCompat
import androidx.databinding.BindingAdapter
import com.example.homex.R
import com.homex.core.model.HomeStatus
import com.homex.core.util.AppEvent
import java.text.DateFormat
import java.text.SimpleDateFormat
import java.util.*

@BindingAdapter(value = ["dob", "gender"])
fun AppCompatTextView.getAge(dob: String?, gender: Boolean?){
    val df1: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.ENGLISH)
    df1.timeZone = TimeZone.getTimeZone("UTC")
    val result1 = dob?.let { df1.parse(it) }
    val birthDate = Calendar.getInstance()
    birthDate.timeZone = TimeZone.getTimeZone("Asia/Vietnam")
    val now = Calendar.getInstance()
    now.timeZone = TimeZone.getTimeZone("Asia/Vietnam")
    if (result1 != null) {
        birthDate.time = result1
        var age = now.get(Calendar.YEAR) - birthDate.get(Calendar.YEAR)
        if(now.get(Calendar.MONTH) < birthDate.get(Calendar.MONTH))
            age--
        else if(now.get(Calendar.MONTH) == birthDate.get(Calendar.MONTH) && now.get(Calendar.DAY_OF_MONTH) < birthDate.get(
                Calendar.DAY_OF_MONTH))
            age--
        var gen = ""
        if(gender == true){
            gen = "Nữ"
        }else if(gender == false){
            gen = "Nam"
        }

        if(age < 0){
            AppEvent.showPopUpError(message = "Wrong date of birth")
            text = gen
            return
        }
        if(gender == null){
            text = "$age"
            return
        }
        text = "$age, $gen"
    }
//    return if(old != null && gender != null){
//        "${old ?:""}, ${gender?:""}"
//    }else if(old != null) {
//        "${old ?:""}"
//    }else if(gender != null) {
//        gender ?:""
//    }else{
//        ""
//    }
}

@BindingAdapter(value =["status"])
fun AppCompatTextView.setHomeStatus(status: Int?){
    when(status){
        HomeStatus.VALID.ordinal->{
            this.text = "Đang hoạt động"
            this.setTextColor(ContextCompat.getColor(context, R.color.green))
        }
        HomeStatus.PENDING.ordinal->{
            this.text = "Đang kiểm duyệt"
            this.setTextColor(ContextCompat.getColor(context, R.color.orange))
        }
        HomeStatus.DISABLE.ordinal->{
            this.text = "Đang ẩn"
            this.setTextColor(ContextCompat.getColor(context, R.color.gray))
        }
        HomeStatus.SWAPPED.ordinal->{
            this.text = "Đang trao đổi"
            this.setTextColor(ContextCompat.getColor(context, R.color.yellow))
        }
    }
}