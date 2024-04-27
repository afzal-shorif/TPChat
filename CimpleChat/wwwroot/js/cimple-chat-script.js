function registerUser(userName, channelId) {
    // validate user

    let userReg = $.ajax({
        url: Urls.registration,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ UserName: userName }),
        success: function (response) {
            if (response.status === 'success') {
                setCookie(JSON.stringify(response.cookieInfo));

                registrationModal.hide();
                redirectToChannel();
            } else {

            }
        },
        error: function (response) {
        }
    });
}

function redirectToChannel() {
    if (!isCookieAvailable()) {
        registrationModal.show();
    } else {
        window.location = Urls.channel + '/?channelId=' + sessionStorage.getItem("channelId");
    }
} 


function isCookieAvailable() {
    let cookieList = document.cookie.split(';');
    let flag = false;
    $.each(cookieList, function (index) {
        let cookeiPair = cookieList[index].trim().split('=');
        
        if (cookeiPair[0] === 'userInfo') {

            if (typeof cookeiPair[1] != 'undefined' && cookeiPair[1] != '') {
                flag = true;
            }
        }
    });

    return flag;
}

function setCookie(cookieInfo) {
    document.cookie = "userInfo=" + JSON.parse(cookieInfo) + "; path=/";
}

function userListSocket() {
    let cnt = 1;
    let socket = new WebSocket("wss://localhost:6969/Channel/ConnectToChannel/?channelId=" + channelId);

    socket.onopen = (e) => {
        //console.log(e);
        socket.send("Connected to the Web Socket");
        console.log("Connected to the Server");
    };

    socket.onclose = (e) => {
        console.log(e);
    };

    socket.onmessage = (e) => {
        console.log("Message Received:" + e.data);
        //GetUserList(e.data);
        return false;
    };

    socket.onerror = (e) => {
        console.log(e);
    };

   setInterval(function () {
        let msg = "Hello World ";
        socket.send(msg + cnt);
        console.log("Sending message: " + msg + cnt);
        cnt++;
    }
    , 5000);
}

function GetUserList(users) {
    let domString = [];
    let userList = JSON.parse(users);

    $.each(userList, function (index) {
        domString.push('<a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" ');
        domString.push('data-id="' + userList[index].ID + '">');
        domString.Push(userList[index].Name);
        domString.push('</a>');
    });

    $('#userList').html('');

    $('#userList').html(userList.join(''));
}

    let WebSocketModule = (function () {
        let socket = null;

        let init = function (OnOpenCallback) {
            socket = new WebSocket("wss://localhost:6969/Channel/ConnectToChannel/?channelId=" + channelId);

            if (OnOpenCallback != null) {
                socket.onopen = OnOpenCallback;
            }

            
            socket.onmessage = receive;
            
        }

        let send = function (msg) {
            socket.send(msg);
        }

        let receive = function (event) {
 
            let response = JSON.parse(event.data);

            switch (response.Type) {
                case 'UserList':
                {
                    //renderUserList(response.UserList);
                }
                    break;
                case 'Message': {
                    $('#msgContainer').append(getMsgRow(response));
                }
                    break;

                case 'Announce': {
                    $('#msgContainer').append(getMsgRow(response));
                }
                    break;
            }       
        }

        let getMsgRow = function(response){
            let msgRow = [];

            if (response.Type.toString() === 'Announce') {

                msgRow.push('<div class="msgRow d-flex flex-row justify-content-center w-100 mt-2">');

                msgRow.push('<div class="msg-txt">');
                msgRow.push(response.Message.Content);
                msgRow.push('</div></div>');

            } else {
                let userName = response.User.Name.toString();

                if (userId !== response.User.Id.toString()) {
                    msgRow.push('<div class="msgRow d-flex flex-row justify-content-start w-100 mt-2">');
                    msgRow.push('<div class="msg-user mx-2">');
                    msgRow.push(userName.substring(0, 1).toUpperCase());
                    msgRow.push('</div>');
                    msgRow.push('<div class="msg-txt">');
                    msgRow.push(response.Message.Content);
                    msgRow.push('</div>');
                    msgRow.push('</div>');

                } else {                    
                    msgRow.push('<div class="msgRow d-flex flex-row justify-content-end w-100 mt-2">');
                    msgRow.push('<div class="msg-txt">');
                    msgRow.push(response.Message.Content);
                    msgRow.push('</div>');
                    msgRow.push('<div class="msg-user mx-2">');
                    msgRow.push(userName.substring(0, 1).toUpperCase());
                    msgRow.push('</div>');
                    msgRow.push('</div>');
                }
            }

            return msgRow.join('');
        }

        return {
            init: init,
            send: send
        }
    })();