package com.example.homex.activity.home.addhome

import android.content.ContentResolver
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.ImageDecoder
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.provider.MediaStore
import android.util.Log
import android.view.View
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.navigation.fragment.findNavController
import androidx.navigation.navGraphViewModels
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.AddImageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome4Binding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.utils.RealPathUtil
import okhttp3.MediaType
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody.Companion.asRequestBody
import java.io.File
import java.io.FileOutputStream


class AddHome4Fragment : BaseFragment<FragmentAddHome4Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home4
    private lateinit var adapter: AddImageAdapter
    private var imgList = arrayListOf<Pair<Uri,Boolean>>()
    private var fileList = arrayListOf<File>()
    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})
    private val fileViewModel: FileViewModel by activityViewModels()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
//        val navController = findNavController();
//        // After a configuration change or process death, the currentBackStackEntry
//        // points to the dialog destination, so you must use getBackStackEntry()
//        // with the specific ID of your destination to ensure we always
//        // get the right NavBackStackEntry
//        val navBackStackEntry = navController.getBackStackEntry(R.id.addHomeFragment)
//
//        // Create our observer and add it to the NavBackStackEntry's lifecycle
//        val observer = LifecycleEventObserver { _, event ->
//            if (event == Lifecycle.Event.ON_RESUME
//                && navBackStackEntry.savedStateHandle.contains("IMG")) {
//                val result = navBackStackEntry.savedStateHandle.get<Uri>("IMG")
//                Log.e("backstackEntry", "$result")
//                // Do something with the result
//                if (result != null) {
//                    updateUI(result, true)
//                }
//            }
//        }
//        navBackStackEntry.lifecycle.addObserver(observer)
//
//        // As addObserver() does not automatically remove the observer, we
//        // call removeObserver() manually when the view lifecycle is destroyed
//        viewLifecycleOwner.lifecycle.addObserver(LifecycleEventObserver { _, event ->
//            if (event == Lifecycle.Event.ON_DESTROY) {
//                navBackStackEntry.lifecycle.removeObserver(observer)
//            }
//        })
//        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Uri>("IMG")?.observe(viewLifecycleOwner){
//            if (it != null) {
//            }
//            findNavController().currentBackStackEntry?.savedStateHandle?.remove<Uri>("IMG")
//        }
//        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Uri?>("Gallery")?.observe(viewLifecycleOwner){
//            Log.e("backstackEntryGallery", "$it")
//            if (it != null) {
//                updateUI(it, false)
//            }
//            findNavController().currentBackStackEntry?.savedStateHandle?.remove<Uri>("Gallery")
//        }
    }

    private fun updateUI(uri: Uri, fromCamera: Boolean) {
        binding.homeImageRecView.visible()
        binding.finishBtn.enable()
        Log.e("notify", "${imgList}")
//        imgList.add(uri)
//        uri.path?.let {
//            if(fromCamera){
//                val file = File(it)
//                Log.e("file", "${file.exists()}")
//                fileList.add(file)
//            }else{
//                val path = RealPathUtil.getRealPath(requireContext(), uri)
//                val file = File(path)
//                Log.e("file", "${file.exists()}")
//                fileList.add(file)
//            }
//            viewModel.files.postValue(fileList)
//        }

        Log.e("notify", "${imgList}")
        adapter.notifyItemInserted(imgList.size-1)
        when(imgList.size){
            5->{
                binding.btnAddImage.gone()
            }
            else->{
                binding.btnAddImage.visible()
                binding.btnAddImage.text = "Thêm Hình Ảnh ${imgList.size}/5"
            }
        }
//        fileViewModel.file.postValue(null)
    }

    override fun setView() {
        adapter = AddImageAdapter(
            mutableListOf()
        ){
            imgList.removeAt(it)
            val list = mutableListOf<Pair<Uri, Boolean>>()
            list.addAll(imgList)
            fileViewModel.file.postValue(list)
//            fileList[it].delete()
//            fileList.removeAt(it)
//            viewModel.files.postValue(fileList)
        }
        adapter.imgList = imgList
        binding.homeImageRecView.adapter = adapter
        binding.homeImageRecView.layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.finishBtn.disable()
    }

    override fun setEvent() {
        binding.btnAddImage.setOnClickListener {
            (parentFragment as AddHomeFragment).openBottomSheet()
        }
        binding.finishBtn.setOnClickListener {
            (parentFragment as AddHomeFragment).createHome()
        }
    }

    override fun setViewModel() {
        fileViewModel.file.observe(viewLifecycleOwner){
            if (it != null) {
                imgList.clear()
                Log.e("viewModelList", "$it")
                if (it.size > 0){
                    binding.homeImageRecView.visible()
                    binding.finishBtn.enable()
                    imgList.addAll(it)
                }
                Handler(Looper.getMainLooper()).post {
                    when(imgList.size){
                        5->{
                            binding.btnAddImage.gone()
                        }
                        0->{
                            binding.homeImageRecView.gone()
                            binding.finishBtn.disable()
                            binding.btnAddImage.text = getString(R.string.upload_image)
                        }
                        else->{
                            binding.btnAddImage.visible()
                            binding.btnAddImage.text = "Thêm Hình Ảnh ${imgList.size}/5"
                        }
                    }
                }
                adapter.notifyDataSetChanged()
            }
        }
    }


}