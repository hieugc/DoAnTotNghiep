package com.example.homex.extension

import android.view.View
import androidx.appcompat.widget.AppCompatButton
import androidx.appcompat.widget.AppCompatTextView
import androidx.core.content.ContextCompat
import com.example.homex.R

fun AppCompatButton.disable(){
    this.background.alpha = 62
    this.isEnabled = false
}

fun AppCompatButton.enable(){
    this.background.alpha = 255
    this.isEnabled = true
}

fun View.visible(){
    if (this.visibility != View.VISIBLE) this.visibility = View.VISIBLE
}

fun View.invisible(){
    if (this.visibility != View.INVISIBLE) this.visibility = View.INVISIBLE
}
fun View.gone(){
    if(this.visibility != View.GONE) this.visibility = View.GONE
}

fun AppCompatTextView.disable(){
    this.isEnabled = false
    this.setTextColor(ContextCompat.getColor(context, R.color.gray))
}
fun AppCompatTextView.enable(){
    this.isEnabled = true
    this.setTextColor(ContextCompat.getColor(context, R.color.secondary))
}