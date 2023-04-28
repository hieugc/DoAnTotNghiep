package com.example.homex.extension

import android.view.View
import androidx.appcompat.widget.AppCompatImageView
import androidx.appcompat.widget.AppCompatTextView
import androidx.core.content.ContextCompat
import androidx.databinding.BindingAdapter
import com.bumptech.glide.Glide
import com.bumptech.glide.load.resource.bitmap.RoundedCorners
import com.bumptech.glide.request.RequestOptions
import com.example.homex.R
import com.homex.core.model.HomeStatus
import com.homex.core.util.AppEvent
import java.text.DateFormat
import java.text.SimpleDateFormat
import java.util.*


@BindingAdapter(value = ["setVisibility"])
fun View.setVisibility(visible: Boolean) {
    visibility = if(visible) View.VISIBLE else View.INVISIBLE
}

@BindingAdapter(value = ["setVisibilityGone"])
fun View.setVisibilityGone(visible: Boolean) {
    visibility = if(visible) View.VISIBLE else View.GONE
}

@BindingAdapter(value = ["loadImage"])
fun AppCompatImageView.loadImage(url: String?){
    Glide.with(context)
        .asBitmap()
        .placeholder(R.drawable.ic_baseline_image_24)
        .error(R.mipmap.location)
        .load(url)
        .into(this)
}

@BindingAdapter(value = ["loadAvatar"])
fun AppCompatImageView.loadAvatar(url: String?){
    Glide.with(context)
        .asBitmap()
        .placeholder(R.drawable.ic_user_solid)
        .error(R.mipmap.avatar)
        .load(url)
        .into(this)
}

@BindingAdapter(value = ["dob", "gender"])
fun AppCompatTextView.getAge(dob: String?, gender: Boolean?){
    val df1: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.ENGLISH)
//    df1.timeZone = TimeZone.getTimeZone("UTC")
    val result1 = dob?.let { df1.parse(it) }
    val birthDate = Calendar.getInstance()
//    birthDate.timeZone = TimeZone.getTimeZone("Asia/Vietnam")
    val now = Calendar.getInstance()
//    now.timeZone = TimeZone.getTimeZone("Asia/Vietnam")
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
            this.text = context.getString(R.string.status_active)
            this.setTextColor(ContextCompat.getColor(context, R.color.green))
        }
        HomeStatus.PENDING.ordinal->{
            this.text = context.getString(R.string.status_pending)
            this.setTextColor(ContextCompat.getColor(context, R.color.orange))
        }
        HomeStatus.DISABLE.ordinal->{
            this.text = context.getString(R.string.status_hidden)
            this.setTextColor(ContextCompat.getColor(context, R.color.gray))
        }
        HomeStatus.SWAPPED.ordinal->{
            this.text = context.getString(R.string.status_swapping)
            this.setTextColor(ContextCompat.getColor(context, R.color.yellow))
        }
    }
}


@BindingAdapter(value =["requestStatus"])
fun AppCompatTextView.setRequestStatus(requestStatus: Int?){
    when(requestStatus){

    }
}


@BindingAdapter(value = ["formatDate"])
fun AppCompatTextView.getFormatDate(formatDate: String?){
    val df1: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())
    if (formatDate != null) {
        val result1 = df1.parse(formatDate)
        val df2: DateFormat = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
        result1?.let {
            text = df2.format(it)
            return
        }
    }
    text = ""
}

@BindingAdapter(value = ["price", "startDate", "endDate"])
fun AppCompatTextView.getPrice(price: Int?, startDate: String?, endDate: String?){
    if (price != null && startDate != null && endDate != null){
        val days = startDate.betweenDays(endDate)
        if (days != null){
            val p = days * price
            text = p.toString()
            return
        }
    }
    text = "0"
}