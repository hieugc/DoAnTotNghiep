package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.model.MessageRoom
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MessageResponse
import com.homex.core.param.chat.ConnectToRoomParam
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.chat.GetMessagesParam
import com.homex.core.param.chat.SendMessageParam
import okhttp3.RequestBody
import retrofit2.Response

interface ChatRepository {

    suspend fun sendMessage(
        param: SendMessageParam
    ): LiveData<ResultResponse<MessageRoom>>

    suspend fun contactUser(
        param: ContactUserParam
    ): LiveData<ResultResponse<MessageRoom>>

    suspend fun connectChat(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>>

    suspend fun connectToRoom(
        param: ConnectToRoomParam
    ): LiveData<ResultResponse<MessageRoom>>

    suspend fun seenAll(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>>

    suspend fun getChatRoom(page: Int): LiveData<ResultResponse<MessageResponse>>

    suspend fun getMessagesInChatRoom(param: GetMessagesParam): LiveData<ResultResponse<MessageRoom>>
}