package com.example.homex.activity.auth

import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.widget.ArrayAdapter
import android.widget.Toast
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUpdateInformationBinding
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.CoreApplication
import com.homex.core.param.auth.UpdateInfoParam
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.text.SimpleDateFormat
import java.util.*


class UpdateInformationFragment : BaseFragment<FragmentUpdateInformationBinding>() {
    override val layoutId: Int = R.layout.fragment_update_information
    private val viewModel: AuthViewModel by viewModel()
    private var pos = -1
    private var validTime = true


    override fun setView() {
        val items = listOf("Nam", "Nữ")
        val adapter = ArrayAdapter(requireContext(), R.layout.sex_item, items)
        binding.autoCompleteTV.setAdapter(adapter)
    }

    override fun setEvent() {
        binding.autoCompleteTV.setOnItemClickListener { _, view, position, _ ->
            pos = position
        }
        binding.btnContinue.setOnClickListener {
            val firstname = binding.firstnameInputEdtTxt.text.toString().trim()
            val lastname = binding.lastnameInputEdtTxt.text.toString().trim()
            val sex = pos != 0
            val phoneNumber = binding.phoneInputEdtTxt.text.toString().trim()
            val format = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.getDefault())
            format.timeZone = TimeZone.getTimeZone("UTC")
            val userFormat = SimpleDateFormat("dd/MM/yyyy", Locale.getDefault())
            userFormat.timeZone = TimeZone.getTimeZone("Asia/Vietnam")
            try {
                val res = userFormat.parse(binding.dobInputEdtTxt.text.toString())

                val dob = res?.let { it1->format.format(it1) } ?:""

                if(firstname.isEmpty() || lastname.isEmpty() || pos == -1 || dob.isEmpty() || !validTime){
                    AppEvent.showPopUpError(message = "Hãy điền thông tin chính xác")
                    return@setOnClickListener
                }

                val prof = UpdateInfoParam(
                    firstName = firstname,
                    lastName = lastname,
                    gender = sex,
                    birthDay = dob,
                    phoneNumber = phoneNumber
                )
                viewModel.updateInformation(prof)
            }catch (e: Exception){
                AppEvent.showPopUpError(e.message)
            }
        }

        binding.dobInputEdtTxt.addTextChangedListener(object : TextWatcher{
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
            override fun afterTextChanged(s: Editable?) {}

            private var current: String = ""
            private var cal = Calendar.getInstance()

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
                Log.e("s", s.toString())
                Log.e("current", current)
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
                    if (clean.length >= 8)
                    {
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

                        if(cal.timeInMillis > Calendar.getInstance().timeInMillis){
                            flag = true
                            cal = Calendar.getInstance()
                            day = cal.get(Calendar.DAY_OF_MONTH)
                            mon = cal.get(Calendar.MONTH) + 1
                            year = cal.get(Calendar.YEAR)
                        }

                        clean = String.format("%02d%02d%02d", day, mon, year)
                        if (flag)
                            Toast.makeText(context, "Your date is invalid. Auto formatting...", Toast.LENGTH_SHORT).show()
                        Log.e("cleanElse",clean)
                        validTime = true
                    }


                    clean = when (clean.length) {
                        2 -> {
                            if(sel == 2) {
                                sel--
                                String.format("%s", clean.substring(0, 1))
                            }
                            else
                                String.format("%s/", clean.substring(0,2))
                        }
                        3 -> {
                            String.format("%s/%s", clean.substring(0,2), clean.substring(2,3))
                        }
                        4 -> {
                            if(sel == 5)
                            {
                                sel--
                                String.format("%s/%s", clean.substring(0,2), clean.substring(2,3))
                            }
                            else
                                String.format("%s/%s/", clean.substring(0,2), clean.substring(2,4))
                        }
                        in 5..8 -> {
                            String.format("%s/%s/%s", clean.substring(0, 2), clean.substring(2, 4), clean.substring(4, clean.length))
                        }
                        else -> {clean}
                    }
                    Log.e("cleanFinal",clean)

                    sel = if (sel < 0) 0 else sel
                    Log.e("sel", "$sel")
                    current = clean
                    binding.dobInputEdtTxt.setText(current)
                    binding.dobInputEdtTxt.setSelection(if (sel < current.length) sel else current.length)
                }
            }
        })
    }

    override fun setViewModel() {
        viewModel.userInfoLiveData.observe(viewLifecycleOwner){ user->
            CoreApplication.instance.saveToken(user?.token)
            CoreApplication.instance.saveProfile(user?.userInfo)
            activity?.finishAffinity()
            startActivity(HomeActivity.open(requireContext()))
        }
    }
}