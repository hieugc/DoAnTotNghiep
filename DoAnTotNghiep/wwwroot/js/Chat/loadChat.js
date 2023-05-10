function layoutInit() {
    return `<ul class="list-user" onscroll="scrollLoadUser(this)"></ul><div class="tab-message"></div>`;
}
//>>> khung user bên trái
function activeUser(room) {
    let str = `<li class="user user-${room.idRoom} active" onclick="loadMessages(${room.idRoom}, this)">
                        <a href="#message-${room.idRoom}">`;
    return str + userItem(room);
}
function seenUser(room) {
    let str = `<li class="user user-${room.idRoom}" onclick="loadMessages(${room.idRoom}, this)">
                        <a href="#message-${room.idRoom}">`;
    return str + userItem(room);
}
function unSeenUser(room) {
    let str = `<li class="user user-${room.idRoom} new-message" onclick="loadMessages(${room.idRoom}, this)">
                        <a href="#message-${room.idRoom}">`;
    return str + userItem(room);
}
function userItem(room) {
    let str = "";
    if (room.userMessages[0].imageUrl == null) {
        str += `<span class="avt"><img src=${window.location.origin + "/Image/user.svg"} alt="avatar" /></span>`;
    }
    else {
        str += `<span class="avt"><img src="${room.userMessages[0].imageUrl}" alt="avatar" /></span>`;
    }
    str += `<span class="name">${room.userMessages[0].userName}</span></a></li>`;
    return str;
}
///////////////////////////////
//khung chat bên phải
function appendMessage(idTag, object) {
    let res = `<div class="tab-content" id="message-${object.idRoom}">`;

    res += `<div class="top-tab-message">
                    <div class="user-name">${object.userMessages[0].userName}</div>
                    <div class="request">
                        <label class=" position-relative">
                            <i class="fa-solid fa-bullhorn"></i>
                            <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                                !
                                <span class="visually-hidden">unread messages</span>
                            </span>
                        </label>
                    </div>
                </div>`;


    res += `<div class="list-message" id="${idTag}" onscroll="scrollLoad(this, ${object.idRoom})">`;
    if (object.messages.length > 0) {
        let prev = object.messages[0].idSend;
        let show = [];
        for (let e in object.messages) {
            if (object.messages[e].idSend == prev) {
                show[show.length] = object.messages[e].message;
            }
            else {
                if (prev == userAccess.userAccess) {
                    res += selfMessage(userAccess.urlImage, userAccess.userName, show);
                }
                else {
                    res += otherMessage(object.userMessages[0].imageUrl, object.userMessages[0].userName, show);
                }
                prev = object.messages[e].idSend;
                show = [object.messages[e].message];
            }
        }
        if (show.length > 0) {
            if (prev == userAccess.userAccess) {
                res += selfMessage(userAccess.urlImage, userAccess.userName, show);
            }
            else {
                res += otherMessage(object.userMessages[0].imageUrl, object.userMessages[0].userName, show);
            }
        }
    }
    res += `</div>`;

    res += `<div class="bot-tab-message">
                <div class="tooltip-control dropup">
                    <button type="button" class="control-label" data-bs-toggle="dropdown"><i class="fa-solid fa-plus"></i></button>
                    <div class="control-frame dropdown-menu">
                        <div class="request dropdown-item" onclick="getRequestFormByUserAccess('${object.userMessages[0].userAccess}')">Tạo yêu cầu</div>
                        <div class="dropdown-item"><label for="image-send">Thêm ảnh</label><input type="file" name="image-send" id="image-send" hidden /></div>
                    </div>
                </div>

                <div class="frame-control">
                <textarea rows="1" maxlength="500" class="form-control" name="input-message-${object.idRoom}" id="input-message-${object.idRoom}" placeholder="Tin nhắn" oninput="changeHeight(this, ${object.idRoom})"></textarea>
                    <button type="button" class="btn btn-primary" onclick="SendMessage('${object.idRoom}')"><i class="fa-solid fa-paper-plane"></i></button>
                </div>
            </div>`;

    res += `</div>`;
    return res;
}
function selfMessage(urlImage, name, listchat) {
    let res = `<div class="user-chat">`;
    res += headMessage(urlImage, name);
    res += frameMessage(listchat);
    res += `</div>`;
    return res;
}
function otherMessage(urlImage, name, listchat) {
    let res = `<div class="other user-chat">`;
    res += headMessage(urlImage, name);
    res += frameMessage(listchat);
    res += `</div>`;
    return res;
}
function headMessage(urlImage, name) {
    let str = `<div class="chat-head">`;
    console.log(urlImage);
    if (urlImage == null) {
        str += `<div class="avt"><img src=${window.location.origin + "/Image/user.svg"} alt="avatar" /></div>`;
    }
    else {
        str += `<div class="avt"><img src="${urlImage}" alt="avatar" /></div>`;
    }
    str += `<div class="name">${name}</div></div>`;
    return str;
}
function frameMessage(listchat) {
    let res = `<div class="message-frame">`;
    for (let e in listchat) {
        res += message(listchat[e]);
    }
    res += `</div>`;
    return res;
}
function message(chat) {
    return `<div class="chat"><span>${chat}</span></div>`;
}
//////////////////////////////

//tab message => update isSeen
//load tab message khi click vào user
function loadMessages(idRoom, othis) {
    if (chatRoom[idRoom]["isInit"] != true) {
        //messages.isFull == true không load nữa
        //messages.length == 0 => chưa load thêm

        if (chatRoom[idRoom]["isFull"] != true && chatRoom[idRoom]["isLoad"] != true) {
            let data = {
                idRoom: idRoom,
                pagination: {
                    page: chatRoom[idRoom].messages.legnth,
                    limit: 10,
                }
            };
            $.ajax({
                url: window.location.origin + '/MessagesInChatRoom',
                data: JSON.stringify(data),
                contentType: "application/json",
                type: "POST",
                success: function (result) {
                    console.log(result);
                    if (result.status == 200) {
                        chatRoom[idRoom] = result.data[idRoom];
                        //update user
                        if (chatRoom[idRoom]["isFull"] != true && result.data[idRoom].messages.legnth < 10) {
                            chatRoom[idRoom]["isFull"] = true;
                        }
                        addTabMessage(idRoom, othis);
                        chatRoom[idRoom]["isLoad"] = false;
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr);
                    console.log(status);
                    console.log(error);
                }
            });
        }
        else {
            addTabMessage(idRoom, othis)
        }
    }
    else {
        showTabMessage(idRoom, othis);
    }
}
//khởi tạo tab khi bên kia chưa init
function addTabMessage(idRoom, othis) {
    $(".tab-message").append(appendMessage("message-" + idRoom, chatRoom[idRoom]));
    chatRoom[idRoom]["isInit"] = true;
    updateSeen(idRoom);
    showTabMessage(idRoom, othis);
}
function showTabMessage(idRoom, othis) {
    $($(".user.active a")[0].hash).hide();
    $("#message-" + idRoom).show();
    $(".user.active").removeClass("active");
    $(othis).removeClass("new-message").addClass("active");
}
function updateSeen(idRoom) {
    $.ajax({
        url: window.location.origin + "/Message/Seen",
        data: JSON.stringify(idRoom),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            updateIsSeenInLocal(idRoom);
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}
function updateIsSeenInLocal(idRoom) {
    for (let e in chatRoom[idRoom.message]) {
        e.isSeen = true;
    }
}
///////////////////////////////////

//thêm message lúc gửi
function changeHeight(element, idRoom) {
    element.style.height = 'auto';
    element.style.height = (element.scrollHeight) + 'px';
    
    element.onkeydown = function (e) {
        if (!e.shiftKey && e.keyCode == 13) {
            SendMessage(idRoom);
            $("#input-message-" + idRoom).val(null);
            element.style.height = 'auto';
        }
    }
}
function addMessageToFrame(chat, tag) {
    $(tag + " .message-frame").first().prepend(message(chat));
}
function addMessage(messageModel, idRoom, isSelf, isContinue) {
    if ($(".list-user").length == 0) {
        layoutInit();

    }

    //isSelf == true => bản thân gửi 
    if (isContinue == true) {
        addMessageToFrame(messageModel.message, "#message-" + idRoom);
    }
    else {
        console.log(userAccess);
        if (isSelf == true) {
            $(`#message-${idRoom} .list-message`).prepend(selfMessage(userAccess.urlImage, userAccess.firstName + " " + userAccess.lastName, [messageModel.message]));
        }
        else {
            $(`#message-${idRoom} .list-message`).prepend(otherMessage(chatRoom[idRoom].userMessages[0].imageUrl, chatRoom[idRoom].userMessages[0].userName, [messageModel.message]));
        }
    }
}
function SendMessage(idRoom) {
    if ($("#input-message-" + idRoom).val().trim().length > 0) {
        let data = {
            message: $("#input-message-" + idRoom).val().trim(),
            idRoom: idRoom,
            idReply: 0
        }
        $("#input-message-" + idRoom).val(null);
        $("#input-message-" + idRoom)[0].style.height = "auto";
        $.ajax({
            url: window.location.origin + "/Send",
            data: JSON.stringify(data),
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                if (result.status == 200) {
                    //>>> thành công thì => clear khung chat
                    $("#input-message-" + idRoom).val(null);
                    $("#input-message-" + idRoom)[0].style.height = "auto";
                    //>>> xóa new message
                    $(".user-" + idRoom).removeClass("new-message");
                    //>>> cập nhật danh sách đã xem
                    updateSeen(idRoom);
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                console.log(status);
                console.log(error);
            }
        });
    }
}

//scroll message
function scrollLoad(element, idRoom) {
    let top = element.scrollTop.toFixed();
    if (chatRoom[idRoom]["isLoad"] != true && top == 0) {
        chatRoom[idRoom]["isLoad"] = true;
        getAndLoadMessage(idRoom);
        console.log("croll");
    }
}
function getAndLoadMessage(idRoom) {
    if (chatRoom[idRoom]["isFull"] != true) {
        let data = {
            idRoom: idRoom,
            pagination: {
                page: chatRoom[idRoom].messages.length,
                limit: 20,
            }
        };
        $.ajax({
            url: window.location.origin + "/MessagesInChatRoom",
            data: JSON.stringify(data),
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                if (result.status == 200) {
                    //html append
                    let prev = chatRoom[idRoom].messages[chatRoom[idRoom].messages.length - 1].idSend;
                    let index = 0;

                    for (let e in result.data[idRoom].messages) {
                        if (prev == result.data[idRoom].messages[e].idSend) {
                            $(".list-message .user-chat .message-frame").last().append(message(result.data[idRoom].messages[e].message));
                            index += 1;
                        }
                        else {
                            prev = result.data[idRoom].messages[e].idSend;
                            break;
                        }
                    }
                    //gán vô cái tiếp theo
                    let show = [];
                    for (let e in result.data[idRoom].messages) {
                        if (e < index) continue;
                        if (result.data[idRoom].messages[e].idSend == prev) {
                            show[show.length] = result.data[idRoom].messages[e].message;
                        }
                        else {
                            if (prev == userAccess.userAccess) {
                                $(`#message-${idRoom} .list-message`).append(selfMessage(userAccess.urlImage, userAccess.userName, show));
                            }
                            else {
                                $(`#message-${idRoom} .list-message`).append(otherMessage(result.data[idRoom].userMessages[0].imageUrl, result.data[idRoom].userMessages[0].userName, show));
                            }
                            prev = result.data[idRoom].messages[e].idSend;
                            show = [result.data[idRoom].messages[e].message];
                        }
                    }
                    if (show.length > 0) {
                        if (prev == userAccess.userAccess) {
                            $(`#message-${idRoom} .list-message`).append(selfMessage(userAccess.urlImage, userAccess.userName, show));
                        }
                        else {
                            $(`#message-${idRoom} .list-message`).append(otherMessage(result.data[idRoom].userMessages[0].imageUrl, result.data[idRoom].userMessages[0].userName, show));
                        }
                    }
                    chatRoom[idRoom].messages = [].concat(chatRoom[idRoom].messages, result.data[idRoom].messages);


                    //xác định isFull?
                    if (result.data[idRoom].messages.length > 0 && result.data[idRoom].messages.length < 20) {
                        chatRoom[idRoom]["isFull"] = true;
                    }

                    //cập nhật isLoad
                    chatRoom[idRoom]["isLoad"] = false;

                    //xóa dấu hiệu load
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                console.log(status);
                console.log(error);
            }
        });
    }
}


function prependNewMessage(tag) {
    let temp = $(tag);
    $(tag).remove();
    $(".list-user").prepend(temp);

    $(tag).addClass("new-message");
}

//scroll user
//function load 10 user tiếp theo => append

function scrollLoadUser(element) {
    let top = element.scrollTop.toFixed();
    if (chatRoom["isLoadUser"] != true && top >= element.style.height) {
        let data = {
            page: chatRoom.length,
            limit: 10
        };
        chatRoom["isLoadUser"] = true;
        $.ajax({
            url: window.location.origin + "/ChatRoom",
            data: JSON.stringify(data),
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                chatRoom["isLoadUser"] = false;
                console.log(result);
                if (result.status == 200) {
                    for (e in result.data) {
                        if (result.data[e].messages[0].isSeen) {
                            $(".list-user").append(seenUser(result.data[e]));
                        }
                        else {
                            $(".list-user").append(unSeenUser(result.data[e]));
                        }
                    }
                    if (result.data.length < 10 && chatRoom["isFullUser"] != true) {
                        chatRoom["isFullUser"] = true;
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
}