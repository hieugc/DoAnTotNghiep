package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.Notification
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.GetNotificationResponse
import com.homex.core.param.notification.UpdateSeenNotificationParam
import com.homex.core.repository.NotificationRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch

class NotificationViewModel(private val repository: NotificationRepository) : ViewModel() {
    public val notificationListLiveDate = MediatorLiveData<GetNotificationResponse?>()
    public val notificationLiveData = MediatorLiveData<Notification?>()
    public val notificationLiveMessage = MediatorLiveData<JsonObject?>()

    fun getNotifications(page: Int, limit: Int) {
        viewModelScope.launch {
            notificationListLiveDate.addSource(repository.getNotifications(page, limit)) {
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetNotification", "${it.data}")
                        notificationListLiveDate.value = it.data
                    }
                    is ResultResponse.Error -> {
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                    }
                }
            }
        }
    }

    fun updateSeenNotification(id: String) {
        viewModelScope.launch {
            notificationLiveMessage.addSource(repository.updateSeenNotification(id)) {
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        notificationLiveMessage.value = it.data
                    }
                    is ResultResponse.Error -> {
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                    }
                }
            }
        }
    }

    fun seenAllNotification() {
        viewModelScope.launch {
            notificationLiveMessage.addSource(repository.seenAllNotification()) {
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        notificationLiveMessage.value = it.data
                    }
                    is ResultResponse.Error -> {
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                    }
                }
            }
        }
    }
}