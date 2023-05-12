package com.homex.core.di

import com.homex.core.repository.AuthRepository
import com.homex.core.repository.ChatRepository
import com.homex.core.repository.HomeRepository
import com.homex.core.repository.NotificationRepository
import com.homex.core.repository.ProfileRepository
import com.homex.core.repository.RequestRepository
import com.homex.core.repository.YourHomeRepository
import com.homex.core.repository.impl.AuthRepositoryImpl
import com.homex.core.repository.impl.ChatRepositoryImpl
import com.homex.core.repository.impl.HomeRepositoryImpl
import com.homex.core.repository.impl.NotificationRepositoryImpl
import com.homex.core.repository.impl.ProfileRepositoryImpl
import com.homex.core.repository.impl.RequestRepositoryImpl
import com.homex.core.repository.impl.YourHomeRepositoryImpl
import org.koin.dsl.module

val repositoryModule = module {
    single<HomeRepository> { HomeRepositoryImpl(get()) }
    single<AuthRepository> { AuthRepositoryImpl(get()) }
    single<YourHomeRepository> { YourHomeRepositoryImpl(get()) }
    single<ChatRepository> { ChatRepositoryImpl(get()) }
    single<RequestRepository> { RequestRepositoryImpl(get()) }
    single<NotificationRepository> { NotificationRepositoryImpl(get()) }
    single<ProfileRepository> { ProfileRepositoryImpl(get()) }
}