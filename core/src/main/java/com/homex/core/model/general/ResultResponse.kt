package com.homex.core.model.general

sealed class ResultResponse<out R> {

    data class Success<out T>(val data: T? = null, val message: String? = ""): ResultResponse<T>()

    data class Error(val message: String, val code: Int?) : ResultResponse<Nothing>()
    object Loading : ResultResponse<Nothing>()
    object LoadingMore : ResultResponse<Nothing>()

    override fun toString(): String {
        return when (this) {
            is Success<*> -> "Success[data=$data]"
            is Error -> "Error[ErrorCode = $code -- message = $message]"
            Loading -> "Loading"
            LoadingMore -> "Loading more"
        }
    }
}