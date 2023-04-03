package com.example.homex.activity.home.pending

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.CONTACT_USER
import com.example.homex.app.ID
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPendingRequestDetailBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.viewmodel.ChatViewModel
import com.example.homex.viewmodel.RequestViewModel
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.request.UpdateStatusParam
import org.koin.androidx.viewmodel.ext.android.sharedViewModel
import org.koin.androidx.viewmodel.ext.android.viewModel


class PendingRequestDetailFragment : BaseFragment<FragmentPendingRequestDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_pending_request_detail
    private val viewModel: RequestViewModel by viewModel()
    private val args: PendingRequestDetailFragmentArgs by navArgs()
    private val chatViewModel: ChatViewModel by sharedViewModel()

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

        binding.contactBtn.setOnClickListener {
            chatViewModel.connectionId.value?.let { it1->
                binding.request?.request?.user?.userAccess?.let { it2->
                    chatViewModel.contactToUser(
                        ContactUserParam(
                            connectionId =  it1,
                            userAccess = it2
                        )
                    )
                }
            }

        }
    }

    override fun setViewModel() {
        viewModel.requestResponseLiveData.observe(this){
            if (it != null){
                binding.request = it
            }
        }

        chatViewModel.connectToUser.observe(this){ messageRoom->
            if (messageRoom != null){
                messageRoom.idRoom?.let {
                    findNavController().navigate(
                        R.id.action_global_messageFragment, bundleOf(
                            ID to it,
                            CONTACT_USER to true
                        )
                    )
                }
                chatViewModel.connectToUser.postValue(null)
            }
        }
    }
}