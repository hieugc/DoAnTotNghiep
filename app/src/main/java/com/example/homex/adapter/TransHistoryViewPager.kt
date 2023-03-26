package com.example.homex.adapter

import android.os.Bundle
import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.homex.activity.home.TransHistoryPageFragment
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.extension.RequestStatus


class TransHistoryViewPager(private val parentFragment: Fragment) :
    FragmentStateAdapter(parentFragment) {
    override fun getItemCount(): Int = 5

    override fun createFragment(position: Int): Fragment {
        val fragment = TransHistoryPageFragment()
        fragment.arguments = Bundle().apply {
            when (position) {
                0 -> putInt(REQUEST_STATUS, RequestStatus.WAITING.ordinal)
                1 -> putInt(REQUEST_STATUS, RequestStatus.ACCEPTED.ordinal)
                2 -> putInt(REQUEST_STATUS, RequestStatus.REJECTED.ordinal)
                3 -> putInt(REQUEST_STATUS, RequestStatus.REVIEWING.ordinal)
                4 -> putInt(REQUEST_STATUS, RequestStatus.DONE.ordinal)
            }
        }

        return fragment
    }
}