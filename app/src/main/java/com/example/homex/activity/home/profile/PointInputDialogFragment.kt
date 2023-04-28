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
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.*
import kotlin.collections.ArrayList

class PointInputDialogFragment : DialogFragment() {
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
                if(it.redirect.isNotEmpty()) {
                    val myIntent = WebviewActivity.open(requireContext())
                    myIntent.putExtra("redirect_url", it.redirect)
                    startActivity(myIntent)
                } else {
                    Toast.makeText(requireContext(), "ERROR", Toast.LENGTH_SHORT).show()
                }
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

}