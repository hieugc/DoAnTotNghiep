package com.example.homex.service

import android.app.Service
import android.content.Intent
import android.os.Binder
import android.os.IBinder
import android.util.Log
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import com.example.homex.app.CHAT_HUB_URL
import com.example.homex.app.CONNECT_CHAT
import com.example.homex.app.NOTIFICATIONS
import com.example.homex.app.RECEIVE_MESSAGE
import com.homex.core.CoreApplication
import com.homex.core.model.MessageRoom
import com.homex.core.model.Notification
import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking
import okhttp3.logging.HttpLoggingInterceptor
import java.util.concurrent.ExecutionException


class ChatService: Service() {

    lateinit var hubConnection: HubConnection
    private val mBinder: IBinder = LocalBinder()
    private lateinit var localBroadcastManager : LocalBroadcastManager
    private var job: Job = Job()
    private val scope = CoroutineScope(job + Dispatchers.Main)

    companion object {
        const val TAG = "ChatService"
    }

    override fun onBind(p0: Intent?): IBinder {
        Log.d( TAG, "Service is bound")
        // Return the communication channel to the service.
        startSignalR()
        return mBinder
    }

    override fun onCreate() {
        super.onCreate()
        // Fires when a service is first initialized
        localBroadcastManager =  LocalBroadcastManager.getInstance(this)
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        // Fires when a service is started up, do work here!
        // ...
        val result = super.onStartCommand(intent, flags, startId)
        startSignalR()
        // Return "sticky" for services that are explicitly
        // started and stopped as needed by the app.
        return result
    }

    override fun onDestroy() {
        // Cleanup service before destruction
        hubConnection.stop()
        super.onDestroy()
    }

    /**
     * Class used for the client Binder.  Because we know this service always
     * runs in the same process as its clients, we don't need to deal with IPC.
     */
    inner class LocalBinder : Binder() {
        // Return this instance of SignalRService so clients can call public methods
        val service: ChatService
            get() =// Return this instance of SignalRService so clients can call public methods
                this@ChatService
    }

    private fun sendMessageToActivity(messageRoom: MessageRoom){
        scope.launch(Dispatchers.IO){
            val intent = Intent(TAG)
            intent.putExtra(RECEIVE_MESSAGE, messageRoom)
            localBroadcastManager.sendBroadcast(intent)
        }
    }

    private fun pushNotificationActivity(notification: Notification){
        scope.launch(Dispatchers.IO){
            val intent = Intent(TAG)
            intent.putExtra(NOTIFICATIONS, notification)
            localBroadcastManager.sendBroadcast(intent)
        }
    }
    private fun connectToChat(){
        scope.launch(Dispatchers.IO){
            val intent = Intent(TAG)
            intent.putExtra(CONNECT_CHAT, true)
            localBroadcastManager.sendBroadcast(intent)
        }
    }

    private fun startSignalR() {
        val token = CoreApplication.instance.getToken()
        val httpLoggingInterceptor = HttpLoggingInterceptor()
        httpLoggingInterceptor.level = HttpLoggingInterceptor.Level.BODY
        hubConnection = if (token != null) {
            HubConnectionBuilder.create(CHAT_HUB_URL)
                .withHeader("Authorization", "Bearer $token")
                .build()
        }else{
            HubConnectionBuilder.create(CHAT_HUB_URL)
                .build()
        }


        hubConnection.on(
            RECEIVE_MESSAGE,
            {
                    message: MessageRoom ->
                Log.d("New Message:", "${message.messages}")
                sendMessageToActivity(message)
            },
            MessageRoom::class.java
        )


        hubConnection.on(
            NOTIFICATIONS,
            {
                    notification: Notification ->
                Log.d("New notification:", "${notification.title}")
                pushNotificationActivity(notification)
            },
            Notification::class.java
        )

        hubConnection.onClosed {
            if (it != null){
                Log.d(TAG, "${it.message}. Reconnecting...")
                runBlocking {
                    try {
                        hubConnection.start()
                            .doOnComplete {
                                connectToChat()
                            }
                            .doOnError { throwable->
                                throw throwable
                            }
                            .blockingAwait()
                    }catch (e: InterruptedException){
                        e.printStackTrace()
                    }catch (e: ExecutionException){
                        e.printStackTrace()
                    }catch (e: Exception){
                        e.printStackTrace()
                    }
                }
            }
        }


        runBlocking {
            try {
                hubConnection.start()
                    .doOnComplete {
                        connectToChat()
                    }
                    .doOnError {
                        throw it
                    }
                    .blockingAwait()
            }catch (e: InterruptedException){
                e.printStackTrace()
            }catch (e: ExecutionException){
                e.printStackTrace()
            }
            catch (e: Exception){
                e.printStackTrace()
            }
        }
    }
}

