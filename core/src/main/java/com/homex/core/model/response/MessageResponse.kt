package com.homex.core.model.response

import com.homex.core.model.MessageRoom


class MessageResponse(
    val rooms: ArrayList<MessageRoom>? = null,
    val metaData: Meta? = null
)

class Meta(
    val pageCount: Int? = null,
    val count: Int? = null,
    val totalCount : Int? = null,
)