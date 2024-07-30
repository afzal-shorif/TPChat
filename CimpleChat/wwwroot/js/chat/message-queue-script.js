// the message queue module send the message to the server and
// wait until receive the response(message is send) from server
// the sending message will fail only if event OnClose/OnError occur

CimpleChat.Chat.MessageQueue = (function () {

	const messageQueue = [];
	let sendMessageInterval = null;
	let isWaitingForResponse = false;

	// private methods
	let sendMessage = function () {
		if (messageQueue.length <= 0) {
			clearInterval(sendMessageInterval);
			sendMessageInterval = null; 
			return;
		}

		if (isWaitingForResponse) {
			return;
		}

		// send the message if websocket buffer is empty and wait for the response
		let msg = messageQueue[0];
		let resp = CimpleChat.WebSocketModule.sendMessage(JSON.stringify(msg));
		if (resp) {
			if (msg.type === 'message') {
				isWaitingForResponse = true;
			}
			messageQueue.shift();
		}
	}

	return {
		/*
		* add the object to the message queue
		* render the message to the UI
		* try to send the message
		*
		*
		* @param object obj: object that need to send to the server
		* @return bool
		*	return true if object is added to the queue and rendered to the UI
		*	calling function should not clear the input field if return false
		*/
		enqueue: function (obj, isRenderable = true) {

			if (messageQueue.length >= 10) {
				return false;											//to inform that don't clear the input field
			}

			// add message to the message queue and 
			messageQueue.push(obj);

			// render the object into the UI
			if (isRenderable) {
				CimpleChat.Chat.UIRender.renderMessage(obj);
			}

			//try to send message
			if (sendMessageInterval === null) {
				sendMessageInterval = setInterval(sendMessage, 500);
			}

			return true;												//the message is added to the queue and rendered to the UI
		},

		onDisconnect: function () {
			if (sendMessageInterval != null) {
				clearInterval(sendMessageInterval);
			}

			// show message send failed
		},

		onConnect: function () {
			if (sendMessageInterval === null) {
				sendMessageInterval = setInterval(sendMessage, 500);
			}
		},

		onResponse: function () {
			isWaitingForResponse = false;
		}
	}
})();