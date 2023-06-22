package com.example.homex.activity.home.homepage

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.core.view.isGone
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.LinearSnapHelper
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.PopularHomeAdapter
import com.example.homex.adapter.PopularLocationAdapter
import com.example.homex.app.LOCATION
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.utils.CenterZoomLayoutManager
import com.example.homex.viewmodel.HomeViewModel
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.util.AppEvent
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

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        viewModel.getPopularHome()
        viewModel.getPopularLocation()
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        binding.homeShimmer.gone()
        binding.locationShimmer.gone()
        if (isHomeShimmer && isLocationShimmer){
            binding.homeShimmer.startShimmer()
            binding.locationShimmer.startShimmer()
            binding.homeShimmer.visible()
            binding.locationShimmer.visible()
            binding.popularHomeRecView.visibility = View.INVISIBLE
            binding.popularLocationRecView.visibility = View.INVISIBLE
        }
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = true,
            showMenu = false,
            showMessage = true,
            showTitleApp = Pair(false, ""),
            showBottomNav = true,
            showBoxChatLayout = Pair(false, null),
        )
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            if (!isHomeShimmer && !isLocationShimmer){
                AppEvent.showPopUp()
                isHomeShimmer = true
                isLocationShimmer = true
                binding.homeShimmer.startShimmer()
                binding.homeShimmer.visible()
                binding.locationShimmer.startShimmer()
                binding.locationShimmer.visible()
                homeList.clear()
                locationList.clear()
                adapter.notifyDataSetChanged()
                homeAdapter.notifyDataSetChanged()
                binding.popularHomeRecView.visibility = View.INVISIBLE
                binding.popularLocationRecView.visibility = View.INVISIBLE
                viewModel.getPopularHome()
                viewModel.getPopularLocation()
                binding.swipeRefreshLayout.isRefreshing = false
            } else {
                binding.swipeRefreshLayout.isRefreshing = false
            }
        }
    }

    override fun setView() {
        adapter =
            PopularLocationAdapter(
                locationList
            ){
                findNavController().navigate(R.id.action_homeFragment_to_searchFragment, bundleOf(LOCATION to it))
            }
        binding.popularLocationRecView.setHasFixedSize(true)
        binding.popularLocationRecView.adapter = adapter
        binding.popularLocationRecView.layoutManager = CenterZoomLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false, 0.05f, 1f, 1.2)
        val snapHelper2 = LinearSnapHelper()
        snapHelper2.attachToRecyclerView(binding.popularLocationRecView)

        homeAdapter =
            PopularHomeAdapter(
                homeList,
                onClick = {
                    val action = HomeFragmentDirections.actionGlobalHomeDetailFragment(id = it)
                    findNavController().navigate(action)
                }
            )
        binding.popularHomeRecView.setHasFixedSize(true)
        binding.popularHomeRecView.adapter = homeAdapter
        binding.popularHomeRecView.layoutManager = CenterZoomLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false, 0.05f, 0.9f, 1.2)
        //Add snap helper to recyclerview to make it focus at 1 item
        val snapHelper = LinearSnapHelper()
        snapHelper.attachToRecyclerView(binding.popularHomeRecView)

    }

    override fun setEvent() {
        binding.searchBar.setOnClickListener {
            findNavController().navigate(R.id.action_homeFragment_to_searchFragment)
        }
    }

    override fun setViewModel() {
        viewModel.popularHome.observe(viewLifecycleOwner){ list->
            if(list != null){
                homeList.clear()
                homeList.addAll(list)
                homeAdapter.notifyDataSetChanged()
                if (homeList.isEmpty()){
                    binding.homeShimmer.stopShimmer()
                    binding.homeShimmer.gone()
                    isHomeShimmer = false
                }else{
                    if (isHomeShimmer){
                        binding.homeShimmer.stopShimmer()
                        binding.homeShimmer.gone()
                        isHomeShimmer = false
                    }
                    binding.popularHomeRecView.visible()
                }
            }else{
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isHomeShimmer = false
                binding.popularHomeRecView.gone()
            }
            if(binding.homeShimmer.isGone && binding.locationShimmer.isGone)
                AppEvent.closePopup()
        }

        viewModel.popularLocation.observe(viewLifecycleOwner){ list->
            if(list != null){
                locationList.clear()
                locationList.addAll(list)
                adapter.notifyDataSetChanged()
                if (locationList.isEmpty()){
                    binding.locationShimmer.stopShimmer()
                    binding.locationShimmer.gone()
                    isLocationShimmer = false
                }else{
                    if (isLocationShimmer){
                        binding.locationShimmer.stopShimmer()
                        binding.locationShimmer.gone()
                        isLocationShimmer = false
                    }
                    binding.popularLocationRecView.visible()
                }
            }else{
                binding.locationShimmer.stopShimmer()
                binding.locationShimmer.gone()
                isLocationShimmer = false
                binding.popularLocationRecView.gone()
            }
            if(binding.homeShimmer.isGone && binding.locationShimmer.isGone)
                AppEvent.closePopup()
        }
    }
}