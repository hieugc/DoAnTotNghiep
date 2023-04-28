package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.PaymentInfoResponse
import com.homex.core.param.auth.PasswordParam
import com.homex.core.param.profile.TopUpPointParam
import okhttp3.RequestBody

interface ProfileRepository {
    suspend fun updatePassword(param: PasswordParam): LiveData<ResultResponse<JsonObject>>

    suspend fun updateProfile(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>>

    suspend fun topUpPoint(
        param: TopUpPointParam
    ): LiveData<ResultResponse<PaymentInfoResponse>>
}
