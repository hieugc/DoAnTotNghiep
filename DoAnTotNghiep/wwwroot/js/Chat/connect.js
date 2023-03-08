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
    console.log(message);
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
                            getMessage(data);
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
function getMessage(data) {
    $.ajax({
        url: window.location.origin + '/MessagesInChatRoom',
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            getChatRoomData();
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