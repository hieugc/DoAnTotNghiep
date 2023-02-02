package com.example.homex.extension

import android.app.Dialog
import android.view.View

inline fun ArrayList<(view: View?, dialog: Dialog?)->Unit>.addMessage(crossinline function: (view: View?, dialog: Dialog?)->Unit){
    this.plusAssign{view, dialog->
        function(view,dialog)
    }
}