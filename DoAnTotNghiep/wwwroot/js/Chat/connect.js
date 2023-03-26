"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl(window.location.origin + "/ChatHub").build();
var chatRoomData = null;
var listRoom = null;

connection.start().then(function () {
    let connectionid = connection.connection.connectionId;
    //>>>kết nối tất cả phòng => lấy danh sách người dùng kèm 1 tin nhắn mới nhất
    connectServer(connectionid);
}).catch(function (err) {
    return console.error(err.toString());
});

//TARGET t gửi tới 
connection.on("ReceiveMessages", function (message) {
    console.log(message);
    listRoom = message;
    updateNoti(message);
    if (window.location.href.indexOf("Member/Messages") != -1) {
        let tag = ".user-" + message.idRoom;
        console.log($(tag)[0].classList.value.indexOf("active"));
        //>>>nếu trong khung chat
        if ($(tag).length != 0) {
            let idAccess = message.messages[0].idSend;
            let isContinue = false;
            if (chatRoom[message.idRoom].messages.length > 0) {
                isContinue = (chatRoom[message.idRoom].messages[0].idSend == idAccess);
                console.log(isContinue);
            }
            addMessage(message.messages[0], message.idRoom, (userAccess.userAccess == idAccess), isContinue);
            prependNewMessage(tag);
        }
        if (message.idRoom in chatRoom) {
            chatRoom[message.idRoom].messages = [].concat(message.messages, chatRoom[message.idRoom].messages);
        }
        else {
            chatRoom[message.idRoom] = message;
        }
    }
    else {
        let userName = chatRoomData[message.idRoom].userMessages[0].userName;
        showNotification(userName, message.messages[0].message, 1);
    }
});

function connectServer(connectionId) {
    $.ajax({
        url: window.location.origin + '/ConnectAllChat',
        data: JSON.stringify(connectionId),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                chatRoomData = result.data;
                //>>> update thông báo tin nhắn
                intiChatNotification();
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}
function connectServerWithRoom(connectionId, idRoom) {
    let data = {
        connectionId: connectionId,
        idRoom: idRoom
    };

    $.ajax({
        url: window.location.origin + '/Message/ConnectToRoom',
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                if (window.location.href.indexOf("Member/Messages") != -1) {
                    if (idRoom in chatRoom) {
                        chatRoom[idRoom].messages = [].concat(result.data[idRoom].messages, chatRoom[idRoom].messages);
                    }
                    else {
                        chatRoom[idRoom] = result.data[idRoom];
                    }
                }

                //cập nhật chatRoomData
                if (idRoom in chatRoomData) {
                    chatRoomData[idRoom].messages = result.data[idRoom].messages;
                }
                else {
                    chatRoomData[idRoom] = result.data[idRoom];
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
function notiUserName(user, idSend) {
    if (user.userAccess == idSend) {
        return user.userName.trim().split(" ").pop();
    }
    return "Bạn";
}
function htmlChatNotification(user, chat, idRoom) {
    let str = `<a href="${window.location.origin + "/Member/Messages?connectionId=" + user.userAccess}" class="dropdown-item message-${idRoom}">`;
    if (user.imageUrl != null) {
        str += `<div class="avt"><img src="${user.imageUrl}" alt="avt" /></div>`;
    }
    else {
        str += `<div class="avt"><img src="${window.location.origin + "/Image/user.svg"}" alt="avt" /></div>`;
    }
    str += `<div class="content"><strong>${user.userName}</strong><small> đã gửi tin nhắn</small>
            <div style="margin: 6px 0;" class="message-${idRoom}"><small><strong>${notiUserName(user, chat.idSend)}</strong></small>: ${chat.message}</div >
            <small class="time">${chat.createdDate}</small></div>`;
    if (!chat.isSeen) {
        str += `<div class="status"><i class="fa-solid fa-circle"></i></div>`;
    }
    str += `</a>`;
    return str;
}
function showNewSignal(tag_element) {
    $(tag_element + " span").css("opacity", 1);
}
function hideNewSignal(tag_element) {
    $(tag_element + " span").css("opacity", 0);
}
function intiChatNotification() {
    let isNew = false;
    //nếu tất cả là true => 
    for (let e in chatRoomData) {
        if (chatRoomData[e].messages.length > 0) {
            if (chatRoomData[e].messages.isSeen == false) {
                isNew = true;
                break;
            }
        }
    }

    //thêm chat
    for (let e in chatRoomData) {
        if (chatRoomData[e].messages.length > 0) {
            $("#tag_chats .alert-frame").append(htmlChatNotification(chatRoomData[e].userMessages[0], chatRoomData[e].messages[0], chatRoomData[e].idRoom));
        }
    }
    if (isNew == true) {
        showNewSignal("#tag_chats #dropdownMenuChat");
    }
    else {
        hideNewSignal("#tag_chats #dropdownMenuChat");
    }
}
function prependNewNotification(tag) {
    let temp = $(tag);
    $(tag).remove();
    $("#tag_chats .alert-frame").prepend(temp);
    showNewSignal("#tag_chats #dropdownMenuChat");
}
function updateNoti(data) {
    //cập nhật lại chatRoomData
    if (chatRoomData != null) {
        if (data.idRoom in chatRoomData) {
            chatRoomData[data.idRoom].messages = data.messages;
        }
        else {
            chatRoomData[data.idRoom] = data;
        }
    }
    else {
        chatRoomData = {};
        chatRoomData[data.idRoom] = data;
    }
    
    console.log("Hy");
    console.log(data);
    let tag_message = ".message-" + data.idRoom;

    $(".dropdown-item" + tag_message + " " + tag_message)
        .html(`<div style="margin: 6px 0;" class="message message-${data.idRoom}"><small><strong>${notiUserName(data.userMessages[0], data.messages[0].idSend)}</strong></small>: ${data.messages[0].message}</div >`);
    prependNewNotification(".dropdown-item" + tag_message);
}