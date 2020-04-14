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
        var chatInfo = `<div>[${message.user}] ${escapeHtml(message.text)}</div>`;
        $("#messagesList").append(chatInfo);
    });

$("#sendButton").click(function () {
    var message = $("#messageInput").val();
    connection.invoke("Send", message);
});

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