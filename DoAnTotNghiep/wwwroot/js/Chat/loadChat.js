var htmlMessageStored = {}

function loadMessages(idRoom, othis) {
    $(".user.active").removeClass("active");
    if (chatRoomData[idRoom].isInit == undefined || chatRoomData[idRoom].isInit == false || chatRoomData[idRoom].isInit == null) {
        //thêm message
        $(".tab-message").append(appendMessage("message-" + idRoom, chatRoomData[idRoom]));
        chatRoomData[idRoom]["isInit"] = true;
    }
    $(othis).removeClass("new-message").addClass("active");
}
function activeChat(object) {
    return `<li class="user active">
                <a href="#message-${object.idRoom}">
                    <span class="avt"><img src="${(window.location.origin + "/" + object.userMessages[0].urlImage)}" alt="avatar" /></span>
                    <span class="name">${object.userMessages[0].userName}</span>
                </a>
            </li>`;
}
function seenUser(object) {
    return `<li class="user">
                <a href="#message-${object.idRoom}">
                    <span class="avt"><img src="${(window.location.origin + "/" + object.userMessages[0].urlImage)}" alt="avatar" /></span>
                    <span class="name">${object.userMessages[0].userName}</span>
                </a>
            </li>`;
}
function unSeenUser(object) {
    return `<li class="user new-message">
                <a href="#message-${object.idRoom}">
                    <span class="avt"><img src="${(window.location.origin + "/" + object.userMessages[0].urlImage)}" alt="avatar" /></span>
                    <span class="name">${object.userMessages[0].userName}</span>
                </a>
            </li>`;
}
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


    res += `<div class="list-message" id="${idTag}">`;
    if (object.messages.length > 0) {
        let prev = object.messages[0].userAccess;
        let show = [];
        for (let e in object.messages) {
            if (object.messages[e].userAccess == prev) {
                show[show.length] = object.messages[e].message;
            }
            else {
                if (prev == userAccess) {
                    res += selfMessage(window.location.origin + "/logo.png", "Hiếu", show);
                }
                else {
                    res += otherMessage(window.location.origin + "/logo.png", object.userMessages[0].userName, show);
                }
                prev = object.messages[e].userAccess;
                show = [object.messages[e].message];
            }
        }
        if (show.length > 0) {
            if (prev == userAccess) {
                res += selfMessage(window.location.origin + "/logo.png", "Hiếu", show);
            }
            else {
                res += selfMessage(window.location.origin + "/logo.png", object.userMessages[0].userName, show);
            }
        }
    }
    res += `</div>`;

    res += `<div class="bot-tab-message">
                <div class="tooltip-control dropup">
                    <button type="button" class="control-label" data-bs-toggle="dropdown"><i class="fa-solid fa-plus"></i></button>
                    <div class="control-frame dropdown-menu">
                        <div class="request dropdown-item" data-bs-toggle="modal" data-bs-target="#messageRequestModal">Tạo yêu cầu</div>
                        <div class="dropdown-item"><label for="image-send">Thêm ảnh</label><input type="file" name="image-send" id="image-send" hidden /></div>
                    </div>
                </div>

                <div class="frame-control">
                <textarea rows="1" maxlength="500" class="form-control" name="input-message-${object.idRoom}" id="input-message-${object.idRoom}" placeholder="Tin nhắn"></textarea>
                    <button type="button" class="btn btn-primary" onclick="SendMessage('${object.idRoom}')"><i class="fa-solid fa-paper-plane"></i></button>
                </div>
            </div>`;

    res += `</div>`;
    return res;
}
function seftMessage(urlImage, name, listchat) {
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
    return `<div class="chat-head">
                            <div class="avt"><img src="${urlImage}" alt="avatar" /></div>
                            <div class="name">${name}</div>
                        </div>`;
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
function addMessageToFrame(chat, tag) {
    $(tag + " .message-frame").prepend(message(chat));
}
function addMessage(messageModel, idRoom, isSelf, isContinue) {
    //isSelf == true => bản thân gửi 
    if (isContinue == true) {
        addMessageToFrame(messageModel.message, "#message-" + idRoom);
    }
    else {
        if (isSelf == true) {
            $(".list-message").prepend(seftMessage((window.location.origin + "/logo.png"), "Hiếu", [messageModel.message]));
        }
        else {
            $(".list-message").prepend(otherMessage((window.location.origin + "/logo.png"), chatRoomData[idRoom].userMessages[0].userName, [messageModel.message]));
        }
    }
}
function SendMessage(idRoom) {
    let data = {
        message: $("#input-message-" + idRoom).val(),
        idRoom: idRoom,
        idReply: 0
    }

    $.ajax({
        url: window.location.origin + "/Chat/Send",
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                let idAccess = result.data.userAccess;
                let isContinue = true;
                let isSelf = idAccess == userAccess;
                if (chatRoomData[idRoom].messages.length > 0) {
                    isContinue = (chatRoomData[idRoom].messages[0].userAccess == idAccess);
                }
                chatRoomData[idRoom].messages = chatRoomData[idRoom].messages.unshift(result.data);
                //show chat
                addMessage(result.data, idRoom, isSelf, isContinue);
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}