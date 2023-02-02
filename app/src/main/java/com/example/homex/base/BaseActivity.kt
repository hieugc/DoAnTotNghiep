package com.example.homex.base

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.example.homex.utils.PopupDialogFragment
import com.example.homex.utils.PopupHelper
import com.homex.core.model.PopUp
import com.homex.core.util.AppEvent
import com.homex.core.util.PopupEventListener
import pub.devrel.easypermissions.EasyPermissions

open class BaseActivity: PopupEventListener , AppCompatActivity() {
    companion object{
        var instance = BaseActivity()
    }

    private var listDialogFragment: ArrayList<PopupDialogFragment?>? = arrayListOf()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        AppEvent.registerListener(this)
        instance = this
    }

    override fun onBackPressed() {
        moveTaskToBack(true)
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)

        EasyPermissions.onRequestPermissionsResult(requestCode, permissions, grantResults)
    }

    override fun onStop() {
        super.onStop()
        AppEvent.unRegisterListener(this)
    }

    override fun onDestroy() {
        super.onDestroy()
        AppEvent.unRegisterListener(this)
    }

    override fun onShowLoading() {
        PopupHelper().showLoading()
    }

    override fun onHideLoading() {
        closePopup()
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

}