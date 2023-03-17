package com.homex.core.param.chat

class SendMessageParam(
    val message: String,
    val idReply: Int = 0,
    val idRoom: Int
)