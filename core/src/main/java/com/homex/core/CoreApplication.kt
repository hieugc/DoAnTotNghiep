package com.homex.core

import android.app.Application

open class CoreApplication: Application() {

    companion object{
        lateinit var instance:  CoreApplication
            private set
    }


}