package com.homex.core

import android.util.Log
import com.homex.core.api.ApiService
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import okhttp3.Authenticator
import okhttp3.Request
import okhttp3.Response
import okhttp3.Route

class AccessTokenInterceptor constructor(
    private val prefUtil: PrefUtil,
    private val apiService: ApiService
) : Authenticator {

    override fun authenticate(route: Route?, response: Response): Request? {
        return when (response.code) {
            401 -> {
    //                val token = prefUtil.token
    //                var accessToken: String? = null
    //                val call = apiService.refreshToken(token?.token ?: "", token?.refresh_token ?: "")

                prefUtil.clearAllData()
    //                try {
    //                    val res = call.execute()
    //                    if (res.isSuccessful) {
    //                        val data = res.body()?.data
    //                        Log.i("NewToken", data.toString())
    //                        data?.let {
    //                            accessToken = it.token
    //                            BaseApplication.instance.setToken(it)
    //                        }
    //                    } else {
                CoreApplication.instance.clearData()
                AppEvent.onLogout()
    //                    }
    //                } catch (e: Exception) {
    //                    Log.d("Refresh error", "onResponse: ${e.printStackTrace()}")
    //                }
    //                return if (accessToken != null)
    //                    response.request().newBuilder()
    //                            .header("X-Http-Token", accessToken!!)
    //                            .build()
    //                else null
                null
            }
            else -> null
        }
    }
}