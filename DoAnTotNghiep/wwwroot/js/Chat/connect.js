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
connection.on("all", function (message) {
    console.log(message);
});
connection.on("Notification", function (model) {
    console.log(model);
    showNewSignal("#tag_notification #dropdownMenuNotification");
    $("#tag_notification .alert-frame").prepend(itemNoti(model));
    showNotification(model.title, model.content, 1);
    if (model.type == 0) {
        if (window.location.href.indexOf("Member/House") != -1) {
            updateRequestInHouse();
        }
    }
    else if (model.type == 6) {
        if (window.location.href.indexOf("Member/History") != -1) {
            if ($("#renderQRCode").length > 0) {
                $("#renderQRCode").html(notiPaymentSuccess());
            }
        }
        updatePointUser();
    }
});

connection.on("Payment", function (message) {
    console.log(message);
    if (window.location.href.indexOf("Member/History") != -1) {
        //show successfull
        if ($("#renderQRCode").length > 0) {
            $("#renderQRCode").html(notiPaymentFail(message));
        }
    }
    else {
        showNotification("Thanh toán thất bại", message, 0);
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
                //updateThôngbao
                getNotification("#tag_notification");
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
            <small class="time">${chat.createdDate.split("T")[1].split(".")[0]} ${dateFormat(chat.createdDate.split("T")[0])}</small></div>`;
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
    let tag_message = ".message-" + data.idRoom;

    $(".dropdown-item" + tag_message + " " + tag_message)
        .html(`<div style="margin: 6px 0;" class="message message-${data.idRoom}"><small><strong>${notiUserName(data.userMessages[0], data.messages[0].idSend)}</strong></small>: ${data.messages[0].message}</div >`);
    prependNewNotification(".dropdown-item" + tag_message);
}
function returnFunction(model) {
    if (model.type == 0 || model.type == 1) {//REQUEST_0 + RATING_1
        return `requestView(${model.idType}, ${model.id})`;
    }
    else if (model.type == 2) { //ADMIN_REPORT_2
        return ``;//chưa có api
    }
    else if (model.type == 4 || model.type == 7) {//CIRCLE_SWAP_4 CIRCLE_RATING_7
        return `circleRequestView(${model.idType}, ${model.id})`;
    }
    else if (model.type == 6) { //PAYMENT_6
        return `paymentView(${model.idType}, ${model.id})`;
    }
}
function itemNoti(model) {
    let res = `<div onclick="${returnFunction(model)}" class="dropdown-item alert-noti-${model.id}">
                    <div class="avt"><img src="${model.imageUrl}" alt="avt" /></div>
                    <div class="content">
                        <div>
                            <span class="title">${model.title}</span>
                            <span>${model.content}</span>
                        </div>
                        <span class="time">${model.createdDate.split("T")[1].split(".")[0]} ${dateFormat(model.createdDate.split("T")[0])}</span>
                    </div>`;
    if (!model.isSeen) {
        res += `<div class="status"><i class="fa-solid fa-circle"></i></div>`;
    }
    res += `</div>`;
    return res;
}
function dateFormat(date) {
    let ele = date.split("-");
    return ele[2] + "/" + ele[1] + "/" + ele[0]; 
}
function initNoti(tag, list) {
    hideNewSignal(tag + " #dropdownMenuNotification");
    let isNew = false;
    for (let e in list) {
        if (list[e].isSeen == false) {
            isNew = true;
            break;
        }
    }

    //thêm chat
    for (let e in list) {
        $(tag + " .alert-frame").append(itemNoti(list[e]));
    }
    if (isNew == true) {
        showNewSignal(tag + " #dropdownMenuNotification");
    }
}
function getNotification(tag) {
    $.ajax({
        url: window.location.origin + '/Notification/Get',
        contentType: "application/json",
        type: "GET",
        success: function (result) {
            if (result.status == 200) {
                initNoti(tag, result.data.model);
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}
function notiPaymentSuccess() {
    let res =   `<div class="zaloPay-notification">
                    <div><i class="fa-solid fa-circle-check"></i></div>
                    <div><h3>Bạn đã thanh toán thành công!!</h3></div>
                </div>`
    return res;
}
function notiPaymentFail(message) {
    let res = `<div class="zaloPay-notification">
                    <div><i class="fa-solid fa-circle-exclamation"></i></div>
                    <div><h3>${message}!!</h3></div>
                </div>`
    return res;
}
function createModal(html) {
    let res = `
            <div class="modal fade" id="Notification-itemModal" data-bs-backdrop="static" data-bs-keyboard="false" aria-hidden="true"tabindex="-1">
                <div class="modal-dialog modal-dialog-centered modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5">Thông tin thông báo</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" id="Notification-itemModalClose"></button>
                        </div>
                        <div class="modal-body">
                            ${html}
                        </div>
                    </div>
                </div>
            </div>
            <button id="NotificationModalToggleClick" style="display: none;" data-bs-target="#Notification-itemModal" data-bs-toggle="modal"></button>`;
    return res;
}
function requestView(idType, id) {
    $.get(
        window.location.origin + "/Request/Detail?Id=" + idType,
        function (data) {
            updatePopupNotification(id);
            $("#renderModal").html(createModal(data));
            $("#Notification-itemModal div.control").remove();
            $("#NotificationModalToggleClick").click();
        }
    )
}
function paymentView(idType, id) {
    $.get(
        window.location.origin + "/Payment/Detail?Id=" + idType,
        function (data) {
            updatePopupNotification(id);
            $("#renderModal").html(createModal(data));
            $("#Notification-itemModal div.control").remove();
            $("#NotificationModalToggleClick").click();
        }
    )
}
function circleRequestView(idType, id) {
    $.get(
        window.location.origin + "/CircleRequest/Detail?Id=" + idType,
        function (data) {
            updatePopupNotification(id);
            $("#renderModal").html(createModal(data));
            $("#Notification-itemModal div.control").remove();
            $("#NotificationModalToggleClick").click();
        }
    )
}
//check lại
function updatePointUser() {
    $.get(
        window.location.origin + "/User/Point",
        function (data) {
            userAccess.Point = data.data;
            if (window.location.href.indexOf("Member") != -1) {
                //show successfull
                if ($("main .infomation .name .point").length > 0) {
                    $("main .infomation .name .point").html(data.data + " Point");
                    console.log(data);
                }
            }
        }
    )
}
function updatePopupNotification(id) {
    $.ajax({
        url: window.location.origin + "/Notification/Seen",
        data: JSON.stringify(id),
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            //nếu đúng
            console.log(result);
            if (result.status == 200) {
                $(`.alert-noti-${id} .status`).remove();
                if ($(`.status`).length == 0) {
                    hideNewSignal("#tag_notification #dropdownMenuNotification");
                }
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function updateAllPopupNotification() {
    $.ajax({
        url: window.location.origin + "/Notification/SeenAll",
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            //nếu đúng
            console.log(result);
            if (result.status == 200) {
                $(`.status`).remove();
                hideNewSignal("#tag_notification #dropdownMenuNotification");
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}