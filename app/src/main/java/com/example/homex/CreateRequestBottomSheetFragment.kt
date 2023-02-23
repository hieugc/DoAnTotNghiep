package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.findNavController
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCreateRequestBottomSheetBinding
import com.google.android.material.bottomsheet.BottomSheetDialogFragment


class CreateRequestBottomSheetFragment : BottomSheetDialogFragment() {
    private lateinit var binding: FragmentCreateRequestBottomSheetBinding

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentCreateRequestBottomSheetBinding.inflate(layoutInflater)

        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        binding.btnCreateRequest.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestBottomSheetFragment_to_createRequestFragment)
        }
    }

}