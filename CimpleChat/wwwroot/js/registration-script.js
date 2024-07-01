CimpleChat.UserRegistration = (function () {
    let registrationModel = null;
    let isUsernameValid = false;
    let isGenderValid = false;
    let isAgeValid = false;
    let isUsernameAlreadyInUse = true;
    let initRegistration = function (containerId) {
        registrationModel = new bootstrap.Modal(containerId);

        if (!isCookieAvailable('userInfo')) {
            // show registration modal and attach event to the registration modal component (submit button, input fields)
            registrationModel.show();

            $(document).on('keyup', '#inputUsername', validateUsername);
            //$(document).on('focusout', '#inputUsername', validateUsernameIsAlreadyInUse);
            $(document).on('keyup', '#inputAge', validateAge);

            $(document).on('click', containerId + " #registerUserBtn", function (e) {
                e.preventDefault();

                validateUsername();
                validateGenger();
                validateAge();

                if (isUsernameValid && isGenderValid && isAgeValid) {
                    validateUsernameIsAlreadyInUse();
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

    let registerUser = function () {
        let data = {
            UserName: $('#inputUsername').val(),
            Gender: $($("input[name=inputGender]")[0]).prop('checked') ? 0 : 1,
            Age: $('#inputAge').val()
        }

        $.ajax({
            url: Urls.registration,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.status === 'success') {
                    
                    let cookei = JSON.stringify(response.cookieInfo);
                    document.cookie = "userInfo=" + JSON.parse(cookei) + "; path=/";
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
        var regex = /^[a-zA-Z]([a-zA-Z0-9]){2,17}$/;

        if (!regex.test(username.val())) {
            username.addClass('is-invalid');
            $('#usernameValidationMessage').text("Illegal characters.");
            isUsernameValid = false;
        } else {
            $('#usernameValidationMessage').text('');
            username.removeClass('is-invalid');
            username.addClass('is-valid');
            isUsernameValid = true;
        }
    }

    let validateGenger = function() {
        let radioButtons = $("input[name=inputGender]");
        $.each(radioButtons, function (index) {
            if ($(radioButtons[index]).prop("checked")) {
                isGenderValid = true;
            }
            $(radioButtons[index]).removeClass('is-invalid');
        });

        if (!isGenderValid) {
            $.each(radioButtons, function (index) {
                $(radioButtons[index]).addClass('is-invalid');               
            });
        }
    }

    let validateAge = function(){
        let ageDom = $('#inputAge');
        let age = parseInt(ageDom.val());

        if (age > 16 && age < 100) {
            ageDom.removeClass('is-invalid');
            ageDom.addClass('is-valid');
            isAgeValid = true;
        } else {
            ageDom.removeClass('is-valid');
            ageDom.addClass('is-invalid');
            isAgeValid = false;
        } 
    }

    let validateUsernameIsAlreadyInUse = function () {
        $.ajax({
            url: Urls.validateUniqueUsername,
            type: 'GET',
            contentType: 'application/json',
            data: { username: $("#inputUsername").val() },
            success: function (response) {

                if (response.status === 'success') {
                    registerUser();
                } else {
                    let userInput = $("#inputUsername");
                    userInput.addClass('is-invalid');
                    $('#usernameValidationMessage').text("Username is already in used.");
                }
            },
            error: function (response) {
            }
        });
    }

	return {
        initRegistration: initRegistration,
        isCookieAvailable: isCookieAvailable
	}
})(document);