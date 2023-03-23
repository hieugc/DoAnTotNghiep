package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.MessageRoom
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MessageResponse
import com.homex.core.param.chat.ConnectToRoomParam
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.chat.GetMessagesParam
import com.homex.core.param.chat.SendMessageParam
import com.homex.core.repository.ChatRepository
import okhttp3.RequestBody
import retrofit2.Response

class ChatRepositoryImpl(private val api: ApiService): ChatRepository {
    override suspend fun sendMessage(param: SendMessageParam): LiveData<ResultResponse<MessageRoom>> {
        return object : NetworkBoundResource<ObjectResponse<MessageRoom>, MessageRoom>(){
            override fun processResponse(response: ObjectResponse<MessageRoom>): MessageRoom? = response.data
            override suspend fun createCall(): Response<ObjectResponse<MessageRoom>> = api.sendMessage(param)
        }.build().asLiveData()
    }

    override suspend fun contactUser(param: ContactUserParam): LiveData<ResultResponse<MessageRoom>> {
        return object : NetworkBoundResource<ObjectResponse<MessageRoom>, MessageRoom>(){
            override fun processResponse(response: ObjectResponse<MessageRoom>): MessageRoom? = response.data
            override suspend fun createCall(): Response<ObjectResponse<MessageRoom>> = api.contactUser(param)
        }.build().asLiveData()
    }

    override suspend fun connectChat(body: RequestBody): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.connectChat(body)
        }.build().asLiveData()
    }

    override suspend fun connectToRoom(param: ConnectToRoomParam): LiveData<ResultResponse<MessageRoom>> {
        return object : NetworkBoundResource<ObjectResponse<MessageRoom>, MessageRoom>(){
            override fun processResponse(response: ObjectResponse<MessageRoom>): MessageRoom? = response.data
            override suspend fun createCall(): Response<ObjectResponse<MessageRoom>> = api.connectToRoom(param)
        }.build().asLiveData()
    }

    override suspend fun seenAll(body: RequestBody): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.seenAll(body)
        }.build().asLiveData()
    }

    override suspend fun getChatRoom(page: Int): LiveData<ResultResponse<MessageResponse>> {
        return object : NetworkBoundResource<ObjectResponse<MessageResponse>, MessageResponse>(){
            override fun processResponse(response: ObjectResponse<MessageResponse>): MessageResponse? = response.data
            override suspend fun createCall(): Response<ObjectResponse<MessageResponse>> = api.getChatRoom(page)
        }.build().asLiveData()
    }

    override suspend fun getMessagesInChatRoom(param: GetMessagesParam): LiveData<ResultResponse<MessageRoom>> {
        return object : NetworkBoundResource<ObjectResponse<MessageRoom>, MessageRoom>(){
            override fun processResponse(response: ObjectResponse<MessageRoom>): MessageRoom? = response.data
            override suspend fun createCall(): Response<ObjectResponse<MessageRoom>> = api.getMessagesInChatRoom(param)
        }.build().asLiveData()
    }
}