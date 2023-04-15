package com.example.homex.extension

import java.text.DateFormat
import java.text.SimpleDateFormat
import java.util.*

const val MILLIS_IN_A_DAY = 1000 * 60 * 60 * 24


fun String?.betweenDays(endDate: String?): Int?{
    val format = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
    val start = this?.let { format.parse(it) }
    val end = endDate?.let { format.parse(it) }
    if (start != null && end != null) {
        return ((end.time-start.time)/ MILLIS_IN_A_DAY).toInt()
    }
    return null
}

fun String?.iso8601BetweenDays(date: Date): Int?{
    val format = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())
    val start = this?.let { format.parse(it) }
    if (start != null) {
        return ((date.time - start.time)/ MILLIS_IN_A_DAY).toInt()
    }
    return null
}

fun String?.convertToRelativeDateTime(): String?{
    val date = Date()
    val dateGap = this?.iso8601BetweenDays(date)
    if (dateGap != null){
        return when{
            dateGap == 0 ->{
                this?.formatIso8601ToFormat(format = "HH:mm")
            }
            dateGap < 7 ->{
                this?.formatIso8601ToFormat(format = "EEEE")
            }
            dateGap < 180->{
                this?.formatIso8601ToFormat(format = "dd MMMM")
            }
            else->{
                this?.formatIso8601ToFormat(format = "dd MMMM, yyyy")
            }
        }
    }else{
        return ""
    }
}

fun String?.convertToRelativeDate(): String?{
    val date = Date()
    val dateGap = this?.iso8601BetweenDays(date)
    if (dateGap != null){
        return when{
            dateGap < 7 ->{
                this?.formatIso8601ToFormat(format = "EEEE")
            }
            dateGap < 180->{
                this?.formatIso8601ToFormat(format = "dd MMMM")
            }
            else->{
                this?.formatIso8601ToFormat(format = "dd MMMM, yyyy")
            }
        }
    }else{
        return ""
    }
}

fun String.formatIso8601ToFormat(format: String = "dd/MM/yyyy"): String{
    val df1: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())
//    df1.timeZone = TimeZone.getTimeZone("UTC")
    val result1 = this.let { df1.parse(it) }
    val df2: DateFormat = SimpleDateFormat(format, Locale.getDefault())
//    df2.timeZone = TimeZone.getTimeZone("Asia/Singapore")
    result1?.let { return df2.format(it) }
    return ""
}

fun String.convertIso8601ToLong(): Long?{
    val df1: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.getDefault())
//    df1.timeZone = TimeZone.getTimeZone("UTC")
    return this.let {
        df1.parse(it)?.time
    }
}