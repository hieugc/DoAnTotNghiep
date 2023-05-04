package com.example.homex.extension

enum class Utilities{
    WIFI,
    COMPUTER,
    TV,
    BATHTUB,
    PARKING_LOT,
    AIR_CONDITIONER,
    WASHING_MACHINE,
    FRIDGE,
    POOL
}

enum class Rules{
    NO_SMOKING,
    NO_PET
}

enum class RequestStatus{
    WAITING,
    ACCEPTED,
    REJECTED,
    CHECK_IN,
    REVIEWING,
    DONE
}

enum class RequestType{
    NONE,
    BY_POINT,
    BY_HOME
}

enum class SearchBy{
    CLOSET,
    RATING,
    PRICE_LOW_TO_HIGH,
    PRICE_HIGH_TO_LOW
}

enum class Payment{
    ALL,
    VALID,
    USED
}

enum class NotificationType{
    REQUEST,
    COMMENT,
    RESPONSE,
    ADMIN_REPORT,
    MESSAGE,
    CIRCLE_SWAP,
    PAYMENT
}
