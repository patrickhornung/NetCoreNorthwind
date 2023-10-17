"use strict";

let connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();

const sendButton = document.getElementById('sendButton');
sendButton.disabled = true;

connection.on('ReceiveMessage', function (user, message) {
    let li = document.createElement('li');
    li.textContent = `${user} says: ${message}`;
    document.getElementById('messagesList').appendChild(li);
});

connection.start().then(function() {
    sendButton.disabled = false;
}).catch (function(err) {
    return console.error(err.toString());
});

sendButton.addEventListener('click', function (event) {
    let user = document.getElementById('userInput').value;
    let message = document.getElementById('messageInput').value;

    connection.invoke('SendMessage', user.toString(), message.toString()).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault;
});