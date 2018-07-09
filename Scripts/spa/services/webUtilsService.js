'use strict';

app.factory("webUtilsService", webUtilsService);

webUtilsService.$inject = ['$http'];

function webUtilsService($http) {

    var service = {
        post: post
    }

    function post(url, data, success, failure) {
        return $http.post(url, data).
        then(function (result) {
            success(result);
        }, function (error) {
            failure(error);
        });
    }

    return service;

}