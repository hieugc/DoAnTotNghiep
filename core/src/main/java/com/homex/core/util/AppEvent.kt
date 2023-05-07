package com.homex.core.util

import com.homex.core.model.PopUp
import java.util.concurrent.CopyOnWriteArraySet

object AppEvent {
    // Logout event
    private var authListeners: Set<AuthenticationListener> = CopyOnWriteArraySet()

    fun registerAuthListener(listener: AuthenticationListener?) {
        if (listener != null)
            authListeners = authListeners.plus(listener)
    }

    fun unRegisterAuthListener(listener: AuthenticationListener?) {
        authListeners = authListeners.minus(listener ?: return)
    }

    fun onLogout() {
        for (index in authListeners.size - 1 downTo 0) {
            authListeners.elementAt(index).onLogout()
            break
        }
    }

    // Popup
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

interface AuthenticationListener{
    fun onLogout()
}

