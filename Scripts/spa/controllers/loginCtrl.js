'use strict';

app.controller("loginController", loginCtrl);

loginCtrl.$inject = ['$scope', 'authService'];

function loginCtrl($scope, authService) {

    $scope.showLoginPopup = function () {
        $scope.hideCarousel = true;
        setVisible('#loginbox', true);
        setVisible('#signupbox', false);
    }

    $scope.showSignupPopup = function () {
        $scope.hideCarousel = true;
        setVisible('#signupbox', true);
        setVisible('#loginbox', false);
    }

    $scope.login = function () {
        var user = {
            HashedPassword: $scope.loginData.password,
            Login: $scope.loginData.email
        };
        authService.login(user, loginSuccess);
    }

    $scope.signin = function () {
        var user = {
            HashedPassword: $scope.signinData.password,
            Login: $scope.signinData.email,
            Salt: 'testSalt',
            CreationDate: (new Date()).toISOString()
        };
        authService.signin(user, signinSuccess)
    }

    function signinSuccess(result) {
        $scope.hideDashboard = false;
        $scope.hideSidebar = false;
        setVisible('#signupbox', false);
        setVisible('#loginbox', false);
    }

    function loginSuccess(result) {
        $scope.hideDashboard = false;
        $scope.hideSidebar = false;
        setVisible('#signupbox', false);
        setVisible('#loginbox', false);
    }

    function setVisible(element, isVisible) {
        if (isVisible && !$(element).is(':visible'))
            $(element).show();
        else if (!isVisible && $(element).is(':visible'))
            $(element).hide();
    }

}