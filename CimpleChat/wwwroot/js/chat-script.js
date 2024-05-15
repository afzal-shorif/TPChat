CimpleChat.Chat = (function () {

	// public methods

	let initChat = function () {

	}

	let renderMessage = function (data) {

		switch (data.MessageType) {
			case 'Single': renderSingleMessage(data.MessageInfo);
				break;
			case 'Multiple': renderMultipleMessage(data.MessageInfo);
				break;
			case 'Announce': renderAnnounceMessage(data.MessageInfo);
				break;
		}
	}


	// private methods

	// message rendering
	let renderSingleMessage = function (data) {
		let messageDom = [];

		let userName = data.User.Name.toString();

		if (data.User.Id != userId) {
			messageDom.push('<div class="msgRow d-flex flex-row justify-content-start w-100 mt-2">');
			messageDom.push('<div class="msg-user mx-2">');
			messageDom.push(userName.substring(0, 1).toUpperCase());
			messageDom.push('</div>');
			messageDom.push('<div class="msg-txt">');
			messageDom.push(data.Message.Content);
			messageDom.push('</div>');
			messageDom.push('</div>');

		} else {
			messageDom.push('<div class="msgRow d-flex flex-row justify-content-end w-100 mt-2">');
			messageDom.push('<div class="msg-txt">');
			messageDom.push(data.Message.Content);
			messageDom.push('</div>');
			messageDom.push('<div class="msg-user mx-2">');
			messageDom.push(userName.substring(0, 1).toUpperCase());
			messageDom.push('</div>');
			messageDom.push('</div>');
		}

		renderElementToMessageContainer(messageDom.join(''));
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

	


	return {
		initChat: initChat
	}
})();