package com.homex.core.util

import com.homex.core.model.PopUp
import java.util.concurrent.CopyOnWriteArraySet

object AppEvent {
    private var popupEventListeners: Set<PopupEventListener> = CopyOnWriteArraySet()

    fun showLoading() {
        for (listener in popupEventListeners)
            listener.onShowLoading()
    }
    fun registerListener(listener: PopupEventListener) {
        popupEventListeners = popupEventListeners.plus(listener)
    }
    fun unRegisterListener(listener: PopupEventListener?) {
        popupEventListeners = popupEventListeners.minus(listener ?: return)
    }
    fun hideLoading() {
        for (listener in popupEventListeners)
            listener.onHideLoading()

    }

    fun showPopUpError(message: String?) {
        for (listener in popupEventListeners)
            listener.onShowPopUpError(message)
    }
    fun showPopUp(popup: PopUp?) {
        for (listener in popupEventListeners)
            listener.onShowPopUp(popup)
    }
    fun showPopup(){

    }
    fun closePopup(){
        for (listener in popupEventListeners)
            listener.onClosePopup()
    }


}

interface PopupEventListener {
    fun onShowLoading()
    fun onHideLoading()
    fun onClosePopup()
    fun onShowPopUpError(message: String?)
    fun onShowPopUp(popup: PopUp?)
}

