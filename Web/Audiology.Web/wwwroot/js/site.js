// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function favourite(userId, songId, albumId) {

    var token = $("#votesForm input[name=__RequestVerificationToken]").val();
    var json = { "userId": userId, "songId": songId, "albumId": albumId };

    $.ajax({
        url: "/api/favourites",
        type: "POST",
        headers: { 'RequestVerificationToken': token },
        data: JSON.stringify(json),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#favouritesCount").html(data.favouritesCount);
        },
        error: function (req, status, error) {
            console.log(error);
        }
    });
}


var connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .withAutomaticReconnect()
        .build();

connection.on("NewMessage",
    function (message) {
        var chatInfo = `<div style="background-color: aliceblue; border-radius: 10px; padding-left: 7px; padding-right: 7px; margin-bottom: 10px; overflow-wrap: break-word;">[${message.user}] ${escapeHtml(message.text)}</div>`;
        $("#messagesList").append(chatInfo);
    });


$("#sendButton").click(function () {

    var artistId = $("#artistId").text();

    if ($("#userId").val() === "") {

    }
    else {
        artistId = $("#userId option:selected").val();
    }
    var message = $("#messageInput").val();
    connection.invoke("Send", message, artistId);
    var chatInfo = `<div style="background-color: aliceblue; border-radius: 10px; padding-left: 7px; padding-right: 7px; margin-bottom: 10px; overflow-wrap: break-word;"> ${(message)}</div>`;
    $("#messagesList").append(chatInfo);
});

$('#messagesList').css("overflow-y", "scroll");

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
