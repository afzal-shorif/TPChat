CimpleChat.UserRegistration = (function () {
    let registrationModel = null;
    let isUsernameValid = false;
    let isGenderValid = false;
    let isAgeValid = false;
    let isUsernameAlreadyInUse = true;
    let initRegistration = function (containerId) {
        registrationModel = new bootstrap.Modal(containerId);

        if (!isCookieAvailable('userInfo')) {
            // show registration modal and attach submit button click event
            registrationModel.show();

            // add event to the input field

            $(document).on('keyup', '#inputUsername', validateUsername);
            $(document).on('focusout', '#inputUsername', validateUsernameIsAlreadyInUse)
            $(document).on('change', '#inputGender', validateGender);
            $(document).on('change', '#inputAge', validateAge);

            $(document).on('click', containerId + " #registerUserBtn", function (e) {
                if (isUsernameValid &&
                    isGenderValid &&
                    isAgeValid &&
                    !isUsernameAlreadyInUse) {
                        registerUser();
                    }           
            });
        }
    }

    let isCookieAvailable = function (cookeiName) {
        let cookieList = document.cookie.split(';');
        let flag = false;
        $.each(cookieList, function (index) {
            let cookeiPair = cookieList[index].trim().split('=');

            if (cookeiPair[0] === cookeiName) {

                if (typeof cookeiPair[1] != 'undefined' && cookeiPair[1] != '') {
                    flag = true;
                }
            }
        });

        return flag;
    }

    let registerUser = function (userName) {

        $.ajax({
            url: Urls.registration,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ UserName: userName }),
            success: function (response) {
                if (response.status === 'success') {
                    
                    let cookei = JSON.stringify(response.cookieInfo);
                    document.cookie = "userInfo=" + JSON.parse(cookei) + "; path=/";

                    registrationModel.hide();
                    document.location = document.location;
                } else {

                }
            },
            error: function (response) {
            }
        });
    }

    // user input validation

    let validateUsername = function () {
        let username = $('#inputUsername');
        var regex = /[^A-Za-z0-9]/;


        // length
        if (username.val().length < 3 || username.val().length > 18) {
            username.addClass('is-invalid');
            $('#usernameValidationMessage').text('Between 3 to 18 character.');
        } else if (regex.test(username.val())) {
            username.addClass('is-invalid');
            $('#usernameValidationMessage').text("Illegal characters.");
        } else {
            $('#usernameValidationMessage').text('');
            username.removeClass('is-invalid');
            username.addClass('is-valid');
        }
    }

    let validateGender = function () {

    }

    let validateAge = function(){

    }

    let validateUsernameIsAlreadyInUse = function () {

    }

	return {
        initRegistration: initRegistration,
	}
})(document);