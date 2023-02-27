package com.example.homex

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.SearchHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchResultBinding
import com.microsoft.maps.MapRenderMode
import com.microsoft.maps.MapView


class SearchResultFragment : BaseFragment<FragmentSearchResultBinding>() {
    override val layoutId: Int = R.layout.fragment_search_result

    private lateinit var adapter: SearchHomeAdapter
    private lateinit var mapView: MapView

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMessage = false,
            showBottomNav = false,
            showMenu = false,
            showTitleApp = Pair(false, ""),
            showBoxChatLayout = Pair(false, "")
        )

        (activity as HomeActivity).showSearchLayout()

        mapView = MapView(requireContext(), MapRenderMode.RASTER)
        mapView.setCredentialsKey(BuildConfig.CREDENTIALS_KEY)
        binding.mapView.addView(mapView)
        mapView.onCreate(savedInstanceState)
    }

    override fun setEvent() {
        binding.iconFilter.setOnClickListener {
            findNavController().navigate(R.id.action_searchResultFragment_to_filterBottomSheetFragment)
        }
    }

    override fun setView() {
        adapter =
            SearchHomeAdapter(
                arrayListOf(
                    "Nhà của Hiếu", "Nhà của tui", "Nhà Nhà Nhà", "Hello mudafakar"
                )
            )

        binding.searchHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.searchHomeRecView.layoutManager = layoutManager
    }
}