package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPendingRequestDetailBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.viewmodel.RequestViewModel
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.homex.core.param.request.UpdateStatusParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class PendingRequestDetailFragment : BaseFragment<FragmentPendingRequestDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_pending_request_detail
    private val viewModel: RequestViewModel by viewModel()
    private val args: PendingRequestDetailFragmentArgs by navArgs()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo =  false,
            showTitleApp = Pair(true, "Chi tiết yêu cầu"),
            showBottomNav = false,
            showSearchLayout = false,
            showMessage = false,
            showMenu = false,
            showBoxChatLayout = Pair(false, null),
        )

        if (args.id != 0){
            viewModel.getRequestDetail(args.id)
        }
    }

    override fun setEvent() {
        binding.rejectBtn.setOnClickListener {
            MaterialAlertDialogBuilder(requireContext())
                .setTitle(getString(R.string.reject_request))
                .setMessage(getString(R.string.reject_request_message))
                .setNegativeButton(resources.getString(R.string.cancel)) { _, _ ->
                    // Respond to negative button press
                }
                .setPositiveButton(resources.getString(R.string.confirm)) { dialog, _ ->
                    // Respond to positive button press
                    dialog.dismiss()
                    if (args.id != 0){
                        val param = UpdateStatusParam(
                            id = args.id,
                            status = RequestStatus.REJECTED.ordinal
                        )
                        viewModel.updateStatus(param)
                    }
                }
                .show()
        }

        binding.acceptBtn.setOnClickListener {
            MaterialAlertDialogBuilder(requireContext())
                .setTitle(getString(R.string.accept_request))
                .setMessage(getString(R.string.accept_request_message))
                .setNegativeButton(resources.getString(R.string.cancel)) { _, _ ->
                    // Respond to negative button press
                }
                .setPositiveButton(resources.getString(R.string.confirm)) { dialog, _ ->
                    // Respond to positive button press
                    dialog.dismiss()
                    if (args.id != 0){
                        val param = UpdateStatusParam(
                            id = args.id,
                            status = RequestStatus.ACCEPTED.ordinal
                        )
                        viewModel.updateStatus(param)
                    }
                }
                .show()
        }
    }

    override fun setViewModel() {
        viewModel.requestResponseLiveData.observe(this){
            if (it != null){
                binding.request = it
            }
        }
    }
}