package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.GetNotificationResponse
import com.homex.core.repository.NotificationRepository
import retrofit2.Response

class NotificationRepositoryImpl(private val api: ApiService) : NotificationRepository {
    override suspend fun getNotifications(
        page: Int,
        limit: Int
    ): LiveData<ResultResponse<GetNotificationResponse>> {
        return object :
            NetworkBoundResource<ObjectResponse<GetNotificationResponse>, GetNotificationResponse>() {
            override fun processResponse(response: ObjectResponse<GetNotificationResponse>): GetNotificationResponse? =
                response.data

            override suspend fun createCall(): Response<ObjectResponse<GetNotificationResponse>> =
                api.getNotifications(page, limit)
        }.build().asLiveData()
    }

    override suspend fun updateSeenNotification(id: String): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>() {
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? =
                response.data

            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> =
                api.updateSeenNotification(id)
        }.build().asLiveData()
    }

    override suspend fun seenAllNotification(): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>() {
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? =
                response.data

            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> =
                api.seenAllNotification()
        }.build().asLiveData()
    }
}