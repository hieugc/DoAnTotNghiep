"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl(window.location.origin + "/ChatHub").build();
var chatRoomData = {}
var listRoom = null;


connection.start().then(function () {
    if ($.cookie("ChatRoom") != undefined && $.cookie("ChatRoom").length > 0) {
        listRoom = $.cookie("ChatRoom").split(",");
        let data = { connectionId: connection.connectionId };
        connectServer(data, 0, 1);
    }
    else {
        $.ajax({
            url: window.location.origin + '/ChatRoom',
            contentType: "application/json",
            type: "GET",
            success: function (result) {
                console.log(result);
                if (result.status == 200) {
                    listRoom = result.data;
                    let data = { connectionId: connection.connectionId };
                    connectServer(data, 0, 1);
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                console.log(status);
                console.log(error);
            }
        });
    }
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessages", function (message) {
    if (window.location.href.indexOf("Member/Messages") != -1) {
        //nếu đang trong message =>
        console.log(message);
        let tag = ".user-" + message.idRoom;
        console.log($(tag));
        if ($(tag)[0].classList.value.indexOf("active") != -1) {
            let idAccess = message.message.idSend;
            let isContinue = true;
            if (chatRoomData[message.idRoom].messages.length > 0) {
                isContinue = (chatRoomData[message.idRoom].messages[0].idSend == idAccess);
                console.log(isContinue);
            }
            //show chat
            addMessage(message.message, message.idRoom, (userAccess.userAccess == idAccess), isContinue);
        }
        chatRoomData[message.idRoom].messages.unshift(message.message);
        $(tag).addClass("new-message");
        console.log(message);
    }
    else {
        //không trong message show notification =>
        let userName = chatRoomData[message.idRoom].userMessages[0].userName;
        showNotification(userName, message.message.message, 1);
        chatRoomData[message.idRoom].messages.unshift(message.message);
        console.log(message);
    }
    //ghi lại cookie =>
    /* Cập nhật cho notification => show 1 chat */
    let data = {
        idRoom: message.idRoom,
        userMessages: chatRoomData[message.idRoom].userMessages,
        messages: [chatRoomData[message.idRoom].messages[0]]
    };
    $.ajax({
        url: window.location.origin + '/UpdateCookie',
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
});

function connectServer(data, start, length) {
    $.ajax({
        url: window.location.origin + '/Connect',
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                getChatRoomData();
                for (let e in listRoom) {
                    if (chatRoomData[listRoom[e]] == undefined) {
                        let data = {
                            idRoom: listRoom[e],
                            rangeRoom: {
                                start: start,
                                length: length
                            }
                        }
                        if (data.idRoom != "" && parseInt(data.idRoom) != NaN) {
                            getMessage(data, null);
                        }
                    }
                }
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}
function getMessage(data, func) {
    $.ajax({
        url: window.location.origin + '/MessagesInChatRoom',
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            getChatRoomData();
            if (func != null && func != undefined) {
                func;
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}
function getChatRoomData() {
    let string = $.cookie("DataChat");
    if (string != null && string != undefined && string.indexOf("{") != -1) {
        chatRoomData = JSON.parse(string.replaceAll("ImageUrl", "imageUrl")
            .replaceAll("IdRoom", "idRoom")
            .replaceAll("User", "user")
            .replaceAll("Message", "message")
            .replaceAll("usermessage", "userMessage")
            .replaceAll("IsSeen", "isSeen")
            .replaceAll("Id", "id")
            .replaceAll("CreatedDate", "createdDate"));
    }
}