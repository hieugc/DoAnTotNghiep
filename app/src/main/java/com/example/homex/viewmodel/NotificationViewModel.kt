package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.homex.core.model.Notification
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.GetNotificationResponse
import com.homex.core.param.notification.UpdateSeenNotificationParam
import com.homex.core.repository.NotificationRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch

class NotificationViewModel(private val repository: NotificationRepository) : ViewModel() {
    public val notificationListLiveDate = MediatorLiveData<GetNotificationResponse?>()
    public val notificationLiveDate = MediatorLiveData<Notification?>()

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

    fun updateSeenNotification(param: UpdateSeenNotificationParam) {
        viewModelScope.launch {
            notificationListLiveDate.addSource(repository.updateSeenNotification(param)) {
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessSeenNotification", "${it.data}")
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