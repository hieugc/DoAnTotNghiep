package com.example.homex.activity.home

import android.content.BroadcastReceiver
import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.content.ServiceConnection
import android.graphics.Color
import android.os.Bundle
import android.os.IBinder
import android.util.Log
import android.view.Gravity
import android.view.View
import android.view.WindowManager
import android.widget.FrameLayout
import android.widget.TextView
import androidx.activity.viewModels
import androidx.annotation.MenuRes
import androidx.appcompat.widget.PopupMenu
import androidx.core.content.ContextCompat
import androidx.databinding.DataBindingUtil
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import androidx.navigation.NavController
import androidx.navigation.findNavController
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.AppBarConfiguration
import androidx.navigation.ui.NavigationUI.navigateUp
import androidx.navigation.ui.NavigationUI.setupActionBarWithNavController
import androidx.navigation.ui.NavigationUI.setupWithNavController
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.activity.home.addhome.FileViewModel
import com.example.homex.activity.home.search.SearchResultFragmentDirections
import com.example.homex.app.CONNECT_CHAT
import com.example.homex.app.NOTIFICATIONS
import com.example.homex.app.RECEIVE_MESSAGE
import com.example.homex.base.BaseActivity
import com.example.homex.databinding.ActivityHomeBinding
import com.example.homex.extension.NotificationType
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.service.ChatService
import com.example.homex.viewmodel.ChatViewModel
import com.example.homex.viewmodel.NotificationViewModel
import com.example.homex.viewmodel.ProfileViewModel
import com.google.android.material.snackbar.Snackbar
import com.homex.core.CoreApplication
import com.homex.core.model.Filter
import com.homex.core.model.MessageRoom
import com.homex.core.model.Notification
import com.homex.core.model.UserMessage
import com.homex.core.util.PrefUtil
import com.microsoft.signalr.HubConnectionState
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody
import okhttp3.RequestBody.Companion.toRequestBody
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import vn.zalopay.sdk.Environment
import vn.zalopay.sdk.ZaloPaySDK
import java.io.File


class HomeActivity : BaseActivity() {
    private lateinit var binding: ActivityHomeBinding
    private lateinit var navController: NavController
    private lateinit var appBarConfiguration: AppBarConfiguration
    private lateinit var readAllNotificationDialogFragment: NotificationDialogFragment
    private var showLogo = true
    private var showBottomNav = true
    private var showTitleApp : Pair<Boolean, String> = Pair(false, "")
    private var showMenu = false
    private var showMsg = true
    private var showBoxChatLayout: Pair<Boolean, UserMessage?> = Pair(false, null)
    private var showSearchLayout: Boolean = false
    private val fileViewModel: FileViewModel by viewModels()
    private val tmpFiles = mutableListOf<File>()
    private var filter: Filter? = null
    private val prefUtil: PrefUtil by inject()
    private val mContext: Context = this
    private var mService: ChatService? = null
    private var mBound = false
    private val chatViewModel: ChatViewModel by viewModel()
    private val profileViewModel: ProfileViewModel by viewModel()
    private val notificationViewModel: NotificationViewModel by viewModel()

    companion object{
        fun open(context: Context) = Intent(context, HomeActivity::class.java)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_home)

        window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        window.statusBarColor = ContextCompat.getColor(this, R.color.white)
        window.navigationBarColor = ContextCompat.getColor(this, R.color.white)
        val navHost =
            supportFragmentManager.findFragmentById(R.id.nav_main_fragment) as NavHostFragment
        navController = navHost.navController
        setupWithNavController(binding.bottomNavigationView, navController)

        appBarConfiguration = AppBarConfiguration.Builder(
            R.id.homeFragment,
//            R.id.exploreFragment,
            R.id.notificationFragment,
            R.id.userFragment
        ).build()

        setSupportActionBar(binding.toolbarHome)

        setupActionBarWithNavController(
            this,
            navController,
            appBarConfiguration
        )
        supportActionBar?.setDisplayShowTitleEnabled(false)

        readAllNotificationDialogFragment = NotificationDialogFragment()

        setViewModel()
        setEvent()
        navController.addOnDestinationChangedListener{ _, _, _->
            supportActionBar?.setHomeAsUpIndicator(R.drawable.ic_back_main)
        }
        ZaloPaySDK.init(554, Environment.SANDBOX)
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)
        ZaloPaySDK.getInstance().onResult(intent)
    }

    fun setPropertiesScreen(showLogo: Boolean, showBottomNav: Boolean, showTitleApp: Pair<Boolean, String>, showMessage: Boolean, showMenu: Boolean, showBoxChatLayout: Pair<Boolean, UserMessage?>, showSearchLayout: Boolean = false){
        this.showLogo = showLogo
        this.showBottomNav = showBottomNav
        this.showTitleApp = showTitleApp
        this.showMsg = showMessage
        this.showMenu = showMenu
        this.showBoxChatLayout = showBoxChatLayout
        this.showSearchLayout = showSearchLayout
        checkShowUI()
    }

    private fun checkShowUI(){
        if (showLogo)
            binding.ivLogo.visible()
        else
            binding.ivLogo.gone()

        if (showBottomNav)
            binding.bottomNavigationView.visible()
        else
            binding.bottomNavigationView.gone()

        if (showTitleApp.first){
            binding.toolbarTitle.text = showTitleApp.second
            binding.toolbarTitle.visible()
        }else{
            binding.toolbarTitle.gone()
        }

        if(showMsg && prefUtil.token != null)
            binding.btnMessage.visible()
        else
            binding.btnMessage.gone()

        if (showMenu)
            binding.btnMenu.visible()
        else
            binding.btnMenu.gone()

        if (showBoxChatLayout.first){
            binding.boxChatName.text = showBoxChatLayout.second?.userName
            Glide.with(this)
                .load(showBoxChatLayout.second?.imageUrl)
                .error(R.mipmap.avatar)
                .into(binding.ivAvatar)
            binding.userChatLayout.visible()
        }
        else
            binding.userChatLayout.gone()

        if(showSearchLayout) {
            binding.searchLayout.visible()
            binding.btnFilter.visible()
        }
        else {
            binding.searchLayout.gone()
            binding.btnFilter.gone()
        }
    }

    fun setSearchParam(location: String?, startDate: String, endDate: String, people: Int){
        binding.locationTV.text = location
        binding.dateTV.text = getString(R.string.search_param, startDate, endDate, people)
    }

    private fun setViewModel(){
        fileViewModel.tmpFiles.observe(this){
            if(it != null){
                tmpFiles.clear()
                tmpFiles.addAll(it)
            }
        }
        chatViewModel.connectChat.observe(this){
            if(it != null){
                Log.i("ConnectAllRoom", "Connect success")
            }
        }
    }
    private fun setEvent(){
        binding.btnMessage.setOnClickListener {
            findNavController(R.id.nav_main_fragment).navigate(R.id.action_global_messageFragment)
        }
        binding.btnMenu.setOnClickListener {
            showMenu(it, R.menu.box_chat_menu)
        }
        binding.btnFilter.setOnClickListener {
            val action = SearchResultFragmentDirections.actionSearchResultFragmentToFilterBottomSheetFragment(filter)
            findNavController(R.id.nav_main_fragment).navigate(action)
        }
    }

    fun setFilter(filter: Filter?){
        this.filter = filter
    }

    override fun onSupportNavigateUp(): Boolean {
        return navigateUp(navController, appBarConfiguration)
    }


    fun showReadAllNotificationDialog(){
        val supportFragmentManager = supportFragmentManager
        readAllNotificationDialogFragment.show(supportFragmentManager, "read_all_notification")
    }

    private fun showMenu(v: View, @MenuRes menuRes: Int) {
        val popup = PopupMenu(this, v)
        popup.menuInflater.inflate(menuRes, popup.menu)

        popup.setOnDismissListener {
            // Respond to popup being dismissed.
        }
        // Show the popup menu.
        popup.show()
    }

    override fun onDestroy() {
        for(item in tmpFiles){
            item.delete()
        }
        super.onDestroy()
    }

    override fun onStart() {
        super.onStart()
        //Start ChatService
        if (prefUtil.token != null){
            val intent = Intent()
            intent.setClass(mContext, ChatService::class.java)
            bindService(intent, mConnection, Context.BIND_AUTO_CREATE)
        }
    }

    override fun onStop() {
        super.onStop()
        // Unbind from the service
        if (mBound) {
            unbindService(mConnection)
            mBound = false
        }
    }

    override fun onResume() {
        super.onResume()
        // Register for the particular broadcast based on ACTION string
        val filter = IntentFilter(ChatService.TAG)
        LocalBroadcastManager.getInstance(this).registerReceiver(myReceiver, filter)
        // or `registerReceiver(testReceiver, filter)` for a normal broadcast
    }

    override fun onPause() {
        super.onPause()
        // Unregister the listener when the application is paused
        LocalBroadcastManager.getInstance(this).unregisterReceiver(myReceiver)
        // or `unregisterReceiver(testReceiver)` for a normal broadcast
    }

    /**
     * Defines callbacks for service binding, passed to bindService()
     */
    private val mConnection : ServiceConnection =  object: ServiceConnection {
        override fun onServiceConnected(p0: ComponentName?, p1: IBinder?) {
            // We've bound to ChatService, cast the IBinder and get ChatService instance
            val binder = p1 as ChatService.LocalBinder
            mService = binder.service
            mBound = true
            binder.service.hubConnection.connectionState?.let{ state ->
                if (state == HubConnectionState.CONNECTED){
                    binder.service.hubConnection.connectionId?.let {
                        val mediaType = "application/json".toMediaType()
                        val body: RequestBody = "\"$it\"".toRequestBody(mediaType)
                        CoreApplication.instance.saveConnectionId(it)
                        chatViewModel.connectAllRoom(body)
                    }
                }
            }
        }

        override fun onServiceDisconnected(p0: ComponentName?) {
            mBound = false
        }
    }

    private val myReceiver =  object:  BroadcastReceiver(){
        override fun onReceive(p0: Context?, p1: Intent?) {
            val message = p1?.getParcelableExtra<MessageRoom>(RECEIVE_MESSAGE)
            if (message != null){
                chatViewModel.newMessage.postValue(message)
            }
            val connect = p1?.getBooleanExtra(CONNECT_CHAT, false)
            if (connect == true){
                mService?.apply {
                    hubConnection.connectionState?.let{ state ->
                        if (state == HubConnectionState.CONNECTED){
                            hubConnection.connectionId?.let {
                                val mediaType = "application/json".toMediaType()
                                val body: RequestBody = "\"$it\"".toRequestBody(mediaType)
                                CoreApplication.instance.saveConnectionId(it)
                                chatViewModel.connectAllRoom(body)
                            }
                        }
                    }
                }
            }
            val notification = p1?.getParcelableExtra<Notification>(NOTIFICATIONS)
            handleNotification(notification)
            if (notification?.type == NotificationType.PAYMENT.ordinal){
                profileViewModel.getPoint()
                profileViewModel.getHistoryAll()
                profileViewModel.getHistoryReceived()
            }
        }
    }

    private fun handleNotification(notification: Notification?) {
        if(notification != null){
            notificationViewModel.notificationLiveData.postValue(notification)

            val snackbar: Snackbar = Snackbar.make(findViewById(R.id.main_view), "", Snackbar.LENGTH_LONG)

            val snackbarLayout: Snackbar.SnackbarLayout =
                snackbar.view as Snackbar.SnackbarLayout

            val customSnackView: View = layoutInflater.inflate(R.layout.layout_snackbar_notification, snackbarLayout)
            val tvTitle = customSnackView.findViewById<TextView>(R.id.tvTitle)
            val tvContent = customSnackView.findViewById<TextView>(R.id.tvContent)
            tvTitle.text = notification.title
            tvContent.text = notification.content

            snackbar.view.layoutParams = (snackbar.view.layoutParams as FrameLayout.LayoutParams).apply {
                gravity = Gravity.TOP
            }
            snackbar.view.setBackgroundColor(Color.TRANSPARENT)

            snackbarLayout.setPadding(0, 0, 0, 0)

            snackbar.show()
        }
    }
}