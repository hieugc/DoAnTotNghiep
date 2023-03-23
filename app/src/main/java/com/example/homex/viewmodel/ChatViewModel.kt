package com.example.homex.viewmodel

import android.util.Log
import androidx.lifecycle.MediatorLiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.JsonObject
import com.homex.core.model.MessageRoom
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MessageResponse
import com.homex.core.param.chat.ConnectToRoomParam
import com.homex.core.param.chat.GetMessagesParam
import com.homex.core.param.chat.SendMessageParam
import com.homex.core.repository.ChatRepository
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class ChatViewModel(private val repository: ChatRepository): ViewModel() {
    val connectChat = MediatorLiveData<JsonObject?>()
    val connectToUser = MediatorLiveData<JsonObject?>()
    val connectToRoom = MediatorLiveData<JsonObject?>()
    val messages = MediatorLiveData<MessageRoom?>()
    val chatRoom = MediatorLiveData<MessageResponse?>()
    val seenAll = MediatorLiveData<JsonObject?>()
    val sendMessage = MediatorLiveData<MessageRoom?>()
    val newMessage = MutableLiveData<MessageRoom>()

    fun connectAllRoom(body: RequestBody){
        viewModelScope.launch {
            connectChat.addSource(repository.connectChat(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessConnectChat", "${it.data}")
                        connectChat.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessConnectChat", "hello")
                    }
                }
            }
        }
    }

    fun getChatRoom(page: Int){
        viewModelScope.launch {
            chatRoom.addSource(repository.getChatRoom(page)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetChat", "${it.data}")
                        chatRoom.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetChat", "hello")
                    }
                }
            }
        }
    }

    fun getMessagesInChatRoom(param: GetMessagesParam){
        viewModelScope.launch {
            messages.addSource(repository.getMessagesInChatRoom(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessGetMessages", "${it.data}")
                        messages.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessGetMessages", "hello")
                    }
                }
            }
        }
    }

    fun sendMessage(param: SendMessageParam){
        viewModelScope.launch {
            sendMessage.addSource(repository.sendMessage(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessSendMessages", "${it.data}")
                        sendMessage.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessSendMessages", "hello")
                    }
                }
            }
        }
    }

    fun seenAll(body: RequestBody){
        viewModelScope.launch {
            seenAll.addSource(repository.seenAll(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessSeenAll", "${it.data}")
                        seenAll.value = it.data
                    }
                    else -> {
                        Log.e("NotSuccessSeenAll", "hello")
                    }
                }
            }
        }
    }
}