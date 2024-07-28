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
        cursorInterval = null;

    var channelListShowHideEvent = function(){
       // bind the event for the arrow icon of public and private channel list
        
        $(dropdownIconPrivateElementClass).on('click', function(){
           toggleDropDownIcon(privateChannelListContainerId, dropdownIconPrivateElementClass);
        });

        $(dropdownIconPublicElementClass).on('click', function(){
            toggleDropDownIcon(publicChannelListContainerId, dropdownIconPublicElementClass);
        });
    }

    var toggleDropDownIcon = function(containerId, iconClass){
        $(containerId).toggle('slow');
        var icon = $(iconClass);
        
        if(icon.hasClass(faAngleUp.substring(1))){
            icon.removeClass(faAngleUp.substring(1));
            icon.addClass(faAngledown.substring(1));   
        }else { 
            icon.removeClass(faAngledown.substring(1));
            icon.addClass(faAngleUp.substring(1));
        }
    }
    
    return{
        adjustMainContainerHeight: function(){
            var navHeight = $('nav').first().outerHeight(true);
		    $(wrapperDivId).height(window.innerHeight - navHeight - 30 + 'px');
        },

        initEvents: function(){
            channelListShowHideEvent();        
        }
    }
})();