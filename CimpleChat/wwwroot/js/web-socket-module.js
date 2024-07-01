CimpleChat.WebSocketModule = (function () {

	let socket = null;
	let connect = null;

	let init = function (url) {
		
		connect = tryToConnect(url);
		connect();
	}

	let tryToConnect = function (url) {
		// to reconnect, we are using javascript clouser to avoid the url parameter as global (global under this module)
		// just try to make things more complex by using clouser. We could easily store url parameter in a variable and reuse it

		return function () {
			socket = new WebSocket(url);

			socket.onopen = open;
			socket.onmessage = receive;
			socket.onerror = error;
		}
	}

	let open = function (event) {
		// active the text input
		console.log("Connection Open");

		// message queue
	}

	let send = function (msg) {
		if (socket == null) {
			alert('Connection is not established yet. Please wait or reload the page.');
			return false;
		}

		if(socket.readyState.toString() === "1") {
			if (socket.bufferedAmount == 0) {
				socket.send(msg);
				return true;
			}
		}

		return false;
	}

	let receive = function (event) {
		let response = JSON.parse(event.data);
		console.log(response);
		switch (response.Type) {
			case '__PING__': send('__PONG__');
				break;
			case 'Message':
			case 'MessageStatus':
			case 'Announce': CimpleChat.Chat.UIRender.renderMessage(response);
				break;
			case 'ActiveChannelUsers': CimpleChat.Channel.renderChannelUserList(response.Data);
				break;
		}
	}

	let error = function (event) {
		// send ajax request to log the error

		// inform Message Queue that disconnect
		CimpleChat.Chat.MessageQueue.onDisconnect();

		// try to reconnect
		//connect();
	}

	let close = function (code, reason) {
		if (socket == null) {
			return;
		}

		socket.onclose = function () { }
		socket.close();

		//if (typeof reason === "undefined") {
		//	socket.close(code);
		//} else {
		//	socket.close(code, reason);
		//}
	}

	return {
		initWebSocket: init,
		closeWebSocket: close,
		sendMessage: send,
	}
})();