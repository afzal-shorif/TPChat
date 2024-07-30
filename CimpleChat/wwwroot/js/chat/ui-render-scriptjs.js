CimpleChat.Chat.UIRender = (function () {
	let messageContainer = null;

	// render message
	let renderMessageToUI = function (response) {
		var messageDomString = '',
			opts = {fomr: ''};
		if (Array.isArray(response.Data)) {
			// if the Data is and array, load the previous message and return
		
			$.each(response.Data, function (index, value) {
				messageDomString += prepareSingleMessage(value);
			});
			renderElementToMessageContainer(messageDomString, opts);
			// place the seen/save/delivered flag based on the status
			// in this case, we don't need to send the seen response

			renderStatusFlag({
				MessageId: response.Data[response.Data.length-1].messageId,
				Status: response.Data[response.Data.length-1].Status
			});
			return;
		}

		if (response.Type === "MessageStatus") {
			renderStatusFlag(response.Data);
			return;
		}

		switch (response.Type) {
			case "Message": messageDomString = prepareSingleMessage(response.Data);
				break;
			case "Announce": messageDomString = prepareAnnounceMessage(response.Data);
				break;
		}

		// for client(current user) message, set the scroll at the bottom
		if (response.Data.UserId == CimpleChat.Common.getConst('userId')) {
			opts.from = 'client';
		}

		renderElementToMessageContainer(messageDomString, opts);

		if (response.Type === "Message" && response.Data.UserId != CimpleChat.Common.getConst('userId')) {
			sendMessageSeenResponse(response.Data.MessageId);
		}
	}


	let renderAnnounceMessage = function (data) {
		let messageDom = [];

		messageDom.push('<div class="msgRow d-flex flex-row justify-content-center w-100 mt-2">');

		messageDom.push('<div class="announce-txt">');
		messageDom.push(data.Content);
		messageDom.push('</div></div>');

		renderElementToMessageContainer(messageDom.join(''), { from : 'server' });
	}

	let prepareSingleMessage = function (value) {
		let messageDom = [],
			msgId = value.hasOwnProperty('MessageId') ? value.MessageId : value.TempMessageId;

		if (value.UserId.toString() === "0") {
			messageDom.push(renderAnnounceMessage(value));

		} else if (value.UserId.toString() === CimpleChat.Common.getConst('userId').toString()) {
			messageDom.push('<div class="msgRow status-' + value.Status + ' d-flex flex-row justify-content-end w-100 mt-2" id="message' + msgId + '" data-uid="' + value.UserId +'">');
			messageDom.push('<div class="msg-txt">');
			messageDom.push(value.Content);
			messageDom.push('</div>');
			messageDom.push('<div class="msg-user mx-2">');
			messageDom.push(value.UserName.substring(0, 1).toUpperCase());
			messageDom.push('</div>');
			messageDom.push('</div>');
		} else {
			messageDom.push('<div class="msgRow status-' + value.Status + ' d-flex flex-row justify-content-start w-100 mt-2" id="message' + msgId + '" data-uid="' + value.UserId + '">');
			messageDom.push('<div class="msg-user mx-2">');
			messageDom.push(value.UserName.substring(0, 1).toUpperCase());
			messageDom.push('</div>');
			messageDom.push('<div class="msg-txt">');
			messageDom.push(value.Content);
			messageDom.push('</div>');
			messageDom.push('</div>');
		}

		return messageDom.join('');
	}

	let prepareAnnounceMessage = function (data) {
		let messageDom = [];

		messageDom.push('<div class="msgRow d-flex flex-row justify-content-center w-100 mt-2" id="message' + data.MessageId + '">');

		messageDom.push('<div class="announce-txt">');
		messageDom.push(data.Content);
		messageDom.push('</div></div>');

		return messageDom.join('');
	}

	let renderElementToMessageContainer = function (newElement, opts) {
		// Is the scroll position at the bottom append the element and update the scroll position to bottom
		let container = $(messageContainer);

		container.append(newElement);

		if (opts["from"] === "client") {
			container.scrollTop(container[0].scrollHeight);
		} else {
			let isScrollAtBottom = isScrollAtTheBottom();

			if (isScrollAtBottom) {
				container.scrollTop(container[0].scrollHeight);
			}
		}
	}

	let renderStatusFlag = function (data) {

		let container = $(messageContainer),
			element = null;

		// remove previous seen/sent flag
		container.find('.msg-status').remove();

		// append new seen/sent flag
		let statusFlagDom = [];
		statusFlagDom.push('<div class="msgRow msg-status d-flex flex-row justify-content-end w-100">');

		switch (data.Status.toString()) {
			case '0': statusFlagDom.push('<i class="fa-regular fa-circle-check" style="font-size: 13px;"></i>');
				break;
			case '1': statusFlagDom.push('<i class="fa-solid fa-circle-check" style="font-size: 13px;"></i>');
				break;
			case '2': statusFlagDom.push('<i class="fa-solid fa-circle-check" style="font-size: 13px; color: #008000"></i>');
				break;
		}

		statusFlagDom.push('</div>');

		if (data.Status.toString() === '0' && data.TempMessageId !== undefined && data.TempMessageId !== '') {
			container.find('#message' + data.TempMessageId).attr('id', 'message' + data.MessageId);
		}

		element = container.find('#message' + data.MessageId);

		element.removeClass('status--1 status-0 status-1 status-2');
		element.addClass('status-' + data.Status);
		element.after(statusFlagDom.join(''));
	}

	let isScrollAtTheBottom = function () {
		// verify that the scroll position for the container is at the bottom
		// (container elements height) - (number of px are scroll from top) == (container height)


		let container = $(messageContainer),
			adjustment = 5;

		let isScrollAtBottom = container[0].scrollHeight - container.scrollTop() <= (container.outerHeight() + adjustment);

		return isScrollAtBottom;
	};

	let sendMessageSeenResponse = function (msgId, status = 1) {
		var msgObj = {
			Type: "MessageStatus",
			Data: {
				MessageId: msgId,
				Status: status
			},
		};

		if (isScrollAtTheBottom() && !document.hidden) {
			msgObj.Data.Status = 2;
		}

		CimpleChat.Chat.MessageQueue.enqueue(msgObj);
	}

	return {
		init: function (messageContainerDiv) {
			messageContainer = messageContainerDiv;
		},
		clear: function () {
			$(messageContainer).html('');
		},
		renderMessage: function (response) {
			renderMessageToUI(response);
		},
		updateSeenFlag: function () {
			var msgId = '';

			//console.log(isScrollAtTheBottom() + " - " + document.activeElement.id);

			if (isScrollAtTheBottom() && !document.hidden) {
				var messages = $(messageContainer).find('.status-1');

				$.each(messages, function (key, value) {

					if ($(this).hasClass('status-1') && $(this).attr('data-uid') !== userId.toString()) {
						// send the seen response
						// set the flag as seen
						msgId = this.id.replace('message', '');
						renderStatusFlag({ MessageId: msgId, Status: 2});
						sendMessageSeenResponse(msgId, 2);
					}
				});				
			}
		}
	}
})();