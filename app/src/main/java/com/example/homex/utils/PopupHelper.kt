package com.example.homex.utils

import android.app.Dialog
import android.view.View
import androidx.appcompat.widget.AppCompatButton
import androidx.appcompat.widget.AppCompatTextView
import androidx.constraintlayout.widget.ConstraintLayout
import com.example.homex.R
import com.example.homex.extension.addMessage
import com.homex.core.model.PopUp
import com.homex.core.util.AppEvent

class PopupHelper {

    //Error
    fun showErrorPopup(message: String?){
        //Init Message queue
        val messageQueue = ArrayList<(view: View?, dialog: Dialog?)->Unit>()
        messageQueue.addMessage(function = closePopUp())
        messageQueue.addMessage(function = updateContent(message))
        messageQueue.addMessage(function = close())

        //end

        val popup = PopUp(isBlur = false, popupId = R.layout.dialog_error, blurAt = null, messageQueue = messageQueue)
        AppEvent.showPopUp(popup)
    }

    private fun closePopUp(): (view: View?, dialog: Dialog?) -> Unit = { view, _ ->
        view?.findViewById<AppCompatButton>(R.id.btnOk)?.setOnClickListener {
            AppEvent.closePopup()
        }
    }

    fun showLoading(){
        val popup = PopUp(false, R.layout.dialog_loading, null)
        AppEvent.showPopUp(popup)
    }

    private fun updateContent(message: String?): (view: View?, dialog: Dialog?) -> Unit = { view, _ ->
        view?.findViewById<AppCompatTextView>(R.id.tvDetailError)?.text = message
    }

    private fun close(): (view: View?, dialog: Dialog?) -> Unit = { view, _ ->
        view?.findViewById<ConstraintLayout>(R.id.background)?.setOnClickListener {
            AppEvent.closePopup()
        }
    }
}