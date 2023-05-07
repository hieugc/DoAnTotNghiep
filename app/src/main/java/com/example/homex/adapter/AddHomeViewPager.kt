package com.example.homex.adapter

import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter

class AddHomeViewPager(private val parentFragment: Fragment, private val listFragment: List<Fragment>):
    FragmentStateAdapter(parentFragment) {
    override fun getItemCount(): Int {
        return listFragment.size
    }

    override fun createFragment(position: Int): Fragment {
        return listFragment[position]
    }
}