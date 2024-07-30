CimpleChat.Common = (function () {
    var constants = {}
    prefix = (Math.random() + '_').slice(2);

    return {
        initApplication: function () {
            let url = 'wss://' + window.location.host + '/WebSocket';

            CimpleChat.WebSocketModule.initWebSocket(url);
            CimpleChat.Chat.UIRender.init('#msgContainer');

        },
        updateUserStatus: function (url) {

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
        },

        setConst: function (name, value) {
            constants[prefix + name] = value;
            return true;
        },

        getConst: function (name) {
            if (constants.hasOwnProperty(prefix+name)) {
                return constants[prefix + name];
            }

            return null;
        }
	}
})();