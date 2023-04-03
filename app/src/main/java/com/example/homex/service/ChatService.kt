package com.example.homex.service

import android.app.Service
import android.content.Intent
import android.os.Binder
import android.os.IBinder
import android.util.Log
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import com.example.homex.app.CHAT_HUB_URL
import com.example.homex.app.CONNECT_CHAT
import com.example.homex.app.RECEIVE_MESSAGE
import com.homex.core.CoreApplication
import com.homex.core.model.MessageRoom
import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import io.reactivex.rxjava3.core.CompletableObserver
import io.reactivex.rxjava3.disposables.Disposable
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.launch
import java.util.concurrent.ExecutionException
import java.util.concurrent.TimeUnit


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

    /**
     * method for clients (activities)
     */

    fun sendMessage(message: String){
        hubConnection.invoke("Send", message)
    }

    private fun sendMessageToActivity(messageRoom: MessageRoom){
        scope.launch(Dispatchers.IO){
            val intent = Intent(TAG)
            intent.putExtra(RECEIVE_MESSAGE, messageRoom)
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
        Log.e("token", "$token")
        if (token != null) {
            hubConnection = HubConnectionBuilder.create(CHAT_HUB_URL)
//                .setHttpClientBuilderCallback {
//                    it.writeTimeout(15 * 60 * 1000, TimeUnit.MILLISECONDS)
//                    it.readTimeout(60 * 1000, TimeUnit.MILLISECONDS)
//                    it.connectTimeout(20 * 1000, TimeUnit.MILLISECONDS)
//                }
                .withHeader("Authorization", "Bearer $token")
                .build()
        }else{
            hubConnection = HubConnectionBuilder.create(CHAT_HUB_URL)
//                .setHttpClientBuilderCallback {
//                    it.writeTimeout(15 * 60 * 1000, TimeUnit.MILLISECONDS)
//                    it.readTimeout(60 * 1000, TimeUnit.MILLISECONDS)
//                    it.connectTimeout(20 * 1000, TimeUnit.MILLISECONDS)
//                }
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

        hubConnection.onClosed {
            if (it != null){
                Log.e(TAG, "${it.message}. Reconnecting...")
                try {
                    Thread{
                        kotlin.run {
                            hubConnection.start().cache()
                                .doOnComplete {
                                    Log.e("complete", "hello")
                                    connectToChat()
                                }
                                .doOnError {
                                    Log.e("error", "${it.message}")
                                }
                                .blockingAwait()
                        }
                    }.start()
                }catch (e: InterruptedException){
                    e.printStackTrace()
                    return@onClosed
                }catch (e: ExecutionException){
                    e.printStackTrace()
                    return@onClosed
                }
            }
        }


        try {
            Thread{
                kotlin.run {
                    hubConnection.start().cache()
                        .doOnComplete {
                            Log.e("complete", "hello")
                            connectToChat()
                        }
                        .doOnError {
                            Log.e("error", "${it.message}")
                        }
                        .blockingAwait()
                }
            }.start()
        }catch (e: InterruptedException){
            e.printStackTrace()
            return
        }catch (e: ExecutionException){
            e.printStackTrace()
            return
        }
    }
}

