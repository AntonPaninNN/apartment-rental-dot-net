'use strict';

var app = angular.module("rentalApp", []);

app.run(function ($rootScope) {
    /*Page Elements*/
    $rootScope.hideCarousel = false;
    $rootScope.hideSidebar = true;
    $rootScope.hideDashboard = true;

    /*Dashboard Elements*/
    $rootScope.hideCustomersDashboard = true;
    $rootScope.hideApartmentsDashboard = true;
    $rootScope.customersDashboardSelected = false;
    $rootScope.apartmentsDashboardSelected = false;
});