package com.example.homex.app

import com.example.homex.application.appModules
import com.homex.core.CoreApplication
import org.koin.core.module.Module

@Suppress("unused")
class MainApplication : CoreApplication() {

    override fun onCreate() {
        super.onCreate()

//        // Setup Sounds Player
//        SoundUtils.setup(applicationContext)
//
//        // Configure API
//        API.configure(AppConfig.appProdBaseUrl)
//
//        // Configure DataStore
//        DataStore.configure(applicationContext)
//
//        //Configure FCM
//        FirebaseCloudMessaging.setup()
    }

    override fun addModules(): List<Module> {
        return listOf(appModules)
    }
}