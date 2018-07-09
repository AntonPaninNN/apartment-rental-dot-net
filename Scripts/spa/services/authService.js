'use strict';

app.factory("authService", authService);

authService.$inject = ['webUtilsService'];

function authService(webUtilsService) {

    var service = {
        login: login,
        signin: signin
    }

    function login(data, loginSuccess) {
        webUtilsService.post('/api/account/login', data, loginSuccess, loginFailure);
    }

    function signin(data, signinSuccess) {
        webUtilsService.post('/api/account/signin', data, signinSuccess, signinFailure);
    }

    function signinFailure(error) {
        alert(error);
    }

    function loginFailure(error) {
        alert(error);
    }

    return service;
}