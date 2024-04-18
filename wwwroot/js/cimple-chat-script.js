function registerUser(userName, channelId) {
    // validate user

    let userReg = $.ajax({
        url: Urls.registration,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ UserName: userName }),
        success: function (response) {
            console.log(response);
            if (response.status === 'success') {
                setCookie(JSON.stringify(response.cookieInfo));

                registrationModal.hide();
                redirectToChannel();
            } else {

            }
        },
        error: function (response) {
            console.log(response);
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