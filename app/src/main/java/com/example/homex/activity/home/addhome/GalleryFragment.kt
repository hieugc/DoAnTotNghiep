package com.example.homex.activity.home.addhome

import android.Manifest
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.ContextCompat
import androidx.core.net.toUri
import androidx.core.os.bundleOf
import androidx.fragment.app.viewModels
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.GalleryAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentGalleryBinding
import com.example.homex.viewmodel.GalleryViewModel
import kotlinx.coroutines.flow.collectLatest
import pub.devrel.easypermissions.EasyPermissions


class GalleryFragment : BaseFragment<FragmentGalleryBinding>() {
    override val layoutId: Int = R.layout.fragment_gallery
    private var uri: Uri? = null
    private val viewModel: GalleryViewModel by viewModels()
    private val permissionLauncher = registerForActivityResult(ActivityResultContracts.RequestPermission()){ success->
        if(success == true){
            Log.e("ext", "granted")
            viewModel.loadImages()
            setupCollecting()
        }else{
            Log.e("ext", "denied")
        }
    }
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        requestPermission()
    }

    private fun requestPermission() {
        if (ContextCompat.checkSelfPermission(requireContext(), Manifest.permission.READ_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED){
            viewModel.loadImages()
        }else{
            permissionLauncher.launch(Manifest.permission.READ_EXTERNAL_STORAGE)
        }
    }

    override fun setView() {
        binding.galleryRecView.layoutManager = GridLayoutManager(requireContext(), 3)
        setupCollecting()
    }

    private fun setupCollecting() {
        lifecycleScope.launchWhenStarted {
            viewModel.imagesFromGallery.collectLatest { images ->
                if (images.isNotEmpty())
                {
                    val list : ArrayList<Uri> = arrayListOf()
                    list.addAll(images)
                    binding.galleryRecView.adapter = GalleryAdapter(list){
                        findNavController().previousBackStackEntry?.savedStateHandle?.set("Gallery", it)
                        findNavController().popBackStack()
                    }
                }
            }
        }
    }

}