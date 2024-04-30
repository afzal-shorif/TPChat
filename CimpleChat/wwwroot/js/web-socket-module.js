CimpleChat.WebSocketModule = (function () {
	let socket = null;

	let init = function (url) {

		try {
			socket = new WebSocket(url);

			socket.onopen = open;
			socket.onmessage = receive;
			socket.onerror = error;
			
		} catch (Exception) {
			alert("Failed to connect with server. Please reload the page.");
		}
	}

	let open = function (event) {
		// active the text input
	}

	let send = function (msg, callBack) {
		if (socket == null) {
			alert('Connection is not established yet. Please wait or reload the page.');
			return;
		}

		if(socket.readyState.toString() === "1") {
			if (typeof (msg) === "string" && msg.length <= 80) {
				socket.send(msg);
			} else {
				alert("Invalid input. Currently we allow only text and maximum 80 character.");
			}
		}

		if (callback != null) {
			callBack(socket.readyState);
		}
	}

	let receive = function (event) {
		let response = JSON.parse(event.data);

		switch (response.Type) {
			case '__PING__': send('__PONG__');
				break;
			case 'Message': CimpleChat.Channel.renderMessage(response.data);
				break;
			case 'ActiveChannelUserList': CimpleChat.Channel.renderChannelUserList(response.data);
				break;
			case 'ChannelList':
				break;
		}
	}

	let error = function (event) {
		// send ajax request to log the error
	}

	let close = function (code, reason) {
		if (socket == null) {
			return;
		}

		if (typeof reason === "undefined") {
			socket.close(code);
		} else {
			socket.close(code, reason);
		}
	}

	return {
		initWebSocket: init,
		closeWebSocket: close,
		sendMessage: send,
	}
})();