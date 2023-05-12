package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.Notification
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.GetNotificationResponse
import com.homex.core.repository.NotificationRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch

class NotificationViewModel(private val repository: NotificationRepository) : ViewModel() {
    val notificationListLiveDate = MediatorLiveData<GetNotificationResponse?>()
    val notificationLiveData = MediatorLiveData<Notification?>()
    val notificationLiveMessage = MediatorLiveData<JsonObject?>()

    fun getNotifications(page: Int, limit: Int) {
        viewModelScope.launch {
            notificationListLiveDate.addSource(repository.getNotifications(page, limit)) {
                when (it) {
                    is ResultResponse.Success -> {
                        notificationListLiveDate.value = it.data
                    }
                    is ResultResponse.Error -> {
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun updateSeenNotification(id: String) {
        viewModelScope.launch {
            notificationLiveMessage.addSource(repository.updateSeenNotification(id)) {
                when (it) {
                    is ResultResponse.Success -> {
                        notificationLiveMessage.value = it.data
                    }
                    is ResultResponse.Error -> {
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun seenAllNotification() {
        viewModelScope.launch {
            notificationLiveMessage.addSource(repository.seenAllNotification()) {
                when (it) {
                    is ResultResponse.Success -> {
                        notificationLiveMessage.value = it.data
                    }
                    is ResultResponse.Error -> {
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }
}