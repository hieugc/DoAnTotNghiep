package com.example.homex.activity.home.homepage

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.LinearSnapHelper
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.PopularHomeAdapter
import com.example.homex.adapter.PopularLocationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.utils.CenterZoomLayoutManager
import com.example.homex.viewmodel.HomeViewModel
import com.homex.core.model.Home
import com.homex.core.model.Location
import org.koin.androidx.viewmodel.ext.android.viewModel


class HomeFragment : BaseFragment<FragmentHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_home
    private lateinit var adapter: PopularLocationAdapter
    private lateinit var homeAdapter: PopularHomeAdapter
    private val homeList = arrayListOf<Home>()
    private val locationList = arrayListOf<Location>()
    private val viewModel: HomeViewModel by viewModel()
    private var isHomeShimmer = true
    private var isLocationShimmer = true

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        binding.homeShimmer.gone()
        binding.locationShimmer.gone()
        if (isHomeShimmer && isLocationShimmer){
            binding.homeShimmer.startShimmer()
            binding.homeShimmer.visible()
            binding.locationShimmer.startShimmer()
            binding.locationShimmer.visible()
        }
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = true,
            showMenu = false,
            showMessage = true,
            showTitleApp = Pair(false, ""),
            showBottomNav = true,
            showBoxChatLayout = Pair(false, null),
        )
        viewModel.getPopularHome()
        viewModel.getPopularLocation()
    }

    override fun setView() {
        adapter =
            PopularLocationAdapter(
                locationList
            )
        binding.popularLocationRecView.adapter = adapter
        binding.popularLocationRecView.layoutManager = CenterZoomLayoutManager(context, LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0f, mShrinkDistance = 1f)

        homeAdapter =
            PopularHomeAdapter(
                homeList,
                onClick = {
                    val action = HomeFragmentDirections.actionGlobalHomeDetailFragment(id = it)
                    findNavController().navigate(action)
                }
            )

        binding.popularHomeRecView.adapter = homeAdapter
        binding.popularHomeRecView.layoutManager = CenterZoomLayoutManager(context, LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0f, mShrinkDistance = 1f)
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
                val size = homeList.size
                if (size > 0){
                    homeList.clear()
                    homeAdapter.notifyItemRangeRemoved(0, size)
                }
                homeList.addAll(list)
                homeAdapter.notifyItemRangeInserted(0, list.size)
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isHomeShimmer = false
            }else{
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isHomeShimmer = false
            }
        }

        viewModel.popularLocation.observe(viewLifecycleOwner){ list->
            if(list != null){
                val size = locationList.size
                if (size > 0){
                    locationList.clear()
                    adapter.notifyItemRangeRemoved(0, size)
                }
                locationList.addAll(list)
                adapter.notifyItemRangeInserted(0, list.size)
                binding.locationShimmer.stopShimmer()
                binding.locationShimmer.gone()
                isLocationShimmer = false
            }else{
                binding.locationShimmer.stopShimmer()
                binding.locationShimmer.gone()
                isLocationShimmer = false
            }
        }
    }
}