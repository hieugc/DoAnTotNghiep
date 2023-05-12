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
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.chat.GetMessagesParam
import com.homex.core.param.chat.SendMessageParam
import com.homex.core.repository.ChatRepository
import com.homex.core.util.AppEvent
import kotlinx.coroutines.launch
import okhttp3.RequestBody

class ChatViewModel(private val repository: ChatRepository): ViewModel() {
    val connectChat = MediatorLiveData<JsonObject?>()
    var connectToUser = MediatorLiveData<MessageRoom?>()
    val messages = MediatorLiveData<MessageRoom?>()
    val chatRoom = MediatorLiveData<MessageResponse?>()
    val seenAll = MediatorLiveData<JsonObject?>()
    val sendMessage = MediatorLiveData<MessageRoom?>()
    val newMessage = MutableLiveData<MessageRoom>()

    fun connectAllRoom(body: RequestBody){
        viewModelScope.launch {
            connectChat.addSource(repository.connectChat(body)){
                when (it) {
                    is ResultResponse.Success -> {
                        connectChat.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun getChatRoom(page: Int){
        viewModelScope.launch {
            chatRoom.addSource(repository.getChatRoom(page)){
                when (it) {
                    is ResultResponse.Success -> {
                        chatRoom.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun getMessagesInChatRoom(param: GetMessagesParam){
        viewModelScope.launch {
            messages.addSource(repository.getMessagesInChatRoom(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        messages.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun sendMessage(param: SendMessageParam){
        viewModelScope.launch {
            sendMessage.addSource(repository.sendMessage(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        sendMessage.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun seenAll(body: RequestBody){
        viewModelScope.launch {
            seenAll.addSource(repository.seenAll(body)){
                when (it) {
                    is ResultResponse.Success -> {
                        seenAll.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.d("Loading", "hello")
                    }
                }
            }
        }
    }

    fun contactToUser(param: ContactUserParam){
        viewModelScope.launch {
            connectToUser.addSource(repository.contactUser(param)){
                when (it) {
                    is ResultResponse.Success -> {
                        connectToUser.value = it.data
                    }
                    is ResultResponse.Error ->{
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