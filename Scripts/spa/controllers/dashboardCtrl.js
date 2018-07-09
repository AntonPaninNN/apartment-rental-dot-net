'use strict';

app.controller('dashboardController', dashboardController);

dashboardController.$inject = ['$scope'];

function dashboardController($scope) {

    $scope.openCustomersDashboard = function () {
        $scope.hideCustomersDashboard = false;
        $scope.customersDashboardSelected = true;
        $scope.hideApartmentsDashboard = true;
        $scope.apartmentsDashboardSelected = false;
    }

    $scope.openApartmentsDashboard = function () {
        $scope.hideApartmentsDashboard = false;
        $scope.apartmentsDashboardSelected = true;
        $scope.hideCustomersDashboard = true;
        $scope.customersDashboardSelected = false;
    }

}