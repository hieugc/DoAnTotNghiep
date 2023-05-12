package com.example.homex.activity

import android.os.Bundle
import androidx.databinding.DataBindingUtil
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseActivity
import com.example.homex.databinding.ActivitySplashBinding
import com.homex.core.CoreApplication
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import java.util.Timer
import kotlin.concurrent.schedule

class SplashActivity : BaseActivity() {
    private lateinit var binding: ActivitySplashBinding
    private val prefUtil: PrefUtil by inject()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(
            this,
            R.layout.activity_splash
        )

        binding.lifecycleOwner = this
        Timer().schedule(1000) {
            if(prefUtil.profile != null){
                CoreApplication.instance.saveToken(prefUtil.token)
                CoreApplication.instance.saveProfile(prefUtil.profile)
            }
            val myIntent = HomeActivity.open(this@SplashActivity)
            if(intent.extras?.getBoolean("fromNotification") == true)
                myIntent.putExtra("fromNotification", true)
            startActivity(myIntent)
            finish()
        }
    }
    override fun onBackPressed() {
        if (supportFragmentManager.backStackEntryCount > 0) {
            supportFragmentManager.popBackStack()
            return
        }

        super.onBackPressed()
    }

}