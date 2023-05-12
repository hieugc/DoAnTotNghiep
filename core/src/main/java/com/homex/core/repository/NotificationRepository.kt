package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.GetNotificationResponse

interface NotificationRepository {
    suspend fun getNotifications(
        page: Int,
        limit: Int
    ): LiveData<ResultResponse<GetNotificationResponse>>

    suspend fun updateSeenNotification(id: String): LiveData<ResultResponse<JsonObject>>

    suspend fun seenAllNotification(): LiveData<ResultResponse<JsonObject>>
}