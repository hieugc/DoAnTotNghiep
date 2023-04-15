package com.example.homex.adapter

import android.os.Bundle
import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.homex.activity.home.pending.viewpager.*
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.extension.RequestStatus

class PendingRequestVPAdapter(parent: Fragment): FragmentStateAdapter(parent) {
    override fun getItemCount(): Int {
        return 6
    }

    override fun createFragment(position: Int): Fragment {
        if (position != 0)
        {
            return when (position){
                1-> PendingRequestAcceptFragment()
                2-> PendingRequestRejectFragment()
                3-> PendingRequestCheckinFragment()
                4-> PendingRequestReviewingFragment()
                5-> PendingRequestDoneFragment()
                else-> PendingRequestFragment()
            }
        }
        val fragment = PendingRequestFragment()
        fragment.arguments = Bundle().apply {
            when(position){
                0-> putInt(REQUEST_STATUS, RequestStatus.WAITING.ordinal)
                1-> putInt(REQUEST_STATUS, RequestStatus.ACCEPTED.ordinal)
                2-> putInt(REQUEST_STATUS, RequestStatus.REJECTED.ordinal)
                3-> putInt(REQUEST_STATUS, RequestStatus.REVIEWING.ordinal)
                4-> putInt(REQUEST_STATUS, RequestStatus.DONE.ordinal)
            }

        }
        return fragment
    }
}