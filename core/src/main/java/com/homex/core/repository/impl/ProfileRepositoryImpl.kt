package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.PaymentHistory
import com.homex.core.model.response.PaymentInfoResponse
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.auth.PasswordParam
import com.homex.core.param.profile.TopUpPointParam
import com.homex.core.repository.ProfileRepository
import okhttp3.RequestBody
import retrofit2.Response

class ProfileRepositoryImpl(val api: ApiService): ProfileRepository {
    override suspend fun updatePassword(param: PasswordParam): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.updateNewPassword(param)
        }.build().asLiveData()
    }

    override suspend fun updateProfile(body: RequestBody): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.updateProfile(body)
        }.build().asLiveData()
    }

    override suspend fun topUpPoint(param: TopUpPointParam): LiveData<ResultResponse<PaymentInfoResponse>> {
        return object : NetworkBoundResource<ObjectResponse<PaymentInfoResponse>, PaymentInfoResponse>(){
            override fun processResponse(response: ObjectResponse<PaymentInfoResponse>): PaymentInfoResponse? = response.data
            override suspend fun createCall(): Response<ObjectResponse<PaymentInfoResponse>> = api.topUpPoint(param)
        }.build().asLiveData()
    }

    override suspend fun getPoint(): LiveData<ResultResponse<Long>> {
        return object : NetworkBoundResource<ObjectResponse<Long>, Long>(){
            override fun processResponse(response: ObjectResponse<Long>): Long? = response.data
            override suspend fun createCall(): Response<ObjectResponse<Long>> = api.getPoint()
        }.build().asLiveData()
    }

    override suspend fun getHistoryAll(): LiveData<ResultResponse<ArrayList<PaymentHistory>>> {
            return object : NetworkBoundResource<ListResponse<PaymentHistory>, ArrayList<PaymentHistory>>(){
                override fun processResponse(response: ListResponse<PaymentHistory>): ArrayList<PaymentHistory>? = response.data
                override suspend fun createCall(): Response<ListResponse<PaymentHistory>> = api.getHistoryAll()
            }.build().asLiveData()
    }

    override suspend fun getHistoryReceived(): LiveData<ResultResponse<ArrayList<PaymentHistory>>> {
        return object : NetworkBoundResource<ListResponse<PaymentHistory>, ArrayList<PaymentHistory>>(){
            override fun processResponse(response: ListResponse<PaymentHistory>): ArrayList<PaymentHistory>? = response.data
            override suspend fun createCall(): Response<ListResponse<PaymentHistory>> = api.getHistoryReceived()
        }.build().asLiveData()
    }

    override suspend fun getHistoryUsed(): LiveData<ResultResponse<ArrayList<PaymentHistory>>> {
        return object : NetworkBoundResource<ListResponse<PaymentHistory>, ArrayList<PaymentHistory>>(){
            override fun processResponse(response: ListResponse<PaymentHistory>): ArrayList<PaymentHistory>? = response.data
            override suspend fun createCall(): Response<ListResponse<PaymentHistory>> = api.getHistoryUsed()
        }.build().asLiveData()
    }

}