package com.example.homex.adapter

import android.os.Bundle
import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.homex.activity.home.profile.PaymentViewPagerFragment
import com.example.homex.app.PAYMENT_STATUS
import com.example.homex.extension.Payment

class PaymentHistoryViewPager(parentFragment: Fragment) :
    FragmentStateAdapter(parentFragment) {
    override fun getItemCount(): Int = 3

    override fun createFragment(position: Int): Fragment {
        val fragment = PaymentViewPagerFragment()
        fragment.arguments = Bundle().apply {
            when (position) {
                0 -> putInt(PAYMENT_STATUS, Payment.ALL.ordinal)
                1 -> putInt(PAYMENT_STATUS, Payment.VALID.ordinal)
                2 -> putInt(PAYMENT_STATUS, Payment.USED.ordinal)
            }
        }

        return fragment
    }
}