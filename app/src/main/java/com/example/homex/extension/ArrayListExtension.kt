package com.example.homex.extension

import android.app.Dialog
import android.view.View
import com.homex.core.model.Message

inline fun ArrayList<(view: View?, dialog: Dialog?)->Unit>.addMessage(crossinline function: (view: View?, dialog: Dialog?)->Unit){
    this.plusAssign{view, dialog->
        function(view,dialog)
    }
}

fun ArrayList<Message>.filterTime(): ArrayList<Message> {
    val tmp = ArrayList(this.filter { it.isDateItem != null })
    if (tmp.isNotEmpty()){
        val tmpList = arrayListOf<Message>()
        var date = tmp[0].createdDate
        for((index, msg) in tmp.withIndex()){
            if(date?.formatIso8601ToFormat("dd/MM/yyyy") != msg.createdDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                tmpList.add(
                    Message(
                        createdDate = date,
                        isDateItem = true
                    )
                )
                date = msg.createdDate
            }
            tmpList.add(msg)
            if(index == tmp.size - 1){
                tmpList.add(
                    Message(
                        createdDate = date,
                        isDateItem = true
                    )
                )
            }
        }
        return tmpList
    }
    return tmp
}