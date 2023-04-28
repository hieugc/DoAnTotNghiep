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
    val connectToRoom = MediatorLiveData<JsonObject?>()
    val messages = MediatorLiveData<MessageRoom?>()
    val chatRoom = MediatorLiveData<MessageResponse?>()
    val seenAll = MediatorLiveData<JsonObject?>()
    val sendMessage = MediatorLiveData<MessageRoom?>()
    val newMessage = MutableLiveData<MessageRoom>()
    val connectionId = MutableLiveData<String>()

    fun connectAllRoom(body: RequestBody){
        viewModelScope.launch {
            connectChat.addSource(repository.connectChat(body)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessConnectChat", "${it.data}")
                        connectChat.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
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
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
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
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
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
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
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
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }

    fun clearContactUser(){
        connectToUser = MediatorLiveData<MessageRoom?>()
    }

    fun contactToUser(param: ContactUserParam){
        viewModelScope.launch {
            connectToUser.addSource(repository.contactUser(param)){
                Log.e("response", it.toString())
                when (it) {
                    is ResultResponse.Success -> {
                        Log.e("SuccessContactUser", "${it.data}")
                        connectToUser.value = it.data
                    }
                    is ResultResponse.Error ->{
                        AppEvent.showPopUpError(it.message)
                    }
                    else -> {
                        Log.e("Loading", "hello")
                    }
                }
            }
        }
    }
}