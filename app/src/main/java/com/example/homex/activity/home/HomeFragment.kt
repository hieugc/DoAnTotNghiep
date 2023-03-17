package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.LinearSnapHelper
import com.example.homex.R
import com.example.homex.adapter.PopularHomeAdapter
import com.example.homex.adapter.PopularLocationAdapter
import com.example.homex.app.HOME
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentHomeBinding
import com.example.homex.utils.CenterZoomLayoutManager
import com.example.homex.viewmodel.HomeViewModel
import com.homex.core.util.AppEvent
import org.koin.android.ext.android.inject


class HomeFragment : BaseFragment<FragmentHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_home
    private lateinit var adapter: PopularLocationAdapter
    private lateinit var homeAdapter: PopularHomeAdapter
    private val viewModel: HomeViewModel by inject()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = true,
            showMenu = false,
            showMessage = true,
            showTitleApp = Pair(false, ""),
            showBottomNav = true,
            showBoxChatLayout = Pair(false, ""),
        )
        viewModel.getPopularHome()
        viewModel.getPopularLocation()
    }

    override fun setView() {
        adapter =
            PopularLocationAdapter(arrayListOf())
        binding.popularLocationRecView.adapter = adapter
        binding.popularLocationRecView.layoutManager = CenterZoomLayoutManager(context, LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0f, mShrinkDistance = 1f)

        homeAdapter =
            PopularHomeAdapter(
                arrayListOf(),
                onClick = {
                    findNavController().navigate(R.id.action_global_homeDetailFragment, bundleOf(HOME to it))
                }
            )

        binding.popularHomeRecView.adapter = homeAdapter
        binding.popularHomeRecView.layoutManager = CenterZoomLayoutManager(context, LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0.05f, mShrinkDistance = 0.8f)
        //Add snap helper to recyclerview to make it focus at 1 item
        val snapHelper = LinearSnapHelper()
        snapHelper.attachToRecyclerView(binding.popularHomeRecView)
        val snapHelper2 = LinearSnapHelper()
        snapHelper2.attachToRecyclerView(binding.popularLocationRecView)
    }

    override fun setEvent() {
        binding.searchBar.setOnClickListener {
            findNavController().navigate(R.id.action_homeFragment_to_searchFragment)
        }
    }

    override fun setViewModel() {
        viewModel.popularHome.observe(viewLifecycleOwner){ list->
            if(list != null){
                if(list.size > 0){
                    homeAdapter.homeList?.clear()
                    for(home in list){
                        homeAdapter.homeList?.add(home)
                    }
                    homeAdapter.notifyDataSetChanged()
                }
            }
            AppEvent.hideLoading()
        }

        viewModel.popularLocation.observe(viewLifecycleOwner){ list->
            if(list != null){
                if(list.size > 0){
                    adapter.list?.clear()
                    for(home in list){
                        adapter.list?.add(home)
                    }
                    adapter.notifyDataSetChanged()
                }
            }
            AppEvent.hideLoading()
        }
    }
}