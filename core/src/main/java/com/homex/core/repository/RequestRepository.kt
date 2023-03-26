package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.model.response.RequestResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.param.request.EditRequestParam
import com.homex.core.param.request.UpdateStatusParam
import okhttp3.RequestBody

interface RequestRepository {
    suspend fun createNewRequest(param: CreateRequestParam): LiveData<ResultResponse<JsonObject>>

    suspend fun deleteRequest(body: RequestBody): LiveData<ResultResponse<JsonObject>>

    suspend fun updateRequest(param: EditRequestParam): LiveData<ResultResponse<JsonObject>>

    suspend fun getRequestById(id: Int): LiveData<ResultResponse<RequestResponse>>

    suspend fun getRequestByHouse(id: Int): LiveData<ResultResponse<ArrayList<RequestResponse>>>

    suspend fun getRequestSent(): LiveData<ResultResponse<ArrayList<RequestResponse>>>

    suspend fun getPendingRequest(): LiveData<ResultResponse<ArrayList<RequestResponse>>>

    suspend fun updateStatus(param: UpdateStatusParam): LiveData<ResultResponse<JsonObject>>
}