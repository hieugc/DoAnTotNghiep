package com.example.homex

import android.os.Bundle
import android.view.*
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeDetailBinding


class MyHomeDetailFragment : BaseFragment<FragmentMyHomeDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_my_home_detail

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
    }
}