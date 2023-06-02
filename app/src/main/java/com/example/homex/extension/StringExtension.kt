package com.example.homex.extension

import java.util.regex.Pattern

val EMAIL_ADDRESS_PATTERN: Pattern = Pattern.compile(
    "([a-zA-Z0-9]([+._%\\-][a-zA-Z0-9])*){1,256}" +
            "@" +
            "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,64}" +
            "(" +
            "\\." +
            "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,25}" +
            ")+"
)

fun CharSequence?.isValidEmail() = !isNullOrEmpty() && EMAIL_ADDRESS_PATTERN.matcher(this).matches()

fun CharSequence.checkUpperLower() : Boolean{
    var ch : Char
    var upper = false
    var lower = false
    for (element in this){
        ch = element
        if(Character.isUpperCase(ch)){
            upper = true
        }else if(Character.isLowerCase(ch)){
            lower = true
        }
        if(upper && lower) return true
    }
    return false
}

fun String.checkPassword(): Boolean {
    var ch: Char
    var capitalFlag = false
    var lowerCaseFlag = false
    for (element in this) {
        ch = element
        if (Character.isUpperCase(ch)) {
            capitalFlag = true
        } else if (Character.isLowerCase(ch)) {
            lowerCaseFlag = true
        }
        if (capitalFlag && lowerCaseFlag) return true
    }
    return false
}