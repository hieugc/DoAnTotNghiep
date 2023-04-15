package com.homex.core.di

import com.homex.core.repository.*
import com.homex.core.repository.impl.*
import org.koin.dsl.module
import kotlin.math.sin

val repositoryModule = module {
    single<HomeRepository> { HomeRepositoryImpl(get()) }
    single<AuthRepository> { AuthRepositoryImpl(get()) }
    single<YourHomeRepository> { YourHomeRepositoryImpl(get()) }
    single<ChatRepository> { ChatRepositoryImpl(get()) }
    single<RequestRepository> { RequestRepositoryImpl(get()) }
    single<NotificationRepository> { NotificationRepositoryImpl(get()) }
}