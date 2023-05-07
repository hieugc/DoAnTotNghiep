package com.example.homex.activity.home.profile

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.FrameLayout
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.databinding.FragmentRateBottomSheetBinding
import com.example.homex.viewmodel.RequestViewModel
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.google.android.material.bottomsheet.BottomSheetDialog
import com.google.android.material.bottomsheet.BottomSheetDialogFragment
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.request.CreateRatingParam
import com.homex.core.param.request.UpdateRatingParam
import org.koin.androidx.viewmodel.ext.android.viewModel

class RateBottomSheetFragment : BottomSheetDialogFragment() {
    private lateinit var binding: FragmentRateBottomSheetBinding
    private lateinit var request: RequestResponse
    private val viewModel: RequestViewModel by viewModel()
    private val args: RateBottomSheetFragmentArgs by navArgs()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentRateBottomSheetBinding.inflate(layoutInflater)

        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        initView()
        initListener()
    }

    private fun initView() {
        if(args.request != null) {
            request = args.request!!
            binding.tvTitle.text = request.house?.name
            binding.tvAddress.text = request.house?.location
            binding.tvPeople.text = request.house?.people.toString()
            binding.ratingHouse.rating = request.myRating?.rating?.toFloat() ?: 0f
            binding.ratingUser.rating = request.myRating?.ratingUser?.toFloat() ?: 0f
            binding.edtComment.setText(request.myRating?.content)
        }
    }

    private fun initListener() {
        binding.closeBtn.setOnClickListener {
            dismiss()
        }

        binding.btnSendRating.setOnClickListener {
            if (request.myRating == null) {
                val createRatingParam = CreateRatingParam(
                    binding.ratingHouse.rating.toInt(),
                    binding.ratingUser.rating.toInt(),
                    request.request?.id?.toInt() ?: 0,
                    binding.edtComment.text.toString()
                )
                viewModel.createRating(createRatingParam)
            } else {
                val updateRatingParam = UpdateRatingParam(
                    binding.ratingHouse.rating.toInt(),
                    binding.ratingUser.rating.toInt(),
                    request.myRating?.id?.toInt() ?: 0,
                    binding.edtComment.text.toString()
                )
                viewModel.updateRating(updateRatingParam)
            }
        }
    }


    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        val dialog: BottomSheetDialog =
            super.onCreateDialog(savedInstanceState) as BottomSheetDialog
        dialog.setOnShowListener {
            val d: BottomSheetDialog = it as BottomSheetDialog
            val bottomSheet: FrameLayout? =
                d.findViewById(com.google.android.material.R.id.design_bottom_sheet)
            bottomSheet?.apply {
                BottomSheetBehavior.from(bottomSheet).state = BottomSheetBehavior.STATE_EXPANDED
                dialog.window?.setLayout(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )
                dialog.window?.setBackgroundDrawable(ColorDrawable(Color.TRANSPARENT))
            }

        }
        return dialog
    }
}