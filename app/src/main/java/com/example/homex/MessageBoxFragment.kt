package com.example.homex

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MessageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBoxBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.homex.core.model.OldMessage


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
        val arrayList = arrayListOf(
            OldMessage(id = "1", message = "alo bạn ơi", isMyMessage = false, date = "03/03/2023", userID = "1"),
            OldMessage(id = "1", message = "ok bạn, không có chi", isMyMessage = true, date = "01/03/2023", userID = "2"),
            OldMessage(id = "1", message = "ok bạn, cảm ơn bạn rất nhiều", isMyMessage = false, date = "01/03/2023", userID = "1"),
            OldMessage(id = "1", message = "có gì bạn cứ nhắn mình nhé", isMyMessage = true, date = "01/03/2023", userID = "2"),
            OldMessage(id = "1", message = "ok bạn hiền", isMyMessage = true, date = "01/03/2023", userID = "2"),
            OldMessage(id = "1", message = "tại mình cần mua một số thứ", isMyMessage = false, date = "01/03/2023", userID = "1"),
            OldMessage(id = "1", message = "ok bạn, có gì mình sẽ xem qua", isMyMessage = false, date = "01/03/2023", userID = "1"),
            OldMessage(id = "1", message = "cách căn hộ 3km sẽ có 1 trung tâm mua sắm bạn nhé !", isMyMessage = true, date = "01/03/2023", userID = "2"),
            OldMessage(id = "1", message = "có bạn ơi", isMyMessage = true, date = "01/03/2023", userID = "2"),
            OldMessage(id = "1", message = "bạn ơi, nhà của bạn có ở gần trung tâm mua sắm không nhỉ", isMyMessage = false, date = "01/03/2023", userID = "1"),
            OldMessage(id = "1", message = "alo alo", isMyMessage = false, date = "01/03/2023", userID = "1"),
        )
        val list = arrayListOf<OldMessage>()
        var date = arrayList[0].date
        for((index, msg) in arrayList.withIndex()){
            if(date != msg.date){
                list.add(
                    OldMessage(
                        id = null,
                        message = null,
                        isMyMessage = false,
                        date = date,
                        isDateItem = true
                    )
                )
                date = msg.date
            }
            list.add(msg)
            if(index == arrayList.size - 1){
                list.add(
                    OldMessage(
                        id = null,
                        message = null,
                        isMyMessage = false,
                        date = date,
                        isDateItem = true
                    )
                )
            }
        }
        adapter = MessageAdapter(
            list
        )
        binding.messageRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, true)
        binding.messageRecView.layoutManager = layoutManager
//
//        val dayOfWeekList =
//            listOf(
//                "T2", "T3", "T4", "T5", "T6", "T7", "CN"
//            )
//
//        val arrayAdapter = ArrayAdapter(requireContext(), R.layout.date_cell_item, dayOfWeekList)
//
//        binding.gridLayout.adapter = arrayAdapter
//
//        val calendar = Calendar.getInstance()
//        Log.e("year", "${calendar.get(Calendar.YEAR)}")
//        Log.e("month", "${calendar.get(Calendar.MONTH)}")
//        Log.e("dayOfMonth", "${calendar.get(Calendar.DAY_OF_MONTH)}")
//        Log.e("dayOfWeek", "${calendar.get(Calendar.DAY_OF_WEEK)}")
//        Log.e("maxDay", "${calendar.getActualMaximum(Calendar.DATE)}")
//        calendar.add(Calendar.MONTH, 1)
//
//        val arrayList = arrayListOf<String>()
//        val daysInMonth = calendar.getActualMaximum(Calendar.DATE)
//        calendar.set(Calendar.DAY_OF_MONTH, 1)
//        val dayOfWeek = calendar.get(Calendar.DAY_OF_WEEK) - 2
//        Log.e("dayOfWeek", "$dayOfWeek")
//        Log.e("daysInMonth", "$daysInMonth")
//
//
//        for(i in 1..42){
//            if(i <= dayOfWeek || i > daysInMonth + dayOfWeek){
//                arrayList.add("")
//            }else{
//                arrayList.add((i - dayOfWeek).toString())
//            }
//        }
//
//        monthAdapter = MyAdapter(
//            arrayList
//        )
//        binding.messageRecView.adapter = monthAdapter
//        val layoutManager = GridLayoutManager(requireContext(), 7)
//        binding.messageRecView.layoutManager = layoutManager

    }

    override fun setEvent() {
        binding.addBtn.setOnClickListener {
            if(binding.actionLayout.visibility == View.GONE)
            {
                binding.actionLayout.visible()
                binding.addBtn.setImageDrawable(ContextCompat.getDrawable(requireContext(), R.drawable.ic_close_circle))
            }
            else if(binding.actionLayout.visibility == View.VISIBLE)
            {
                binding.actionLayout.gone()
                binding.addBtn.setImageDrawable(ContextCompat.getDrawable(requireContext(), R.drawable.ic_pluscircle))
            }
        }

        binding.createRequestBtn.setOnClickListener {
            findNavController().navigate(R.id.action_messageBoxFragment_to_createRequestFragment)
        }
        binding.sendBtn.setOnClickListener {
            Log.e("send", "hello")
        }
        binding.msgInputLayout.setOnClickListener {
            Log.e("layout", "hello")
        }
    }
}