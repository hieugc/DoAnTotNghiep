package com.example.homex.activity.home.profile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.PaymentHistoryAdapter
import com.example.homex.app.PAYMENT_STATUS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPaymentViewPagerBinding
import com.example.homex.extension.Payment
import com.example.homex.viewmodel.ProfileViewModel
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.sharedViewModel

class PaymentViewPagerFragment : BaseFragment<FragmentPaymentViewPagerBinding>() {
    override val layoutId: Int = R.layout.fragment_payment_view_pager
    private var paymentStatus: Int = Payment.ALL.ordinal
    private val viewModel: ProfileViewModel by sharedViewModel()
    private lateinit var adapter: PaymentHistoryAdapter

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        binding = FragmentPaymentViewPagerBinding.inflate(layoutInflater)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        arguments?.takeIf { it.containsKey(PAYMENT_STATUS) }?.apply {
            paymentStatus = getInt(PAYMENT_STATUS)
        }

        when(paymentStatus){
            Payment.ALL.ordinal -> viewModel.getHistoryAll()
            Payment.VALID.ordinal -> viewModel.getHistoryReceived()
            Payment.USED.ordinal -> viewModel.getHistoryUsed()
        }
    }

    override fun setView() {
        adapter = PaymentHistoryAdapter(requireContext())
        binding.rvPaymentHis.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.rvPaymentHis.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.paymentHistoryAllLiveData.observe(viewLifecycleOwner){
            if(it != null){
                if(paymentStatus == Payment.ALL.ordinal) {
                    adapter.setList(it)
                }
            }
            AppEvent.closePopup()
        }

        viewModel.paymentHistoryReceivedLiveData.observe(viewLifecycleOwner){
            if(it != null){
                if(paymentStatus == Payment.VALID.ordinal) {
                    adapter.setList(it)
                }
            }
            AppEvent.closePopup()
        }


        viewModel.paymentHistoryUsedLiveData.observe(viewLifecycleOwner){
            if(it != null){
                if(paymentStatus == Payment.USED.ordinal) {
                    adapter.setList(it)
                }
            }
            AppEvent.closePopup()
        }

    }

}