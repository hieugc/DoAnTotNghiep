package com.homex.core.util

import com.homex.core.model.PopUp
import java.util.concurrent.CopyOnWriteArraySet

object AppEvent {
    private var popupEventListeners: Set<PopupEventListener> = CopyOnWriteArraySet()

    fun registerListener(listener: PopupEventListener?) {
        if (listener != null)
            popupEventListeners = popupEventListeners.plus(listener)
    }
    fun unRegisterListener(listener: PopupEventListener?) {
        popupEventListeners = popupEventListeners.minus(listener ?: return)
    }

    fun showPopUpError(message: String?) {
        for (listener in popupEventListeners)
            listener.onShowPopUpError(message)
    }
    fun showPopUp(popup: PopUp? = null) {
        for (listener in popupEventListeners)
            listener.onShowPopUp(popup)
    }

    fun closePopup(){
        for (listener in popupEventListeners)
            listener.onClosePopup()
    }


}

interface PopupEventListener {
    fun onClosePopup()
    fun onShowPopUpError(message: String?)
    fun onShowPopUp(popup: PopUp?)
}

