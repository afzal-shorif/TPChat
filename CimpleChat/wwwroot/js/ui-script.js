CimpleChat.UI = (function(){
    var wrapperDivId = '#wrapper',
        dropdownIconPrivateElementClass = '.dropdown-arrow-private',
        dropdownIconPublicElementClass = '.dropdown-arrow-public',
        privateChannelListContainerId = '#privateChannelListContainer',
        publicChannelListContainerId = '#publicChannelListContainer'
        faAngleUp = '.fa-angle-up',
        faAngledown = '.fa-angle-down',
        inputCursorElementId = '#inputCursor',
        msgInputField = '#msgInputField',
        cursorInterval = null,
        searchField = '#searchField',
        suggestionContainer = '#searchSuggestionContainer',
        addMemberModal = null;

    var channelListShowHideEvent = function(){
       // bind the event for the arrow icon of public and private channel list
        
        $(dropdownIconPrivateElementClass).on('click', function(){
           toggleDropDownIcon(privateChannelListContainerId, dropdownIconPrivateElementClass);
        });

        $(dropdownIconPublicElementClass).on('click', function(){
            toggleDropDownIcon(publicChannelListContainerId, dropdownIconPublicElementClass);
        });
    }

    var toggleDropDownIcon = function (containerId, iconClass) {
        $(containerId).toggle('slow');
        var icon = $(iconClass);

        if (icon.hasClass(faAngleUp.substring(1))) {
            icon.removeClass(faAngleUp.substring(1));
            icon.addClass(faAngledown.substring(1));
        } else {
            icon.removeClass(faAngledown.substring(1));
            icon.addClass(faAngleUp.substring(1));
        }
    };

    var searchChannelEvent = function () {
        var inputField = $(searchField);
        inputField.on('keydown', function () {
            if (inputField.val().length > 2) {
                CimpleChat.Channel.search(inputField.val());
                $('#searchField').addClass('activeSearchInputField');
            }
        });

        //inputField.on('blur', function () {
        //    $(suggestionContainer).hide();
        //    $(suggestionContainer).html('');
        //});
    };

    var searchChannelResultEvent = function () {
        $(document).on('click', '#searchSuggestionContainer .list-group-item', function (e) {
            CimpleChat.Channel.addMemberToChannel( $(this).attr('data-id'), '');

            $(suggestionContainer).hide();
            $(suggestionContainer).html('');
            $('#searchField').removeClass('activeSearchInputField');
        });
    };

    var createPrivateChannel = function () {
        $('#createPrivateChannel').on('click', function () {
            $('#channelNameInput').val('');
            $('#channelTypeInput').val('Private');
            $('#channelNameErrorMsg').text('');
            $('#createChannelModal').modal('show');
        });
    };

    var createPublicChannel = function () {
        $('#createPublicChannel').on('click', function () {
            $('#channelNameInput').val('');
            $('#channelTypeInput').val('Public');
            $('#channelNameErrorMsg').text('');
            $('#createChannelModal').modal('show');
        });
    };

    var createChannelButtonEvent = function () {
        $('#createChannelBtn').on('click', CimpleChat.Channel.createChannel);
    }
    
    return{
        adjustMainContainerHeight: function(){
            var navHeight = $('nav').first().outerHeight(true);
		    $(wrapperDivId).height(window.innerHeight - navHeight - 30 + 'px');
        },

        initEvents: function(){
            channelListShowHideEvent();
            $(document).on('click', '.channel-list .list-group-item', function () {
                $('.channel-list .active').removeClass('active');
                $(this).addClass('active');
                CimpleChat.Channel.getMessageHistory($(this).attr('data-channel'));
                $('.msg-header h5').first().text($(this).attr('data-name'));
            });

            searchChannelEvent();
            searchChannelResultEvent();

            $(document).on('click', function () {
                $(suggestionContainer).hide();
                $(suggestionContainer).html('');
                $('#searchField').removeClass('activeSearchInputField');
            });

            createPrivateChannel();
            createPublicChannel();
            createChannelButtonEvent();

            $('#addMemberBtn').on('click', function () {
                if (addMemberModal == null) {
                    addMemberModal = new bootstrap.Modal(document.getElementById('addMemberModal'));
                }

                $('#addMemberSearchInput').val('');
                addMemberModal.show();
            });
            
            $('#addMemberSearchInput').on('keyup', function(){
                var searchText = $(this).val();
                
                if(searchText.length > 2){
                   $.ajax({
				        url: Urls.searchUser,
				        data: { name: searchText},
				        type: 'GET',
				        success: function (result) {
                            var container = $('#addMemberSearchResultContainer');
                            console.log(result);
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
                }
            });

            $(document).on('click', '#addMemberSearchResultContainer .list-group-item', function (e) {
                var suggestionContainer = '#addMemberSearchResultContainer';

                CimpleChat.Channel.addMemberToChannel('', $(this).attr('data-id'));

                $(suggestionContainer).hide();
                $(suggestionContainer).html('');
            });
        }
    }
})();