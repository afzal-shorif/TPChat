
CimpleChat.Chat = (function () {

	let messageContainer = null;

	// public methods

	let initChat = function (messageContainerDiv) {
		messageContainer = messageContainerDiv;
	}


	let renderMessage = function (data) {

		switch (data.MessageType) {
			case 'Single': renderSingleMessage(data.MessageInfo, { "form": "server" });
				break;
			case 'Multiple': renderMultipleMessage(data.MessageInfo);
				break;
			case 'Announce': renderAnnounceMessage(data.MessageInfo);
				break;
			case 'MessageSave': renderMessageSaveResponseFlag(data.MessageInfo);			// response: my message is received by server
				break;
			case 'MessageSend': renderMessageSendResponseFlag(data.MessageInfo);			// response: my message has been send to other people
				break;
			case 'MessageSeen': renderMessageSeenResponseFlag(data.MessageInfo);			// response: my message has been seen by other people
				break;
			case 'MessageSeenByOther': renderMessageSeenByOtherFlag(data.MessageInfo);		// response: a message (other people) has been seen by other poeple
				break;
		}
	}

	// send message
	let sendMessageSeenResponse = function (msgId) {
		let msgObj = {
			type: "msgSeen",
			id: msgId,
			date: Date.now()
		}

		CimpleChat.WebSocketModule.sendMessage(JSON.stringify(msgObj));
	}

	let sendUserMessage = function (msg) {
		let msgObj = {
			type: "Message",
			text: msg,
			date: Date.now()
		}

		renderSingleMessage(msgObj, {"form": "user"});
		CimpleChat.WebSocketModule.sendMessage(JSON.stringify(msgObj));
	}

	// private methods

	// message rendering
	let renderSingleMessage = function (data, opts) {
		let messageDom = [];
		

		if (opts["form"] === "user") {
			messageDom.push('<div class="msgRow d-flex flex-row justify-content-end w-100 mt-2">');
			messageDom.push('<div class="msg-txt">');
			messageDom.push(data.text);
			messageDom.push('</div>');
			messageDom.push('<div class="msg-user mx-2">');
			messageDom.push(userName.substring(0, 1).toUpperCase());
			messageDom.push('</div>');
			messageDom.push('</div>');

		} else {
			let userName = data.User.Name.toString();

			messageDom.push('<div class="msgRow d-flex flex-row justify-content-start w-100 mt-2">');
			messageDom.push('<div class="msg-user mx-2">');
			messageDom.push(userName.substring(0, 1).toUpperCase());
			messageDom.push('</div>');
			messageDom.push('<div class="msg-txt">');
			messageDom.push(data.Message.Content);
			messageDom.push('</div>');
			messageDom.push('</div>');
		}

		renderElementToMessageContainer(messageDom.join(''));
		//renderSeenFlag(flag);

		// send message receive and seen response
	}

	let renderMultipleMessage = function (data) {
		$.each(data, function (index) {
			if (typeof data[index].User.Id.toString() === '0') {
				renderAnnounceMessage(data[index]);
			} else {
				renderSingleMessage(data[index]);
			}
		});
	}

	let renderAnnounceMessage = function (data) {
		let messageDom = [];

		messageDom.push('<div class="msgRow d-flex flex-row justify-content-center w-100 mt-2">');

		messageDom.push('<div class="announce-txt">');
		messageDom.push(data.Content);
		messageDom.push('</div></div>');

		renderElementToMessageContainer(messageDom.join(''));
	}

	let renderElementToMessageContainer = function (newElement) {
		// Is the scroll position at the bottom append the element and update the scroll position to bottom
		let container = $(messageContainer);
		let isScrollAtBottom = container[0].scrollHeight - container.scrollTop() === container.outerHeight();

		container.append(newElement);

		if (isScrollAtBottom) {		
			container.scrollTop(container[0].scrollHeight);
		}
	}

	// message seen flag rendering

	let renderSeenFlag = function (status) {

		// remove previous seen flag
		$(messageContainer).find('.msg-status').remove();

		// append new seen flag

		let seenFlagDom = [];
		seenFlagDom.push('<div class="msgRow msg-status d-flex flex-row justify-content-end w-100 mt-2">');

		// if my message or my message seen response, no matter where the scroll or input field active
		// update the seen flag


		// if other user message or message seen response,
			//if scroll position is at bottom
			//message input box is active
			// set seen flag and seen response as seen


		if (status === 'seen') {
			// if input box in focused or scroll position is at the bottom
			//if (document.activeElement.id === 'msgContent') {
				seenFlagDom.push('<i class="fa-solid fa-circle-check" style="font-size: 13px;"></i>');
			//} else {
				//seenFlagDom.push('<i class="fa-regular fa-circle-check" style="font-size: 13px;"></i>');
			//}
		} else {
			seenFlagDom.push('<i class="fa-regular fa-circle-check" style="font-size: 13px;"></i>');
		}

		seenFlagDom.push('</div>');

		renderElementToMessageContainer(seenFlagDom.join(''));
		// send response as message seen
	}

	let renderSelfMessageSeenflag = function () {
		$(messageContainer).find('.msg-status').remove();

		let seenFlagDom = [];
		seenFlagDom.push('<div class="msgRow msg-status d-flex flex-row justify-content-end w-100 mt-2">');

		if (status === 'seen') {
			seenFlagDom.push('<i class="fa-solid fa-circle-check" style="font-size: 13px;"></i>');
		} else {
			seenFlagDom.push('<i class="fa-regular fa-circle-check" style="font-size: 13px;"></i>');
		}

		seenFlagDom.push('</div>');

		renderElementToMessageContainer(seenFlagDom.join(''));

	}


	return {
		initChat: initChat,
		renderMessage: renderMessage,
		//updateMessageSeenStatus: updateMessageSeenStatus
	}
})();