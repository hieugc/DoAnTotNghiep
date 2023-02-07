package com.example.homex

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.FrameLayout
import androidx.core.content.ContextCompat
import androidx.core.text.HtmlCompat
import com.example.homex.databinding.FragmentFilterBottomSheetBinding
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.google.android.material.bottomsheet.BottomSheetDialog
import com.google.android.material.bottomsheet.BottomSheetDialogFragment


class FilterBottomSheetFragment : BottomSheetDialogFragment() {
    private lateinit var binding: FragmentFilterBottomSheetBinding

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentFilterBottomSheetBinding.inflate(layoutInflater)

        binding.deleteFilterTxt.text = HtmlCompat.fromHtml(resources.getString(R.string.delete_filter), HtmlCompat.FROM_HTML_MODE_LEGACY)


        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        binding.closeBtn.setOnClickListener {
            dismiss()
        }
    }


    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        val dialog : BottomSheetDialog = super.onCreateDialog(savedInstanceState) as BottomSheetDialog
        dialog.setOnShowListener {
            val d: BottomSheetDialog = it as BottomSheetDialog
            val bottomSheet : FrameLayout? =
                d.findViewById(com.google.android.material.R.id.design_bottom_sheet)
            bottomSheet?.apply {
                BottomSheetBehavior.from(bottomSheet).state = BottomSheetBehavior.STATE_EXPANDED
                dialog.window?.setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT)
                dialog.window?.setBackgroundDrawable(ColorDrawable(Color.TRANSPARENT))
            }

        }
        return dialog
    }
}