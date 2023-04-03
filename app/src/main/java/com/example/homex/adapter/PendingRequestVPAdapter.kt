package com.example.homex.adapter

import android.os.Bundle
import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.homex.activity.home.pending.viewpager.PendingRequestFragment
import com.example.homex.activity.home.pending.viewpager.PendingRequestAcceptFragment
import com.example.homex.activity.home.pending.viewpager.PendingRequestDoneFragment
import com.example.homex.activity.home.pending.viewpager.PendingRequestRejectFragment
import com.example.homex.activity.home.pending.viewpager.PendingRequestReviewingFragment
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.extension.RequestStatus

class PendingRequestVPAdapter(parent: Fragment): FragmentStateAdapter(parent) {
    override fun getItemCount(): Int {
        return 5
    }

    override fun createFragment(position: Int): Fragment {
        if (position != 0)
        {
            return when (position){
                1-> PendingRequestAcceptFragment()
                2-> PendingRequestRejectFragment()
                3-> PendingRequestReviewingFragment()
                4->PendingRequestDoneFragment()
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