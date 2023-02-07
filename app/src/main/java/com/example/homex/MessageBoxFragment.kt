package com.example.homex

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MessageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBoxBinding
import com.homex.core.model.Message


class MessageBoxFragment : BaseFragment<FragmentMessageBoxBinding>() {
    override val layoutId: Int = R.layout.fragment_message_box

    private lateinit var adapter: MessageAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(false, ""),
            showMessage = false,
            showMenu = true,
            showBoxChatLayout = Pair(true, "Chat"),
        )
    }

    override fun setView() {
        adapter = MessageAdapter(
            arrayListOf(
                Message(id = "1", message = "alo alo", isMyMessage = false),
                Message(id = "1", message = "bạn ơi, nhà của bạn có ở gần trung tâm mua sắm không nhỉ", isMyMessage = false),
                Message(id = "1", message = "có bạn ơi", isMyMessage = true),
                Message(id = "1", message = "cách căn hộ 3km sẽ có 1 trung tâm mua sắm bạn nhé !", isMyMessage = true),
                Message(id = "1", message = "ok bạn, có gì mình sẽ xem qua", isMyMessage = false),
                Message(id = "1", message = "tại mình cần mua một số thứ", isMyMessage = false),
                Message(id = "1", message = "ok bạn hiền", isMyMessage = true),
                Message(id = "1", message = "có gì bạn cứ nhắn mình nhé", isMyMessage = true),
                Message(id = "1", message = "ok bạn, cảm ơn bạn rất nhiều", isMyMessage = false),
                Message(id = "1", message = "ok bạn, không có chi", isMyMessage = true),
                Message(id = "1", message = "alo bạn ơi", isMyMessage = false),
            )
        )
        binding.messageRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.messageRecView.layoutManager = layoutManager

    }

    override fun setEvent() {
        binding.addBtn.setOnClickListener {
            Log.e("add", "hello")
        }
        binding.sendBtn.setOnClickListener {
            Log.e("send", "hello")
        }
        binding.msgInputLayout.setOnClickListener {
            Log.e("layout", "hello")
        }
    }
}