package com.example.homex.extension

import android.view.View
import android.widget.ImageView
import androidx.appcompat.widget.AppCompatButton
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
fun ImageView.loadAvatar(url: String?){
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
            this.setTextColor(ContextCompat.getColor(context, R.color.done))
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
        RequestStatus.WAITING.ordinal->{
            this.text = context.getString(R.string.status_waiting)
            this.setTextColor(ContextCompat.getColor(context, R.color.orange))
        }
        RequestStatus.ACCEPTED.ordinal->{
            this.text = context.getString(R.string.status_accepted)
            this.setTextColor(ContextCompat.getColor(context, R.color.yellow))
        }
        RequestStatus.REJECTED.ordinal->{
            this.text = context.getString(R.string.status_rejected)
            this.setTextColor(ContextCompat.getColor(context, R.color.red))
        }
        RequestStatus.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.status_checkin)
            this.setTextColor(ContextCompat.getColor(context, R.color.primary))
        }
        RequestStatus.REVIEWING.ordinal->{
            this.text = context.getString(R.string.status_reviewing)
            this.setTextColor(ContextCompat.getColor(context, R.color.green))
        }
        RequestStatus.DONE.ordinal->{
            this.text = context.getString(R.string.status_done)
            this.setTextColor(ContextCompat.getColor(context, R.color.done))
        }
    }
}

@BindingAdapter(value =["requestStatus", "startDate", "endDate"])
fun AppCompatButton.setRequestStatus(requestStatus: Int?, startDate: String?, endDate: String?){
    when(requestStatus){
        RequestStatus.WAITING.ordinal->{
            this.text = context.getString(R.string.detail)
        }
        RequestStatus.ACCEPTED.ordinal->{
            this.text = context.getString(R.string.check_in)
            val date = Date()
            if (date.time.longToDate() == startDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val start = startDate?.convertIso8601ToLong()
            if (start != null && date.time <= start){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        RequestStatus.REJECTED.ordinal->{
            this.text = context.getString(R.string.contact)
        }
        RequestStatus.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.check_out)
            val date = Date()
            if (date.time.longToDate() == endDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val end = endDate?.convertIso8601ToLong()
            if (end != null && date.time <= end){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        RequestStatus.REVIEWING.ordinal->{
            this.text = context.getString(R.string.rate)
        }
        RequestStatus.DONE.ordinal->{
            this.text = context.getString(R.string.detail)
        }
    }
}

@BindingAdapter(value =["circleRequestStatus", "startDate", "endDate"])
fun AppCompatButton.setCircleRequestStatus(circleRequestStatus: Int?, startDate: String?, endDate: String?){
    when(circleRequestStatus){
        StatusWaitingRequest.INIT.ordinal->{
            this.text = context.getString(R.string.detail)
        }
        StatusWaitingRequest.ACCEPT.ordinal->{
            this.text = context.getString(R.string.check_in)
            val date = Date()
            if (date.time.longToDate() == startDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val start = startDate?.convertIso8601ToLong()
            if (start != null && date.time <= start){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        StatusWaitingRequest.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.check_out)
            val date = Date()
            if (date.time.longToDate() == endDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val end = endDate?.convertIso8601ToLong()
            if (end != null && date.time <= end){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        StatusWaitingRequest.CHECK_OUT.ordinal->{
            this.text = context.getString(R.string.rate)
        }
        StatusWaitingRequest.ENDED.ordinal->{
            this.text = context.getString(R.string.detail)
        }
    }
}

@BindingAdapter(value =["requestPrimary", "startDate", "endDate"])
fun AppCompatButton.setRequestPrimary(requestStatus: Int?, startDate: String?, endDate: String?){
    when(requestStatus){
        RequestStatus.WAITING.ordinal->{
            this.text = context.getString(R.string.accept_request)
        }
        RequestStatus.ACCEPTED.ordinal->{
            this.text = context.getString(R.string.check_in)
            val date = Date()
            if (date.time.longToDate() == startDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val start = startDate?.convertIso8601ToLong()
            if (start != null && date.time <= start){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        RequestStatus.REJECTED.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.check_out)
            val date = Date()
            if (date.time.longToDate() == endDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val end = endDate?.convertIso8601ToLong()
            if (end != null && date.time <= end){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        RequestStatus.REVIEWING.ordinal->{
            this.text = context.getString(R.string.rate)
        }
        RequestStatus.DONE.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
    }
}

@BindingAdapter(value =["requestSent", "startDate", "endDate"])
fun AppCompatButton.setRequestSent(requestSent: Int?, startDate: String?, endDate: String?){
    when(requestSent){
        RequestStatus.ACCEPTED.ordinal->{
            this.visible()
            this.text = context.getString(R.string.check_in)
            val date = Date()
            if (date.time.longToDate() == startDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val start = startDate?.convertIso8601ToLong()
            if (start != null && date.time <= start){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        RequestStatus.REJECTED.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.CHECK_IN.ordinal->{
            this.visible()
            this.text = context.getString(R.string.check_out)
            val date = Date()
            if (date.time.longToDate() == endDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val end = endDate?.convertIso8601ToLong()
            if (end != null && date.time <= end){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        RequestStatus.REVIEWING.ordinal->{
            this.visible()
            this.text = context.getString(R.string.rate)
        }
        RequestStatus.DONE.ordinal->{
            this.visible()
            this.text = context.getString(R.string.view_rating)
        }
        else->{
            this.visibility = View.GONE
            this.text = ""
        }
    }
}

@BindingAdapter(value =["myStatusCircle", "fullStatusCircle", "startDateCircle", "endDateCircle"])
fun AppCompatButton.setCircleRequestPrimary(myStatus: Int?, fullStatus: Int?, startDate: String?, endDate: String?){
    when(fullStatus){
        StatusWaitingRequest.INIT.ordinal->{
            if(myStatus == fullStatus) {
                this.text = context.getString(R.string.accept_request)
                this.enable()
            } else {
                this.text = "Đã chấp nhận"
                this.disable()
            }
        }
        StatusWaitingRequest.ACCEPT.ordinal->{
            this.text = context.getString(R.string.check_in)
            val date = Date()
            if (date.time.longToDate() == startDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val start = startDate?.convertIso8601ToLong()
            if (start != null && date.time <= start){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        StatusWaitingRequest.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.check_out)
            val date = Date()
            if (date.time.longToDate() == endDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                this.text = ""
                this.visibility = View.GONE
            }
            val end = endDate?.convertIso8601ToLong()
            if (end != null && date.time <= end){
                this.text = ""
                this.visibility = View.GONE
            }
        }
        StatusWaitingRequest.CHECK_OUT.ordinal->{
            this.text = context.getString(R.string.rate)
        }
        else->{
            this.visibility = View.GONE
            this.text = ""
        }
    }
}

@BindingAdapter(value =["myStatusCircle", "fullStatusCircle"])
fun AppCompatButton.setCircleRequestSecondary(myStatusSecondary: Int?, fullStatusSecondary: Int?){
    when(fullStatusSecondary){
        StatusWaitingRequest.INIT.ordinal->{
            if(fullStatusSecondary == myStatusSecondary){
                this.enable()
                this.text = context.getString(R.string.reject_request)
            } else {
                this.text = "Đã từ chối"
                this.disable()
            }
        }
        else -> {
            this.visibility = View.GONE
            this.text = ""
        }
    }
}

@BindingAdapter(value =["requestSecondary"])
fun AppCompatButton.setRequestSecondary(requestStatus: Int?){
    when(requestStatus){
        RequestStatus.WAITING.ordinal->{
            this.text = context.getString(R.string.reject_request)
        }
        RequestStatus.ACCEPTED.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.REJECTED.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.CHECK_IN.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.REVIEWING.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.DONE.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
    }
}

@BindingAdapter(value=["requestText"])
fun AppCompatTextView.setRequestText(requestStatus: Int?){
    when(requestStatus){
        RequestStatus.WAITING.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
        RequestStatus.ACCEPTED.ordinal->{
            this.text = context.getString(R.string.request_accepted)
        }
        RequestStatus.REJECTED.ordinal->{
            this.text = context.getString(R.string.request_rejected)
        }
        RequestStatus.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.request_checkin)
        }
        RequestStatus.REVIEWING.ordinal->{
            this.text = context.getString(R.string.request_reviewing)
        }
        RequestStatus.DONE.ordinal->{
            this.visibility = View.GONE
            this.text = ""
        }
    }
}

@BindingAdapter(value=["requestType"])
fun AppCompatTextView.setRequestType(requestType: Int?){
    when(requestType){
        RequestType.BY_HOME.ordinal->{
            this.text = context.getString(R.string.request_type, context.getString(R.string.request_type_house))
        }
        RequestType.BY_POINT.ordinal->{
            this.text = context.getString(R.string.request_type, context.getString(R.string.request_type_point))
        }
    }
}

@BindingAdapter(value=["requestType"])
fun AppCompatTextView.setRequestType(requestType: String?){
    this.text = context.getString(R.string.request_type, requestType)
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

@BindingAdapter(value = ["formatDate"])
fun AppCompatTextView.getFormatDateTime(formatDate: String?){
    val df1: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())
    if (formatDate != null) {
        val result1 = df1.parse(formatDate)
        val df2: DateFormat = SimpleDateFormat("dd/MM - HH:mm", Locale.getDefault())
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

@BindingAdapter(value =["CircleRequestStatus"])
fun AppCompatTextView.setCircleRequestStatus(requestStatus: Int?){
    when(requestStatus){
        StatusWaitingRequest.INIT.ordinal->{
            this.text = context.getString(R.string.new_request)
            this.setTextColor(ContextCompat.getColor(context, R.color.orange))
        }
        StatusWaitingRequest.ACCEPT.ordinal->{
            this.text = context.getString(R.string.status_accepted)
            this.setTextColor(ContextCompat.getColor(context, R.color.yellow))
        }
        StatusWaitingRequest.CHECK_IN.ordinal->{
            this.text = context.getString(R.string.status_checkin)
            this.setTextColor(ContextCompat.getColor(context, R.color.primary))
        }
        StatusWaitingRequest.CHECK_OUT.ordinal->{
            this.text = context.getString(R.string.status_reviewing)
            this.setTextColor(ContextCompat.getColor(context, R.color.green))
        }
        StatusWaitingRequest.ENDED.ordinal->{
            this.text = context.getString(R.string.status_done)
            this.setTextColor(ContextCompat.getColor(context, R.color.done))
        }
    }
}

@BindingAdapter(value =["userToYourHouse"])
fun AppCompatTextView.setUserToYourHouse(userName: String?){
    this.text = userName + " sẽ đến nhà của bạn"
}