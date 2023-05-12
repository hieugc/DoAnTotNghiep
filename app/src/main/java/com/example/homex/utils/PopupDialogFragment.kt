package com.example.homex.utils

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.Window
import android.view.WindowManager
import androidx.fragment.app.DialogFragment
import androidx.fragment.app.FragmentManager
import com.example.homex.R
import com.example.homex.app.POPUP_MODEL
import com.homex.core.model.PopUp

class PopupDialogFragment: DialogFragment() {
    private var popup : PopUp? = null
    private var popupView: View? = null

    companion object{
        fun newInstance(popup: PopUp?): PopupDialogFragment =
            PopupDialogFragment().apply {
                arguments = Bundle().apply {
                    putParcelable(POPUP_MODEL, popup)
                }
            }
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        if(popup == null)
            return
        popup?.messageQueue?.forEach{
            it.invoke(view, dialog)
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        popup = arguments?.getParcelable(POPUP_MODEL)
        popupView = layoutInflater.inflate(popup?.popupId?: R.layout.layout_loading, container)
        return popupView
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        val dialog = super.onCreateDialog(savedInstanceState)
        dialog.window?.requestFeature(Window.FEATURE_NO_TITLE)
        dialog.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        return dialog
    }

    override fun onStart() {
        super.onStart()
        dialog?.window?.setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT)
        dialog?.window?.setBackgroundDrawable(ColorDrawable(Color.TRANSPARENT))
    }

    override fun show(manager: FragmentManager, tag: String?) {
        val ft = manager.beginTransaction()
        ft.add(this, tag)
        ft.commitAllowingStateLoss()
    }
}