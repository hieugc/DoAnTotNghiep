package com.example.homex.activity.home.profile

import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.CONTACT_USER
import com.example.homex.app.ID
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCircleRequestDetailBinding
import com.example.homex.viewmodel.ChatViewModel
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CircleRequestDetailFragment : BaseFragment<FragmentCircleRequestDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_circle_request_detail
    private val viewModel: RequestViewModel by viewModel()
    private val args: CircleRequestDetailFragmentArgs by navArgs()
    private val chatViewModel: ChatViewModel by viewModel()
    private val prefUtil: PrefUtil by inject()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        binding = FragmentCircleRequestDetailBinding.inflate(layoutInflater)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showTitleApp = Pair(true, "Chi tiết yêu cầu"),
            showBottomNav = false,
            showSearchLayout = false,
            showMessage = false,
            showMenu = false,
            showBoxChatLayout = Pair(false, null),
        )

        if (args.request != null) {
            binding.request = args.request
        }
    }

    override fun setEvent() {
        binding.contactBtnTo.setOnClickListener {
            prefUtil.connectionId?.let { it1 ->
                binding.request?.nextNode?.user?.userAccess?.let { it2 ->
                    chatViewModel.contactToUser(
                        ContactUserParam(
                            connectionId = it1,
                            userAccess = it2
                        )
                    )
                }
            }
        }

        binding.contactBtnFrom.setOnClickListener {
            prefUtil.connectionId?.let { it1 ->
                binding.request?.prevNode?.user?.userAccess?.let { it2 ->
                    chatViewModel.contactToUser(
                        ContactUserParam(
                            connectionId = it1,
                            userAccess = it2
                        )
                    )
                }
            }
        }
    }

    override fun setViewModel() {
        chatViewModel.connectToUser.observe(this) { messageRoom ->
            Log.e("messageRoom", "$messageRoom")
            if (messageRoom != null) {
                messageRoom.idRoom?.let {
                    findNavController().navigate(
                        R.id.action_global_messageFragment, bundleOf(
                            ID to it,
                            CONTACT_USER to true
                        )
                    )
                }
                chatViewModel.clearContactUser()
            }
        }
    }
}