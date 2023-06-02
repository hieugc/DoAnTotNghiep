package com.example.homex.activity.home.profile

import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.databinding.FragmentRateBottomSheetBinding
import com.example.homex.utils.CustomBottomSheet
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.request.CreateRatingParam
import com.homex.core.param.request.UpdateRatingParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class RateBottomSheetFragment : CustomBottomSheet<FragmentRateBottomSheetBinding>() {
    override val layoutId: Int
        get() = R.layout.fragment_rate_bottom_sheet
    private lateinit var request: RequestResponse
    private val viewModel: RequestViewModel by viewModel()
    private val args: RateBottomSheetFragmentArgs by navArgs()

    override fun setView() {
        initView()
    }

    override fun setEvent() {
        initListener()
    }

    private fun initView() {
        if(args.request != null) {
            request = args.request!!
            binding.tvTitle.text = request.house?.name
            binding.tvAddress.text = request.house?.getFullLocation()
            binding.tvPeople.text = getString(R.string.people, request.house?.people)
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
                    request.request?.id ?: 0,
                    binding.edtComment.text.toString()
                )
                viewModel.createRating(createRatingParam)
            } else {
                val updateRatingParam = UpdateRatingParam(
                    binding.ratingHouse.rating.toInt(),
                    binding.ratingUser.rating.toInt(),
                    request.myRating?.id ?: 0,
                    binding.edtComment.text.toString()
                )
                viewModel.updateRating(updateRatingParam)
            }
        }
    }
}