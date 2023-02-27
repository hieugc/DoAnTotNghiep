package com.example.homex

import android.os.Bundle
import android.view.*
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.adapter.HomeRatingAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeDetailBinding
import com.example.homex.utils.CenterZoomLayoutManager


class MyHomeDetailFragment : BaseFragment<FragmentMyHomeDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_my_home_detail
    private lateinit var ratingAdapter: HomeRatingAdapter

    override fun setView() {
        val callback : ActionMode.Callback = object : ActionMode.Callback {

            override fun onCreateActionMode(mode: ActionMode?, menu: Menu?): Boolean {
                activity?.menuInflater?.inflate(R.menu.my_home_menu, menu)
                return true
            }

            override fun onPrepareActionMode(mode: ActionMode?, menu: Menu?): Boolean {
                return false
            }

            override fun onActionItemClicked(mode: ActionMode?, item: MenuItem?): Boolean {
                return when (item?.itemId) {
                    R.id.edit -> {
                        // Handle share icon press
                        true
                    }
                    R.id.delete -> {
                        // Handle delete icon press
                        true
                    }
                    else -> false
                }
            }

            override fun onDestroyActionMode(mode: ActionMode?) {
                findNavController().popBackStack()
            }
        }

        val actionMode = activity?.startActionMode(callback)
        actionMode?.title = "Thông tin nhà của bạn"

        ratingAdapter = HomeRatingAdapter(
            arrayListOf(
                "Nhà đẹp lắm mọi người",
                "Nhà thoải mái, đẹp",
                "Hoàn toàn tuyệt vời"
            )
        )
        binding.homeRatingRecView.adapter = ratingAdapter
        val layoutManager = CenterZoomLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0.05f, mShrinkDistance = 0.8f)
        binding.homeRatingRecView.layoutManager = layoutManager
    }
}