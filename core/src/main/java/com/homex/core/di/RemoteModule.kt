package com.homex.core.di

import com.facebook.stetho.okhttp3.StethoInterceptor
import com.homex.core.BuildConfig
import com.homex.core.CoreApplication
import com.homex.core.api.ApiService
import okhttp3.Interceptor
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import org.koin.dsl.module
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

val remoteModule = module {

    single {
        createService<ApiService>(get())
    }

    single {
        createOkHttpClient()
    }
}

fun createOkHttpClient(): OkHttpClient {
    val httpLoggingInterceptor = HttpLoggingInterceptor()
    httpLoggingInterceptor.level = HttpLoggingInterceptor.Level.BODY
    return OkHttpClient.Builder()
        .writeTimeout(15 * 60 * 1000, TimeUnit.MILLISECONDS)
        .readTimeout(60 * 1000, TimeUnit.MILLISECONDS)
        .connectTimeout(20 * 1000, TimeUnit.MILLISECONDS)
//        .addInterceptor(NoInternetInterceptor(CoreApplication.instance))
        .addNetworkInterceptor(Interceptor { chain ->
            var request = chain.request()
            val builder = request.newBuilder()
            val token = CoreApplication.instance.getToken()
            if (token != null) {
                builder.header("Authorization", "Bearer $token")
            }
            request = builder.build()
            chain.proceed(request)
        })
        .addInterceptor(httpLoggingInterceptor)
        .addNetworkInterceptor(StethoInterceptor())
        .build()
}


inline fun <reified T> createService(okHttpClient: OkHttpClient): T {
    val retrofit = Retrofit.Builder()
        .baseUrl(BuildConfig.SERVER_URL)
        .client(okHttpClient)
        .addConverterFactory(GsonConverterFactory.create())
        .build()
    return retrofit.create(T::class.java)
}
