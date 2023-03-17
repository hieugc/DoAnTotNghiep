package com.example.homex

import android.Manifest
import android.content.ContentResolver
import android.content.pm.PackageManager
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.ImageDecoder
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.os.Environment
import android.provider.MediaStore
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.ContextCompat
import androidx.core.content.FileProvider
import androidx.fragment.app.activityViewModels
import androidx.navigation.fragment.findNavController
import androidx.navigation.navGraphViewModels
import com.example.homex.activity.home.addhome.FileViewModel
import com.example.homex.databinding.FragmentBottomSheetDialogSelectImageBinding
import com.google.android.material.bottomsheet.BottomSheetDialogFragment
import com.homex.core.util.AppEvent
import java.io.File
import java.io.FileOutputStream
import java.text.SimpleDateFormat
import java.util.*


class BottomSheetDialogSelectImage : BottomSheetDialogFragment(){
    private lateinit var binding: FragmentBottomSheetDialogSelectImageBinding
    private var uri: Uri? = null
    val viewModel: FileViewModel by activityViewModels()
    private val resultLauncher = registerForActivityResult(ActivityResultContracts.TakePicture()){ success->
        if(success){
            Log.e("takePicture", "success $uri")
//            val bitmap: Bitmap?
//            val contentResolver: ContentResolver = requireContext().contentResolver
//            try {
//                bitmap = if (Build.VERSION.SDK_INT < 28) {
//                    MediaStore.Images.Media.getBitmap(contentResolver, uri)
//                } else {
//                    uri?.let{
//                        val source = ImageDecoder.createSource(contentResolver, it)
//                        ImageDecoder.decodeBitmap(source)
//                    }
//                    null
//                }
//
//                val storageDir = context?.getExternalFilesDir(Environment.DIRECTORY_PICTURES)
//                val imageFile = File.createTempFile(
//                    "JPEG_${SimpleDateFormat("yyyyMMdd_HHmmss").format(Date())}_",
//                    ".jpg",
//                    storageDir
//                )
//               val file = File(uri?.path ?: "")
//                Log.e("path", "${uri?.path}")
//                Log.e("file", "${file.exists()}")
////                val bitmap = BitmapFactory.decodeFile(file.absolutePath)
//                val outputStream = FileOutputStream(file)
//                bitmap?.compress(Bitmap.CompressFormat.JPEG, 100, outputStream)
//                outputStream.flush()
//                outputStream.close()
//            } catch (e: Exception) {
//                e.printStackTrace()
//            }
//            findNavController().previousBackStackEntry?.savedStateHandle?.set("IMG", uri)
            uri?.let {
                Log.e("hello", "addlist")
                viewModel.addFile(it, true)
            }
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
//        val dir = context?.getExternalFilesDir(Environment.DIRECTORY_PICTURES)
//        val date = Date()
//        if(dir?.exists() == false){
//            dir.mkdir()
//        }
//        val random = Random()
//        val n = random.nextInt(10000)
//        val file = File(dir, "IMG_${date.time}_${n}.jpg")
//        Log.e("filePath", file.path)
//        if(file.exists()){
//            file.delete()
//        }
//        return Uri.fromFile(file)
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