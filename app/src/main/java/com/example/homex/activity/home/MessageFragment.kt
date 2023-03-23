package com.example.homex.activity.home

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.MessageBoxAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBinding
import com.example.homex.viewmodel.ChatViewModel
import com.homex.core.model.MessageBox
import com.homex.core.model.MessageRoom
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.sharedViewModel


class MessageFragment : BaseFragment<FragmentMessageBinding>() {
    override val layoutId: Int = R.layout.fragment_message
    private val viewModel: ChatViewModel by sharedViewModel()
    private val prefUtil: PrefUtil by inject()
    private val boxChatList = arrayListOf<MessageRoom>()
    private lateinit var adapter: MessageBoxAdapter
    private var page = 0

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true , "Trò chuyện"),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
        viewModel.getChatRoom(page)
    }

    override fun setView() {
        adapter = MessageBoxAdapter(
            boxChatList,
            onClick = { room->
                room.idRoom?.let {
                    findNavController().navigate(MessageFragmentDirections.actionMessageFragmentToMessageBoxFragment(it))
                }
            },
            prefUtil.profile?.userAccess
        )
        binding.messageBoxRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.messageBoxRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.chatRoom.observe(this){
            if (it != null){
                if(page == 0){
                    boxChatList.clear()
                }
                val rooms = it.rooms
                if(rooms != null){
                    boxChatList.addAll(rooms)
                }
                adapter.notifyDataSetChanged()
            }
            AppEvent.hideLoading()
        }

        viewModel.newMessage.observe(this){
            if (it != null){
                Log.e("newBox", "${it.messages}")
                for ((index, chat) in boxChatList.withIndex()){
                    if (chat.idRoom == it.idRoom){
                        boxChatList.removeAt(index)
                        break
                    }
                }
                boxChatList.add(0, it)
                adapter.notifyDataSetChanged()
                viewModel.newMessage.postValue(null)
            }
        }
    }
}