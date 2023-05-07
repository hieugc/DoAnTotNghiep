package com.example.homex.adapter

import android.os.Bundle
import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.homex.activity.home.profile.CircleRequestPagerFragment
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.extension.StatusWaitingRequest

class CircleRequestViewPager(private val parentFragment: Fragment) :
    FragmentStateAdapter(parentFragment) {
    override fun getItemCount(): Int = 5

    override fun createFragment(position: Int): Fragment {
        val fragment = CircleRequestPagerFragment()
        fragment.arguments = Bundle().apply {
            when (position) {
                0 -> putInt(REQUEST_STATUS, StatusWaitingRequest.INIT.ordinal)
                1 -> putInt(REQUEST_STATUS, StatusWaitingRequest.ACCEPT.ordinal)
                2 -> putInt(REQUEST_STATUS, StatusWaitingRequest.CHECK_IN.ordinal)
                3 -> putInt(REQUEST_STATUS, StatusWaitingRequest.CHECK_OUT.ordinal)
                4 -> putInt(REQUEST_STATUS, StatusWaitingRequest.ENDED.ordinal)
            }
        }

        return fragment
    }
}