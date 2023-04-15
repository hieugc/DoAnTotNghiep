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
    REJECTED,
    WAITING,
    ACCEPTED,
    CHECKIN,
    REVIEWING,
    DONE
}

enum class RequestType{
    NONE,
    BY_POINT,
    BY_HOME
}
