package com.homex.core

import android.app.Application
import com.homex.core.di.localModule
import com.homex.core.di.remoteModule
import com.homex.core.di.repositoryModule
import com.homex.core.model.LocationSuggestion
import com.homex.core.model.Profile
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.android.ext.koin.androidContext
import org.koin.android.ext.koin.androidLogger
import org.koin.core.context.startKoin
import org.koin.core.module.Module

open class CoreApplication: Application() {

    companion object{
        lateinit var instance:  CoreApplication
            private set
    }
    private val prefsUtil: PrefUtil by inject()
    private var profile: Profile? = null
    private var listSearch: List<LocationSuggestion>? = listOf()
    private var token: String? = null
    private var connectionId: String? = null


    override fun onCreate() {
        super.onCreate()
        startKoin {
            androidLogger()
            androidContext(this@CoreApplication)
            modules(getModule())
        }

        instance = this
    }


    private fun getModule(): List<Module> {
        val moduleList = arrayListOf<Module>()
        moduleList.addAll(listOf(localModule, remoteModule, repositoryModule))
        moduleList.addAll(addModules())
        return moduleList
    }

    open fun addModules(): List<Module> = emptyList()

    fun saveProfile(profile: Profile?) {
        prefsUtil.profile = profile
        this.profile = profile
    }
    fun saveToken(token: String?) {
        prefsUtil.token = token
        this.token = token
    }

    fun saveConnectionId(connectionId: String?){
        prefsUtil.connectionId = connectionId
        this.connectionId = connectionId
    }

    fun saveListSearch(list: List<LocationSuggestion>?){
        prefsUtil.listSearch = list
        this.listSearch = list
    }


    fun getProfile(): Profile? = profile
    fun getToken(): String? = token
    fun getConnectionId(): String? = connectionId
    fun getListSearch(): List<LocationSuggestion>? = listSearch

    fun clearData(){
        prefsUtil.clearAllData()
        token = null
        profile = null
        connectionId = null
        listSearch = null
    }
}