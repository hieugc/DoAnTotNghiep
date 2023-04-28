package com.example.homex.activity.webview

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.webkit.WebView
import android.webkit.WebViewClient
import androidx.databinding.DataBindingUtil
import com.example.homex.R
import com.example.homex.base.BaseActivity
import com.example.homex.databinding.ActivityWebviewBinding


class WebviewActivity : BaseActivity() {
    private lateinit var binding: ActivityWebviewBinding

    companion object{
        fun open(context: Context) = Intent(context, WebviewActivity::class.java)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_webview)

        initListener()
        processWebView()
    }

    private fun initListener() {
        binding.btnBack.setOnClickListener{
            if(binding.webView.canGoBack()){
                binding.webView.goBack()
            } else {
                finish()
            }
        }

        binding.btnClose.setOnClickListener{
            finish()
        }
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun processWebView() {
        binding.webView.settings.javaScriptEnabled = true
        binding.webView.settings.domStorageEnabled = true
        binding.webView.settings.mediaPlaybackRequiresUserGesture = true
        binding.webView.settings.allowContentAccess = true
        binding.webView.webViewClient = object : WebViewClient() {
            override fun shouldOverrideUrlLoading(view: WebView, url: String): Boolean {
                return if(url.contains("http")) {
                    view.loadUrl(url)
                    true
                } else {
                    false
                }
            }
        }
        val url = intent.getStringExtra("redirect_url")
        if(url?.isNotEmpty() == true){
            binding.webView.loadUrl(url)
        }
    }
}