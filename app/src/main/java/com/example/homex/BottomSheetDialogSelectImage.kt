package com.example.homex

import android.Manifest
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.ContextCompat
import androidx.core.content.FileProvider
import androidx.navigation.fragment.findNavController
import com.example.homex.databinding.FragmentBottomSheetDialogSelectImageBinding
import com.google.android.material.bottomsheet.BottomSheetDialogFragment
import com.homex.core.util.AppEvent
import pub.devrel.easypermissions.EasyPermissions
import java.io.File
import java.lang.Exception
import java.util.*


class BottomSheetDialogSelectImage : BottomSheetDialogFragment(){
    private lateinit var binding: FragmentBottomSheetDialogSelectImageBinding
    private var uri: Uri? = null
    private val resultLauncher = registerForActivityResult(ActivityResultContracts.TakePicture()){ success->
        if(success){
            Log.e("takePicture", "success $uri")
            findNavController().previousBackStackEntry?.savedStateHandle?.set("IMG", uri)
            dialog?.dismiss()
        }
    }
    private val permissionResult = registerForActivityResult(ActivityResultContracts.RequestPermission()){ success->
        if(success == true){
            openCamera()
            Log.e("permission", "granted")
        }else{
            Log.e("permission", "denied")
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentBottomSheetDialogSelectImageBinding.inflate(inflater)
        return binding.root
    }


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        binding.btnCancel.setOnClickListener {
            dismiss()
        }
        binding.btnGallery.setOnClickListener {
            findNavController().navigate(R.id.action_bottomSheetDialogSelectImage_to_galleryFragment)
        }
        binding.btnTakeNewPhoto.setOnClickListener {
            requestPermission()
        }
    }

    private fun requestPermission() {
        if (ContextCompat.checkSelfPermission(
                requireContext(),
                Manifest.permission.CAMERA
            ) == PackageManager.PERMISSION_GRANTED
        ) {
            openCamera()
        }
        else{
            permissionResult.launch(Manifest.permission.CAMERA)
        }
    }

    private fun openCamera() {
        try{
            uri = initTempUri()
            resultLauncher.launch(uri)
        }
        catch (e: Exception){
            AppEvent.showPopUpError(e.message)
        }
    }

    private fun initTempUri(): Uri? {
        //gets the temp_images dir
        val tempImagesDir = File(
            activity?.applicationContext?.filesDir, //this function gets the external cache dir
            getString(R.string.temp_images_dir)) //gets the directory for the temporary images dir

        tempImagesDir.mkdir() //Create the temp_images dir

        val date = Date()
        //Creates the temp_image.jpg file
        val tempImage = File(
            tempImagesDir, //prefix the new abstract path with the temporary images dir path
            "${date.time}_" + getString(R.string.temp_image)) //gets the abstract temp_image file name

        //Returns the Uri object to be used with ActivityResultLauncher
        return activity?.applicationContext?.let {
            FileProvider.getUriForFile(
                it,
                it.packageName + ".provider",
                tempImage)
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        clearFile()
    }

    private fun clearFile(){
        uri?.let { context?.contentResolver?.delete(it, null, null) };
    }

}