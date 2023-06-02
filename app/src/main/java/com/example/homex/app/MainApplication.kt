package com.example.homex.app

import com.example.homex.application.appModules
import com.homex.core.CoreApplication
import org.koin.core.module.Module

class MainApplication : CoreApplication() {

    override fun addModules(): List<Module> {
        return listOf(appModules)
    }
}