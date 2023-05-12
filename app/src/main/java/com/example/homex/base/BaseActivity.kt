package com.example.homex.base

import android.content.Intent
import android.graphics.Color
import android.os.Bundle
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.utils.PopupDialogFragment
import com.example.homex.utils.PopupHelper
import com.google.android.material.snackbar.Snackbar
import com.homex.core.CoreApplication
import com.homex.core.model.PopUp
import com.homex.core.util.AppEvent
import com.homex.core.util.AuthenticationListener
import com.homex.core.util.PopupEventListener
import pub.devrel.easypermissions.EasyPermissions

open class BaseActivity: PopupEventListener, AuthenticationListener , AppCompatActivity() {
    companion object{
        var instance = BaseActivity()
    }

    private var listDialogFragment: ArrayList<PopupDialogFragment?>? = arrayListOf()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        instance = this
        AppEvent.registerListener(this)
        AppEvent.registerAuthListener(this)
    }

    override fun onBackPressed() {
        moveTaskToBack(true)
        AppEvent.closePopup()
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)

        EasyPermissions.onRequestPermissionsResult(requestCode, permissions, grantResults)
    }

    override fun onResume() {
        super.onResume()
        AppEvent.registerListener(this)
        AppEvent.registerAuthListener(this)
    }

    override fun onStop() {
        super.onStop()
        AppEvent.unRegisterListener(this)
        AppEvent.unRegisterAuthListener(this)
    }

    override fun onDestroy() {
        super.onDestroy()
        AppEvent.unRegisterListener(this)
        AppEvent.unRegisterAuthListener(this)
    }

    override fun onClosePopup() {
        closePopup()
    }

    override fun onShowPopUpError(message: String?) {
        PopupHelper().showErrorPopup(message)
    }

    override fun onShowPopUp(popup: PopUp?) {
        showPopup(popup)
    }

    private fun showPopup(popup: PopUp?){
        closePopup()
        val dialog = PopupDialogFragment.newInstance(popup)
        dialog.show(supportFragmentManager, PopupDialogFragment().tag)
        listDialogFragment?.add(dialog)
    }

    private fun closePopup(){
        for(item in listDialogFragment?: arrayListOf()){
            item?.dismissAllowingStateLoss()
        }
        listDialogFragment?.clear()
    }

    override fun onLogout() {
        CoreApplication.instance.clearData()
        val intent = Intent(this, HomeActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
    }


    fun displayError(message: String) {
        val view: View = if(findViewById<View>(R.id.main_view) != null) findViewById(R.id.main_view) else findViewById(android.R.id.content)
        Snackbar.make(view, message, Snackbar.LENGTH_SHORT)
            .setTextColor(Color.WHITE)
            .setBackgroundTint(ContextCompat.getColor(this, R.color.errorRed))
            .show()
    }

    fun displayMessage(message: String) {
        val view: View = if(findViewById<View>(R.id.main_view) != null) findViewById(R.id.main_view) else findViewById(android.R.id.content)
        Snackbar.make(view, message, Snackbar.LENGTH_LONG)
            .setTextColor(Color.WHITE)
            .setBackgroundTint(ContextCompat.getColor(this, R.color.successGreen))
            .show()
    }
}