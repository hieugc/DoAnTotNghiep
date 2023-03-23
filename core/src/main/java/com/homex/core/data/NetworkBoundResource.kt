package com.homex.core.data

import android.util.Log
import androidx.annotation.MainThread
import androidx.annotation.WorkerThread
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.google.gson.Gson
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.util.AppEvent
import kotlinx.coroutines.CoroutineDispatcher
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import retrofit2.Response

/**
 * Constructs [NetworkBoundResource]
 *
 * @param dispatcher
 *            the Coroutines will lunch at Dispatchers.IO Worker Thread.
 * @param RequestType
 *            the type Input data.
 * @param ResultResponseType
 *            the type Output data.
 */

abstract class NetworkBoundResource<RequestType, ResultResponseType>
constructor(private val dispatcher: CoroutineDispatcher = Dispatchers.IO) {

    private val TAG = NetworkBoundResource::class.java.name

    private val result = MutableLiveData<ResultResponse<ResultResponseType>>()

    /**
     *  Init this abstract class [build].
     */
    fun build(): NetworkBoundResource<RequestType, ResultResponseType> {

        result.value = ResultResponse.Loading


        CoroutineScope(dispatcher).launch {

            val dbResultResponse = loadFromDb()

            if (shouldFetch(dbResultResponse)) {
                try {
                    fetchFromNetwork()
                } catch (e: java.lang.Exception) {
                    Log.e(TAG, "An error happened: $e")
                    setValue(ResultResponse.Error(e.message ?: "", 404))
                    AppEvent.showPopUpError(e.message)
                }
            } else {
                Log.d(TAG, "Return data from local database")
                if (dbResultResponse != null)
                    setValue(ResultResponse.Success(dbResultResponse))
            }

        }
        return this
    }

    @MainThread
    private fun setValue(newValue: ResultResponse<ResultResponseType>) {
        if (result.value != newValue) {
            result.postValue(newValue)
        }
    }

    /**
     * This Func handle response from Network [fetchFromNetwork].
     */
    private suspend fun fetchFromNetwork() {
        try{
            Log.i(TAG, "Fetch data from network")

            val apiResponse = createCall()

            Log.i(TAG, "Data fetched from network")

            if (apiResponse.isSuccessful) {
                val body = apiResponse.body()
                Log.e("body", body.toString())
                when (apiResponse.code()) {
                    200, 201, 204 -> {
                        body?.let {
                            val message = ""
                            if (it == null) {
                                setValue(ResultResponse.Success(it, message))
                            } else {
                                saveCallResultResponse(processResponse(it))
                                val result = loadFromDb() ?: processResponse(it)
                                setValue(ResultResponse.Success(result))
                            }

                        }
                        if (body == null) {
                            setValue(ResultResponse.Success(null, apiResponse.message()))
                        }
                        Log.e("apiSuccess", body.toString())
                    }
                    else -> {
                        Log.e("apiError", body.toString())
                        setValue(ResultResponse.Error(apiResponse.message(), apiResponse.code()))
                        AppEvent.showPopUpError(message = apiResponse.message().toString())
                    }
                }
            } else {
                if(apiResponse.code() == 502 || apiResponse.code().toString() == "502")
                {
                    val errorMsg = "502 Bad Gateway"
                    setValue(ResultResponse.Error(errorMsg, apiResponse.code()))
                    AppEvent.showPopUpError(errorMsg)
                }else {
                    val response =
                        Gson().fromJson(
                            apiResponse.errorBody()?.string(),
                            ObjectResponse::class.java
                        )
                    val errorMsg = response?.detail ?: ""
                    setValue(ResultResponse.Error(errorMsg, apiResponse.code()))
                    if (response.detail != "Invalid email or password") {
                        AppEvent.showPopUpError(errorMsg)
                    }
                }
            }
        }catch (e: Exception){
            AppEvent.showPopUpError(message = e.localizedMessage)
        }

    }

    /**
     * Return a LiveData [asLiveData].
     */
    fun asLiveData(): LiveData<ResultResponse<ResultResponseType>> = result

    /**
     * This Func handle data before return to View [processResponse].
     */
    @WorkerThread
    protected abstract fun processResponse(response: RequestType): ResultResponseType?

    /**
     * Save data into database [saveCallResultResponse] (Optional).
     */
    @WorkerThread
    protected open suspend fun saveCallResultResponse(item: ResultResponseType?) {
    }

    /**
     * Want to load data from api or database [shouldFetch].
     * If [loadFromDb] return a [result] -> [shouldFetch] is false
     */
    @MainThread
    protected open fun shouldFetch(data: ResultResponseType?): Boolean = true

    /**
     * Load data from database [loadFromDb] (Optional).
     */
    @MainThread
    protected open suspend fun loadFromDb(): ResultResponseType? = null

    /**
     * Call data from network [createCall] (Required).
     */
    @MainThread
    protected abstract suspend fun createCall(): Response<RequestType>
}
