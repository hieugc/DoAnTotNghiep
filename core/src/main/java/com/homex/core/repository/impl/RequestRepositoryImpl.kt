package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.response.RequestResponse
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.param.request.EditRequestParam
import com.homex.core.param.request.UpdateStatusParam
import com.homex.core.repository.RequestRepository
import okhttp3.RequestBody
import retrofit2.Response

class RequestRepositoryImpl(private val api: ApiService): RequestRepository {
    override suspend fun createNewRequest(param: CreateRequestParam): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? =response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.createNewRequest(param)
        }.build().asLiveData()
    }

    override suspend fun deleteRequest(body: RequestBody): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? =response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.deleteRequest(body)
        }.build().asLiveData()
    }

    override suspend fun updateRequest(param: EditRequestParam): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? =response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.editRequest(param)
        }.build().asLiveData()
    }

    override suspend fun getRequestById(id: Int): LiveData<ResultResponse<RequestResponse>> {
        return object : NetworkBoundResource<ObjectResponse<RequestResponse>, RequestResponse>(){
            override fun processResponse(response: ObjectResponse<RequestResponse>): RequestResponse? = response.data
            override suspend fun createCall(): Response<ObjectResponse<RequestResponse>> = api.getRequestById(id)
        }.build().asLiveData()
    }

    override suspend fun getRequestByHouse(id: Int): LiveData<ResultResponse<ArrayList<RequestResponse>>> {
        return object : NetworkBoundResource<ListResponse<RequestResponse>, ArrayList<RequestResponse>>(){
            override fun processResponse(response: ListResponse<RequestResponse>): ArrayList<RequestResponse>? = response.data
            override suspend fun createCall(): Response<ListResponse<RequestResponse>> = api.getRequestByHouse(id)
        }.build().asLiveData()
    }

    override suspend fun getRequestSent(): LiveData<ResultResponse<ArrayList<RequestResponse>>> {
        return object : NetworkBoundResource<ListResponse<RequestResponse>, ArrayList<RequestResponse>>(){
            override fun processResponse(response: ListResponse<RequestResponse>): ArrayList<RequestResponse>? = response.data
            override suspend fun createCall(): Response<ListResponse<RequestResponse>> = api.getRequestSent()
        }.build().asLiveData()
    }

    override suspend fun getPendingRequest(): LiveData<ResultResponse<ArrayList<RequestResponse>>> {
        return object : NetworkBoundResource<ListResponse<RequestResponse>, ArrayList<RequestResponse>>(){
            override fun processResponse(response: ListResponse<RequestResponse>): ArrayList<RequestResponse>? = response.data
            override suspend fun createCall(): Response<ListResponse<RequestResponse>> = api.getPendingRequest()
        }.build().asLiveData()
    }

    override suspend fun updateStatus(param: UpdateStatusParam): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.updateStatus(param)
        }.build().asLiveData()
    }
}