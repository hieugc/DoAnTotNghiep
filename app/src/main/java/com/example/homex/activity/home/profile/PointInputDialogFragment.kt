package com.example.homex.activity.home.profile

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.*
import android.widget.Toast
import androidx.core.widget.addTextChangedListener
import androidx.fragment.app.DialogFragment
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.activity.webview.WebviewActivity
import com.example.homex.databinding.FragmentNotificationDialogBinding
import com.example.homex.databinding.FragmentPointInputDialogBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.isValidEmail
import com.example.homex.viewmodel.ProfileViewModel
import com.homex.core.param.profile.TopUpPointParam
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.sharedViewModel
import org.koin.androidx.viewmodel.ext.android.viewModel
import vn.zalopay.sdk.ZaloPayError
import vn.zalopay.sdk.ZaloPaySDK
import vn.zalopay.sdk.listeners.PayOrderListener
import java.util.*
import kotlin.collections.ArrayList

class PointInputDialogFragment(private val listener: EventListener) : DialogFragment() {
    private lateinit var binding: FragmentPointInputDialogBinding
    private val viewModel: ProfileViewModel by viewModel()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        binding = FragmentPointInputDialogBinding.inflate(layoutInflater)
        return binding.root
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

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        binding.btnTopup.disable()
        initListener()
    }

    private fun initListener() {
        binding.closeBtn.setOnClickListener {
            dismiss()
        }

        binding.btnTopup.setOnClickListener {
            topUpPoint()
        }

        binding.pointAmountInputEdtTxt.addTextChangedListener { t->
            if (isValidValue(t.toString())){
                binding.btnTopup.enable()
            } else {
                binding.btnTopup.disable()
            }
        }

        binding.btnTopup.setOnClickListener {
            topUpPoint()
        }

        viewModel.topUpLiveData.observe(viewLifecycleOwner){
            if(it != null){
                ZaloPaySDK.getInstance().payOrder(activity as HomeActivity, it.zptranstoken, "demozpdk://app", object:
                    PayOrderListener {
                    override fun onPaymentCanceled(zpTransToken: String?, appTransID: String?) {
                        Toast.makeText(requireContext(), getString(R.string.top_up_cancel), Toast.LENGTH_SHORT).show()
                        dismiss()
                    }
                    override fun onPaymentError(zaloPayErrorCode: ZaloPayError?, zpTransToken: String?, appTransID: String?) {
                        Toast.makeText(requireContext(), getString(R.string.top_up_failed), Toast.LENGTH_SHORT).show()
                        dismiss()
                    }
                    override fun onPaymentSucceeded(transactionId: String, transToken: String, appTransID: String?) {
                        listener.onPaymentSuccess()
                        dismiss()
                    }
                })
            }
            AppEvent.closePopup()
        }
    }

    private fun isValidValue(input: String): Boolean {
        if (input.isEmpty()){
            return false
        }
        val value = input.toLong()
        return value > 0
    }

    private fun topUpPoint() {
        val input = binding.pointAmountInputEdtTxt.text.toString().trim()
        val value = input.toLong()
        viewModel.topUpPoint(TopUpPointParam(
            value
        ))
    }

    public interface EventListener{
        fun onPaymentSuccess()
    }

}