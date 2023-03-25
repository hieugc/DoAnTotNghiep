package com.example.homex.application

import com.example.homex.viewmodel.*
import org.koin.androidx.viewmodel.dsl.viewModel
import org.koin.dsl.module

val appModules = module {
    viewModel { HomeViewModel(get()) }
    viewModel { AuthViewModel(get()) }
    viewModel { YourHomeViewModel(get()) }
    viewModel { ChatViewModel(get()) }
    viewModel { RequestViewModel(get()) }
}