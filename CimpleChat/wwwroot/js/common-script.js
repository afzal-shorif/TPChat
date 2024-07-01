CimpleChat.Common = (function () {
	let updateUserStatus = function (url) {
 
        setInterval(function () {
            if (CimpleChat.UserRegistration.isCookieAvailable("userInfo")) {
                $.ajax({
                    url: url,
                    type: 'GET',
                    success: function (response) {
                    },
                    error: function (response) {
                    }
                });
            }
        }, 30000); 
	}

	return {
        updateUserStatus: updateUserStatus
	}
})();