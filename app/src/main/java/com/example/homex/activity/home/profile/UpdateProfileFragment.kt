package com.example.homex.activity.home.profile

import android.graphics.Bitmap
import android.net.Uri
import android.os.Bundle
import android.os.Environment
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.View
import android.widget.Toast
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.navigation.fragment.findNavController
import com.bumptech.glide.Glide
import com.bumptech.glide.load.DataSource
import com.bumptech.glide.load.engine.GlideException
import com.bumptech.glide.request.RequestListener
import com.bumptech.glide.request.target.Target
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.DATE_TIME_FORMAT
import com.example.homex.app.IMAGE
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUpdateProfileBinding
import com.example.homex.viewmodel.ProfileViewModel
import com.homex.core.param.profile.UpdateProfileParam
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import okhttp3.MediaType
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody.Companion.asRequestBody
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.io.File
import java.io.FileOutputStream
import java.text.SimpleDateFormat
import java.util.*

class UpdateProfileFragment : BaseFragment<FragmentUpdateProfileBinding>() {
    override val layoutId: Int = R.layout.fragment_update_profile
    private val prefUtil: PrefUtil by inject()
    private var validTime = true
    private lateinit var file: File
    private lateinit var updateProfileParam: UpdateProfileParam
    private val viewModel: ProfileViewModel by viewModel()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.user_profile)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
        val navController = findNavController()
        // After a configuration change or process death, the currentBackStackEntry
        // points to the dialog destination, so you must use getBackStackEntry()
        // with the specific ID of your destination to ensure we always
        // get the right NavBackStackEntry
        val navBackStackEntry = navController.getBackStackEntry(R.id.updateProfileFragment)

        // Create our observer and add it to the NavBackStackEntry's lifecycle
        val observer = LifecycleEventObserver { _, event ->
            if (event == Lifecycle.Event.ON_RESUME
                && navBackStackEntry.savedStateHandle.contains(IMAGE)
            ) {
                val result = navBackStackEntry.savedStateHandle.get<Uri>(IMAGE)
                // Do something with the result
                var bitmap: Bitmap?
                Glide.with(requireContext())
                    .asBitmap()
                    .load(result)
                    .listener(object : RequestListener<Bitmap> {
                        override fun onLoadFailed(
                            e: GlideException?,
                            model: Any?,
                            target: Target<Bitmap>?,
                            isFirstResource: Boolean
                        ): Boolean {
                            bitmap = null
                            return false
                        }

                        override fun onResourceReady(
                            resource: Bitmap?,
                            model: Any?,
                            target: Target<Bitmap>?,
                            dataSource: DataSource?,
                            isFirstResource: Boolean
                        ): Boolean {
                            bitmap = resource
                            file = saveBitmapToSDCard(bitmap)
                            return false
                        }

                    })
                    .placeholder(R.drawable.ic_baseline_image_24)
                    .error(R.mipmap.avatar)
                    .into(binding.ivAvatar)
            }
        }
        navBackStackEntry.lifecycle.addObserver(observer)

        // As addObserver() does not automatically remove the observer, we
        // call removeObserver() manually when the view lifecycle is destroyed
        viewLifecycleOwner.lifecycle.addObserver(LifecycleEventObserver { _, event ->
            if (event == Lifecycle.Event.ON_DESTROY) {
                navBackStackEntry.lifecycle.removeObserver(observer)
            }
        })
    }

    override fun setView() {
        if (prefUtil.profile != null) {
            binding.user = prefUtil.profile
            Glide.with(requireContext())
                .load(prefUtil.profile!!.urlImage)
                .placeholder(R.drawable.ic_baseline_image_24)
                .error(R.mipmap.avatar)
                .into(binding.ivAvatar)
        }
    }

    override fun setViewModel() {
        viewModel.updateProfileLiveData.observe(viewLifecycleOwner) {
                findNavController().popBackStack()
                Toast.makeText(
                    requireContext(),
                    getString(R.string.update_profile_success),
                    Toast.LENGTH_LONG
                ).show()
            AppEvent.closePopup()
        }
    }

    override fun setEvent() {
        binding.tvEditProfile.setOnClickListener {
            findNavController().navigate(R.id.action_updateProfileFragment_to_bottomSheetDialogSelectImage)
        }

        binding.btnUpdate.setOnClickListener {
            updateProfile()
        }

        binding.dobInputEdtTxt.addTextChangedListener(object : TextWatcher {
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
            override fun afterTextChanged(s: Editable?) {}

            private var current: String = ""
            private var cal = Calendar.getInstance()

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
                if (s.toString() != current) {
                    var clean: String = s.toString().replace("[^\\d.]|\\.".toRegex(), "")
                    val cleanC = current.replace("[^\\d.]|\\.".toRegex(), "")
                    Log.e("cleanS", clean)
                    Log.e("cleanC", cleanC)
                    val cl = clean.length
                    var sel = cl
                    var i = 2
                    while (i <= cl && i < 6) {
                        sel++
                        i += 2
                    }
                    //Deleting a slash / rather than a number
                    if (clean == cleanC) sel--
                    validTime = false
                    if (clean.length >= 8) {
                        //Create new instance to validate Maximum date
                        //Using old instance will get 30/02 or 31/02 date maximum for some reason
                        cal = Calendar.getInstance()
                        //This part makes sure that when we finish entering numbers
                        //the date is correct, fixing it otherwise
                        var day = clean.substring(0, 2).toInt()
                        var mon = clean.substring(2, 4).toInt()
                        var year = clean.substring(4, 8).toInt()
                        Log.e("day", "$day")
                        Log.e("mon", "$mon")
                        Log.e("year", "$year")
                        //Add a flag to notify user if their date is wrong
                        //and will be re-format
                        var flag = false
                        mon = if (mon < 1) {
                            flag = true
                            1
                        } else if (mon > 12) {
                            flag = true
                            12
                        } else mon
                        cal[Calendar.MONTH] = mon - 1
                        val thisYear = Calendar.getInstance().get(Calendar.YEAR)
                        year = if (year < 1900) {
                            flag = true
                            1900
                        } else if (year > thisYear) {
                            flag = true
                            thisYear
                        } else year
                        cal[Calendar.YEAR] = year
                        // ^ first set year for the line below to work correctly
                        //with leap years - otherwise, date e.g. 29/02/2012
                        //would be automatically corrected to 28/02/2012
                        Log.e("max", "${cal.getActualMaximum(Calendar.DATE)}")
                        day = if (day > cal.getActualMaximum(Calendar.DATE)) {
                            flag = true
                            cal.getActualMaximum(
                                Calendar.DATE
                            )
                        } else day
                        cal[Calendar.DATE] = day

                        if (cal.timeInMillis > Calendar.getInstance().timeInMillis) {
                            flag = true
                            cal = Calendar.getInstance()
                            day = cal.get(Calendar.DAY_OF_MONTH)
                            mon = cal.get(Calendar.MONTH) + 1
                            year = cal.get(Calendar.YEAR)
                        }

                        clean = String.format("%02d%02d%02d", day, mon, year)
                        if (flag)
                            Toast.makeText(
                                context,
                                "Your date is invalid. Auto formatting...",
                                Toast.LENGTH_SHORT
                            ).show()
                        Log.e("cleanElse", clean)
                        validTime = true
                    }


                    clean = when (clean.length) {
                        2 -> {
                            if (sel == 2) {
                                sel--
                                String.format("%s", clean.substring(0, 1))
                            } else
                                String.format("%s/", clean.substring(0, 2))
                        }
                        3 -> {
                            String.format("%s/%s", clean.substring(0, 2), clean.substring(2, 3))
                        }
                        4 -> {
                            if (sel == 5) {
                                sel--
                                String.format("%s/%s", clean.substring(0, 2), clean.substring(2, 3))
                            } else
                                String.format(
                                    "%s/%s/",
                                    clean.substring(0, 2),
                                    clean.substring(2, 4)
                                )
                        }
                        in 5..8 -> {
                            String.format(
                                "%s/%s/%s",
                                clean.substring(0, 2),
                                clean.substring(2, 4),
                                clean.substring(4, clean.length)
                            )
                        }
                        else -> {
                            clean
                        }
                    }
                    Log.e("cleanFinal", clean)

                    sel = if (sel < 0) 0 else sel
                    Log.e("sel", "$sel")
                    current = clean
                    binding.dobInputEdtTxt.setText(current)
                    binding.dobInputEdtTxt.setSelection(if (sel < current.length) sel else current.length)
                }
            }
        })
    }

    private fun updateProfile() {
        if (validateInformation()) {
            val builder: MultipartBody.Builder = MultipartBody.Builder().setType(MultipartBody.FORM)
            builder.addFormDataPart("lastName", updateProfileParam.lastName)
            builder.addFormDataPart("firstName", updateProfileParam.firstName)
            builder.addFormDataPart("phoneNumber", updateProfileParam.phoneNumber)
            builder.addFormDataPart("email", updateProfileParam.email)
            builder.addFormDataPart("birthDay", updateProfileParam.birthDay)
            builder.addFormDataPart("gender", prefUtil.profile?.gender.toString())
            val mediaType: MediaType = "multipart/form-data".toMediaType()

            if (this::file.isInitialized){
                val requestBody = file.asRequestBody(mediaType)
                val body = MultipartBody.Part.createFormData("file", file.name, requestBody)
                builder.addPart(body)
            }

            viewModel.updateProfile(builder.build())
        }
    }

    private fun validateInformation(): Boolean {
        val firstname = binding.firstNameInputEdtTxt.text.toString().trim()
        val lastname = binding.middleLastInputEdtTxt.text.toString().trim()
        val phoneNumber = binding.phoneInputEdtTxt.text.toString().trim()
        val email = binding.emailInputEdtTxt.text.toString().trim()
        val format = SimpleDateFormat(DATE_TIME_FORMAT, Locale.getDefault())
        val userFormat = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
        var dob = ""
        try {
            val res = userFormat.parse(binding.dobInputEdtTxt.text.toString())

            dob = res?.let { it1 -> format.format(it1) } ?: ""

            if (firstname.isEmpty() || lastname.isEmpty()) {
                AppEvent.showPopUpError(getString(R.string.error_invalid_name))
                return false
            }
            if (email.isEmpty()
                || !android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()
            ) {
                AppEvent.showPopUpError(getString(R.string.email))
                return false
            }
            if (phoneNumber.isEmpty()) {
                AppEvent.showPopUpError(getString(R.string.error_invalid_phone))
                return false
            }
            if (dob.isEmpty() || !validTime) {
                AppEvent.showPopUpError(getString(R.string.dob))
                return false
            }
        } catch (e: Exception) {
            AppEvent.showPopUpError(e.message)
        }
        updateProfileParam = UpdateProfileParam(
            firstname,
            lastname,
            phoneNumber,
            email,
            dob
        )
        return true
    }

    private fun saveBitmapToSDCard(bitmap: Bitmap?): File {
        val dir = context?.getExternalFilesDir(Environment.DIRECTORY_PICTURES)
        val date = Date()
        if (dir?.exists() == false) {
            dir.mkdirs()
        }
        val random = Random()
        val n = random.nextInt(10000)
        val file = File(dir, "IMG_${date.time}_${n}.jpg")
        Log.e("filePath", file.path)
        if (file.exists()) {
            file.delete()
        }
        val fos = FileOutputStream(file)
        bitmap?.compress(Bitmap.CompressFormat.JPEG, 100, fos)
        fos.flush()
        fos.close()
        return file
    }
}