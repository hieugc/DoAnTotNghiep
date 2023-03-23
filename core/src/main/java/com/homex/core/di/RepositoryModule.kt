package com.homex.core.di

import com.homex.core.repository.AuthRepository
import com.homex.core.repository.ChatRepository
import com.homex.core.repository.HomeRepository
import com.homex.core.repository.YourHomeRepository
import com.homex.core.repository.impl.AuthRepositoryImpl
import com.homex.core.repository.impl.ChatRepositoryImpl
import com.homex.core.repository.impl.HomeRepositoryImpl
import com.homex.core.repository.impl.YourHomeRepositoryImpl
import org.koin.dsl.module

val repositoryModule = module {
    single<HomeRepository> { HomeRepositoryImpl(get()) }
    single<AuthRepository> { AuthRepositoryImpl(get()) }
    single<YourHomeRepository> { YourHomeRepositoryImpl(get()) }
    single<ChatRepository> { ChatRepositoryImpl(get()) }
}