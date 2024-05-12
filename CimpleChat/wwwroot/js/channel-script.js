CimpleChat.Channel = (function () {
	let userListContainer = null;
	let messageContainer = null;

	let init = function (userListDiv, messageContainerDiv) {
		userListContainer = userListDiv;
		messageContainer = messageContainerDiv;

		adjustMessageContainerHeight();

		let url = 'wss://' + window.location.host + '/Chat/?channelId=' + channelId;

		CimpleChat.WebSocketModule.initWebSocket(url);
	}

	let adjustMessageContainerHeight = function(){
		let navHeight = $('nav').first().outerHeight(true);
		$('#bodyWrapper').height(window.innerHeight - navHeight - 20 + 'px');

		let footerHeight = $('#footerWrapper').outerHeight(true);
		let msgWindow = $('#msgWindow');
		msgWindow.height(msgWindow.parent().height() - footerHeight);
	}

	let renderUserList = function (userList) {
		let userListDom = [];

		$.each(userList, function (index) {
			userListDom.push('<button type="button" class="list-group-item list-group-item-action" data-id="');
			userListDom.push(userList[index].Id);
			userListDom.push('">');
			userListDom.push(userList[index].Name);
			userListDom.push('</button>');
		});

		$(userListContainer).html('');
		$(userListContainer).html(userListDom.join(''));
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

		$(messageContainer).append(messageDom.join(''));
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

		$(messageContainer).append(messageDom.join(''));
	}

	let sendMessage = function (msg, callBack) {
		// validate

		//if (valid) {
		CimpleChat.WebSocketModule.sendMessage(msg, callBack);
		//}
	}

	return {
		initChannel: init,
		renderChannelUserList: renderUserList,
		renderMessage: renderMessage,
		sendMessage: sendMessage
	}
})();