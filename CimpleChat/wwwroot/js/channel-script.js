CimpleChat.Channel = (function () {
	let userListContainer = null;

	let init = function (userListDiv, messageContainerDiv) {
		userListContainer = userListDiv;

		adjustMessageContainerHeight();

		let url = 'wss://' + window.location.host + '/Channel/Chat/?channelId=' + channelId;

		CimpleChat.Chat.UIRender.init(messageContainerDiv);
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

	let sendMessage = function (msg, callBack) {
		var trimedMessage = "",
			isEnqueue = false,
			msgObj = {};

		// validate
		trimedMessage = msg.trim();

		if (trimedMessage.length > 0) {
			msgObj = {
				Type: "Message",
				Data: {
					ChannelId: channelId,
					UserId: userId,
					UserName: username,
					Content: msg,
					Status: -1,
					TempMessageId: Date.now(),
				},
			}

			isEnqueue = CimpleChat.Chat.MessageQueue.enqueue(msgObj);

			if (isEnqueue) {
				callBack();
			}
		}
	}

	let getChannelList = function (callback) {
		$.ajax({
			url: Urls.channelList,
			type: 'GET',
			contentType: 'application/json',
			success: function (response) {
				if (typeof callback !== "undefined") {
					callback(response);
				}
			},
			error: function (response) {
			}
		});
	}

	let renderChannels = function (userList) {
		let userListDom = [];
		let no = 0;
		$.each(userList, function (i) {
			userListDom.push('<tr>');
			userListDom.push('<td>' + (++no) + '</td>');
			userListDom.push('<td class="channel-name" style="cursor: pointer;" data-channel="' + userList[i].id + '">' + userList[i].name +'</td>');
			userListDom.push('<td class="text-end">' + userList[i].numberOfUser + '</td>');
			userListDom.push('</tr>');
		});

		$('#tblChannelList tbody').html('');
		$('#tblChannelList tbody').html(userListDom.join(''));
	}

	return {
		initChannel: init,
		renderChannelUserList: renderUserList,
		sendMessage: sendMessage,
		getChannelList: getChannelList,
		renderChannels: renderChannels
	}
})();