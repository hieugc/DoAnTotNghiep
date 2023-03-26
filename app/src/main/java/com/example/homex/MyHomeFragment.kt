package com.example.homex

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MyHomeAdapter
import com.example.homex.app.EMAIL
import com.example.homex.app.HOME
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel


class MyHomeFragment : BaseFragment<FragmentMyHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_my_home
    private lateinit var adapter: MyHomeAdapter
    private val viewModel: YourHomeViewModel by viewModel()
    private var page = 0

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(true, "Nhà của bạn"),
            showMenu = false,
            showMessage = false,
            showBoxChatLayout = Pair(false, null),
        )
        viewModel.getMyHomes(page)
        AppEvent.showLoading()
    }

    override fun setView() {
        adapter = MyHomeAdapter(
            arrayListOf(),
            onClick = {
                val action = MyHomeFragmentDirections.actionGlobalMyHomeDetailFragment(id = it)
                findNavController().navigate(action)
            }
        )
        binding.mainHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.mainHomeRecView.layoutManager = layoutManager
    }

    override fun setEvent() {
        binding.addHomeFAB.setOnClickListener {
            findNavController().navigate(R.id.action_myHomeFragment_to_addHomeFragment)
        }
    }

    override fun setViewModel() {
        viewModel.myHomesLiveData.observe(viewLifecycleOwner){
            if (it != null){
                val homes = it.homes
                Log.e("homes", "$homes")
                adapter.homeList?.clear()
                if (homes != null){
                    if(homes.size > 0){
                        adapter.homeList?.addAll(homes)
                        adapter.notifyDataSetChanged()
                    }
                }
                if(adapter.homeList.isNullOrEmpty()){
                    binding.mainHomeRecView.gone()
                    binding.noHomeTxt.visible()
                }else{
                    binding.mainHomeRecView.visible()
                    binding.noHomeTxt.gone()
                }
            }
            AppEvent.hideLoading()
        }
    }
}