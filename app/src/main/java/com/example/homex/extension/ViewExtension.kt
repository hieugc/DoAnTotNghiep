package com.example.homex.extension

import android.view.View
import androidx.appcompat.widget.AppCompatButton

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

fun View.gone(){
    if(this.visibility != View.GONE) this.visibility = View.GONE
}