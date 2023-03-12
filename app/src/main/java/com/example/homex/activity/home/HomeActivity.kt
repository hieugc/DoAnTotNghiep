package com.example.homex.activity.home

import android.R.id.input
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.View
import android.view.WindowManager
import androidx.annotation.MenuRes
import androidx.appcompat.widget.PopupMenu
import androidx.core.content.ContextCompat
import androidx.databinding.DataBindingUtil
import androidx.navigation.NavController
import androidx.navigation.findNavController
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.AppBarConfiguration
import androidx.navigation.ui.NavigationUI.navigateUp
import androidx.navigation.ui.NavigationUI.setupActionBarWithNavController
import androidx.navigation.ui.NavigationUI.setupWithNavController
import com.example.homex.NotificationDialogFragment
import com.example.homex.R
import com.example.homex.base.BaseActivity
import com.example.homex.databinding.ActivityHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.microsoft.signalr.HubConnectionBuilder


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
    private var showBoxChatLayout: Pair<Boolean, String> = Pair(false, "")
    private var showSearchLayout: Boolean = false

    companion object{
        fun open(context: Context) = Intent(context, HomeActivity::class.java)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_home)

        window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS);
        window.statusBarColor = ContextCompat.getColor(this, R.color.white)
        window.navigationBarColor = ContextCompat.getColor(this, R.color.white)
        val navHost =
            supportFragmentManager.findFragmentById(R.id.nav_main_fragment) as NavHostFragment
        navController = navHost.navController
        setupWithNavController(binding.bottomNavigationView, navController)

        appBarConfiguration = AppBarConfiguration.Builder(
            R.id.homeFragment,
            R.id.exploreFragment,
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
        navController.addOnDestinationChangedListener{ _, destination, _->
            supportActionBar?.setHomeAsUpIndicator(R.drawable.ic_back_main)
        }
    }

    fun setPropertiesScreen(showLogo: Boolean, showBottomNav: Boolean, showTitleApp: Pair<Boolean, String>, showMessage: Boolean, showMenu: Boolean, showBoxChatLayout: Pair<Boolean, String>, showSearchLayout: Boolean = false){
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

        if(showMsg)
            binding.btnMessage.visible()
        else
            binding.btnMessage.gone()

        if (showMenu)
            binding.btnMenu.visible()
        else
            binding.btnMenu.gone()

        if (showBoxChatLayout.first){
            binding.boxChatName.text = showBoxChatLayout.second
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

    private fun setViewModel(){}
    private fun setEvent(){
        binding.btnMessage.setOnClickListener {
            findNavController(R.id.nav_main_fragment).navigate(R.id.action_global_messageFragment)
        }
        binding.btnMenu.setOnClickListener {
            showMenu(it, R.menu.box_chat_menu)
        }
        binding.btnFilter.setOnClickListener {
            findNavController(R.id.nav_main_fragment).navigate(R.id.action_searchResultFragment_to_filterBottomSheetFragment)
        }
    }

    override fun onSupportNavigateUp(): Boolean {
        return navigateUp(navController, appBarConfiguration)
    }

    fun showSearchLayout(){
        binding.searchLayout.visible()
        binding.btnFilter.visible()
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
}