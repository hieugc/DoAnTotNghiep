package com.homex.core.model.general

import com.google.gson.annotations.SerializedName

open class ObjectResponse<T>(
    val metadata: MetaData? = null,
    val code: String? = null,
    @SerializedName(value = "detail", alternate = ["message"])
    val detail: String? = null,
    @SerializedName(value = "status", alternate = ["statusCode"])
    val statusCode: Int? = null,
    @SerializedName(value = "data", alternate = ["results"])
    val data: T? = null
)
