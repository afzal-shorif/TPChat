CimpleChat.Channel = (function () {
	let userListContainer = null;
	var currentChannelId = -1;

	let init = function (userListDiv, messageContainerDiv) {
		userListContainer = userListDiv;
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
					ChannelId: currentChannelId,
					UserId: CimpleChat.Common.getConst('userId'),
					UserName: CimpleChat.Common.getConst('username'),
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

	let sendRequestForUpdateChannelList = function () {
		var channelListRequest = {
			Type: "ChannelList",
			Data: { emptyObject: "" },
		}

		CimpleChat.Chat.MessageQueue.enqueue(channelListRequest, false);
	}

	return {
		initChannel: init,
		renderChannelUserList: renderUserList,
		sendMessage: sendMessage,
		getChannelList: function (callback) {
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
		},
		renderChannels: function (channelList) {
			var privateChannels = [],
				publicChannels = [];

			privateChannels.push('<ul class="list-group">');
			publicChannels.push('<ul class="list-group">');

			$.each(channelList, function (i) {
				var name = (channelList[i].Name.length > 16) ? channelList[i].Name.substring(0, 16) + '...' : channelList[i].Name

				if (channelList[i].Type === 0) {

					privateChannels.push('<li class="list-group-item d-flex justify-content-between align-items-center" data-channel="' + channelList[i].Id + '" data-name="' + channelList[i].Name + '" title="' + channelList[i].Name + '">');
					privateChannels.push(name);
					privateChannels.push('<span class="badge bg-primary rounded-pill">0</span>');
					privateChannels.push('</li>');

				} else {

					publicChannels.push('<li class="list-group-item d-flex justify-content-between align-items-center" data-channel="' + channelList[i].Id + '" data-name="' + channelList[i].Name + '" title="' + channelList[i].Name + '">');
					publicChannels.push(name);
					publicChannels.push('<span class="badge bg-primary rounded-pill">0</span>');
					publicChannels.push('</li>');

				}
			});

			privateChannels.push('</ul>');
			publicChannels.push('</ul>');

			$("#privateChannelListContainer").html('').html(privateChannels.join(''));
			$("#publicChannelListContainer").html('').html(publicChannels.join(''));
		},

		getMessageHistory: function (channelId) {
			// check is cache available
			// if not, send a request to get message history

			var requestObject = {
				Type: "MessageHistory",
				Data: {
					ChannelId: channelId,
					LastMsgId: -1,
				}
			};
			CimpleChat.Chat.UIRender.clear();
			currentChannelId = channelId;
			$('#addMemberBtn').show();
			CimpleChat.Chat.MessageQueue.enqueue(requestObject);
		},

		search: function (data) {
			$.ajax({
				url: Urls.searchChannel,
				data: { search: data },
				type: 'GET',
				success: function (result) {
					var container = $('#searchSuggestionContainer');

					if (result.length > 0) {
						var resultDom = [];
						resultDom.push('<ul class="list-group list-group-flush">');

						$.each(result, function (i) {
							resultDom.push('<li class="list-group-item d-flex justify-content-between align-items-start" data-id="' + result[i].id + '">');
							resultDom.push(result[i].name);
							resultDom.push('</li>');
						});

						resultDom.push('</ul>');

						container.html(resultDom.join(''));
						container.show();
					}
				},
				error: function (response) {

				}
			});
		},

		addMemberToChannel: function (channelID, userID) {
			var cId = (channelID === '') ? currentChannelId : channelID,
				uId = (userID === '') ? CimpleChat.Common.getConst('userId') : userID;

			$.ajax({
				url: Urls.addMemberToChannel,
				data: { channelId: cId, userId: uId },
				type: 'GET',
				success: function (response) {
					sendRequestForUpdateChannelList();
				},
				error: function (response) {

				}
			});
		},
		createChannel: function () {
			// validate user input
			var channelType = ($('#channelTypeInput').val() === 'Private') ? 0 : 1;
			var name = $('#channelNameInput').val();

			$.ajax({
				url: Urls.createChannel,
				data: { channelName: name, type: channelType },
				type: 'GET',
				success: function (response) {
					if (typeof (response.channelId) !== 'undefined') {
						$('#createChannelModal').modal('hide');
						sendRequestForUpdateChannelList();
					} else if (typeof (response.error !== 'undefined')) {
						$('#channelNameErrorMsg').text(response.error);
						$('#channelNameInput').addClass('is-invalid');
					}
				},
				error: function (response) {

				}
			});

		}
	}
})();