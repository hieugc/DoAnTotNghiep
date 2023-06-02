package com.example.homex.activity.home.search

import android.content.Context
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.text.Editable
import android.text.TextWatcher
import android.view.WindowManager
import android.view.inputmethod.InputMethodManager
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.RecentSearchAdapter
import com.example.homex.app.SUGGEST
import com.example.homex.databinding.FragmentBottomSheetSearchBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.utils.CustomBottomSheet
import com.example.homex.viewmodel.HomeViewModel
import com.homex.core.model.LocationSuggestion
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.Timer
import java.util.TimerTask


class BottomSheetSearchFragment : CustomBottomSheet<FragmentBottomSheetSearchBinding>() {
    override val layoutId: Int
        get() = R.layout.fragment_bottom_sheet_search
    private lateinit var adapter: RecentSearchAdapter
    private val searchList: ArrayList<LocationSuggestion> = arrayListOf()
    private val viewModel: HomeViewModel by viewModel()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        viewModel.suggestion.observe(this){
            if (it != null){
                searchList.clear()
                searchList.addAll(it)
                adapter.notifyDataSetChanged()
            }
            binding.progressIndicator.gone()
        }
    }

    override fun setView() {
        showKeyboard()
        adapter = RecentSearchAdapter(searchList, false, onClick = {
            findNavController().previousBackStackEntry?.savedStateHandle?.set(SUGGEST, it)
            findNavController().popBackStack()
        }, deleteOnClick = {})
        binding.rvSuggestion.adapter = adapter
        binding.rvSuggestion.layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.rvSuggestion.setHasFixedSize(true)
    }

    override fun setEvent() {
        binding.searchEdtTxt.addTextChangedListener(object : TextWatcher{
            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
                if(p0.toString().trim() != "")
                {
                    searchList.clear()
                    adapter.notifyDataSetChanged()
                }
            }
            private val delay: Long = 1000
            private var timer = Timer()
            override fun afterTextChanged(p0: Editable?) {
                timer.cancel()
                timer = Timer()
                showProgress()
                timer.schedule(object : TimerTask(){
                    override fun run() {
                        Handler(Looper.getMainLooper()).post {
                            val q = p0?.toString()?.trim()
                            if (q != null && q != ""){
                                viewModel.getSuggestion(q)
                            }
                            else{
                                searchList.clear()
                                adapter.notifyDataSetChanged()
                                binding.progressIndicator.gone()

                            }
                        }
                    }
                }, delay)
            }
        })
        binding.closeBtn.setOnClickListener {
            dismiss()
        }
    }

    private fun showProgress(){
        binding.progressIndicator.visible()
    }

    private fun showKeyboard() {
        dialog?.window?.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        binding.searchEdtTxt.requestFocus()
        val imm =
            activity?.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
        imm.showSoftInput(binding.searchEdtTxt, 0)
    }
}