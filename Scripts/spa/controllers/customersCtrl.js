'use strict';

app.controller('customersController', customersController);

customersController.$inject = ['$scope', 'webUtilsService'];

function customersController($scope, webUtilsService) {

    var selectedIds = [];
    var currentObjects = [];

    $scope.creationPopup = false;
    $scope.showCreationPopup = function () {
        $scope.creationPopup = true;
    }

    $scope.deletePopup = false;
    $scope.showDeletePopup = function () {
        $scope.deletePopup = true;
    }

    $scope.updatePopup = false;
    $scope.showUpdatePopup = function () {
        $scope.updatePopup = true;
    }

    $scope.filterPopup = false;
    $scope.showFilterPopup = function () {
        $scope.filterPopup = true;
    }

    $scope.sortPopup = false;
    $scope.showSortPopup = function () {
        $scope.sortPopup = true;
    }

    $scope.currentSorting = { fieldName: undefined, direction: undefined };
    $scope.filterCustomer = { firstName: {}, lastName: {}, email: {}, mobile: {}, passportData: {}, notes: {} };
    $scope.sortCustomer = {
        firstName: { priority: 0 }, lastName: { priority: 0 }, email: { priority: 0 },
        dateOfBirth: { priority: 0 }, mobile: { priority: 0 }, passportData: { priority: 0 }, notes: { priority: 0 }
    };

    $scope.$on('$includeContentLoaded', function (event, url) {
        if (url == 'commonViews/customersDashboard.html')
            loadCustomersDashboard();
        else if (url == 'commonViews/customers/create.html')
            $('#exampleModal').modal('show');
        else if (url == 'commonViews/customers/delete.html')
            $('#deleteModal').modal('show');
        else if (url == 'commonViews/customers/update.html')
            $("#updateModal").modal('show');
        else if (url == 'commonViews/customers/filter.html')
            $('#filterModal').modal('show');
        else if (url == 'commonViews/customers/sort.html') 
            $('#sortModal').modal('show');
    });

    function loadCustomersDashboard() {
        webUtilsService.post('/api/customers/range', {
            PageNum: 1, PageSize: 10,
            FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
        }, success, failure);

        function initTable() {
            /*currentObjects = data;*/
            $('#customersTable').bootstrapTable({
                sidePagination: 'server',
                columns: [{
                    field: 'state',
                    title: '',
                    checkbox: true
                }, {
                    field: 'ID',
                    title: '',
                    visible: false
                }, {
                    field: 'firstName',
                    title: 'First Name',
                    sortable: true
                }, {
                    field: 'lastName',
                    title: 'Last Name',
                    editable: {
                        type: 'text'
                    },
                    sortable: true
                }, {
                    field: 'registrationDate',
                    title: 'Registration Date',
                    sortable: true
                },
                {
                    field: 'email',
                    title: 'Email',
                    sortable: true
                },
                {
                    field: 'dayOfBirth',
                    title: 'Date Of Birth',
                    sortable: true
                },
                {
                    field: 'mobile',
                    title: 'Mobile',
                    sortable: true
                },
                {
                    field: 'passportData',
                    title: 'Passport Data',
                    sortable: true
                },
                {
                    field: 'notes',
                    title: 'Notes',
                    sortable: true
                }]
            });
        }

        function success(result) {
            initTable();
            reloadData(getDataFromResponse(result.data));
            $('#customersTable').on('check.bs.table', function (row, $element) {
                selectedIds.push($element.ID);
            });
            $('#customersTable').on('uncheck.bs.table', function (row, $element) {
                var index = selectedIds.indexOf($element.ID);
                if (index > -1)
                    selectedIds.splice(index, 1);
            });
            $('#customersTable').on('editable-save.bs.table', function (editable, field, row, oldValue, $el) {
                var data = prepareNewData(row);
                data.ID = row.ID;
                selectedIds = [];
                edit(data);

                console.log(row);
            });
            $('#customersTable').on('page-change.bs.table', function (event, number, size) {
                webUtilsService.post('/api/customers/range', {
                    PageNum: number, PageSize: size,
                    FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
                }, success, failure);

                function success(result) {
                    reloadData(getDataFromResponse(result.data));
                }

                function failure(error) {
                    console.log(error);
                }

            });
            $('#customersTable').on('sort.bs.table', function (obj, name, order) {
                $scope.currentSorting.fieldName = name;
                $scope.currentSorting.direction = order;
                $scope.sort();
            });
        }

        function failure(error) {
            console.log(error);
        }

        $scope.newCustomer = {};
    }

    function getDataFromResponse(result) {
        var data = [];
        for (var i = 0; i < result.Items.length; i++) {
            data.push({
                ID: result.Items[i].ID,
                firstName: result.Items[i].FirstName,
                lastName: result.Items[i].LastName,
                registrationDate: result.Items[i].RegistrationDate,
                email: result.Items[i].Email,
                dayOfBirth: result.Items[i].DateOfBirth,
                mobile: result.Items[i].Mobile,
                passportData: result.Items[i].PassportData,
                notes: result.Items[i].Notes
            });
        }
        return { total: result.AllItemsCount, rows: data };
    }

    function reloadData(data) {
        currentObjects = data;
        $('#customersTable').bootstrapTable('load', data);
    }

    $scope.deleteSelectedCustomers = function () {
        if (selectedIds.length > 0) {
            var pageSize = $('.page-size').text();
            var pageNum = $('.page-number.active').text();
            webUtilsService.post('/api/customers/delete', {
                IDs: selectedIds, PageSize: pageSize, PageNum: pageNum,
                FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
            }, success, failure);
        }
        selectedIds = [];
        $('#deleteModal').modal('toggle');

        function success(result) {
            reloadData(getDataFromResponse(result.data));
        }

        function failure(error) {
            console.log(error);
        }
    }

    $scope.editCustomer = function () {
        if (selectedIds.length < 1) {
            alert('Select some customer first');
            return;
        }
        else if (selectedIds.length > 1) {
            alert('Select just one customer for editing');
            return;
        }
        
        var selectedObject = getSelectedObject();
        if (!selectedObject)
            return;

        $scope.editCustomer.firstName = selectedObject.firstName;
        $scope.editCustomer.lastName = selectedObject.lastName;
        $scope.editCustomer.dayOfBirth = new Date(selectedObject.dayOfBirth);
        $scope.editCustomer.email = selectedObject.email;
        $scope.editCustomer.passportData = selectedObject.passportData;
        $scope.editCustomer.notes = selectedObject.notes;
        $scope.editCustomer.mobile - selectedObject.mobile;

        if (!$scope.updatePopup)
            $scope.showUpdatePopup();
        else
            $("#updateModal").modal("show");
    }

    /* sort */
    $scope.sort = function() {
        var pagesize = $('.page-size').text();
        var pageNum = $('.page-number.active').text();
        webUtilsService.post('/api/customers/sort', {PageSize: pagesize, PageNum: pageNum,
            FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
        }, success, failure);

        function success(result) {
            reloadData(getDataFromResponse(result.data));
        }

        function failure(error) {
            console.log(error);
        }
    }

    /* edit */
    function edit(data) {
        var pagesize = $('.page-size').text();
        var pageNum = $('.page-number.active').text();
        webUtilsService.post('/api/customers/update', {
            Item: data, PageSize: pagesize, PageNum: pageNum,
            FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
        }, success, failure);

        function success(result) {
            reloadData(getDataFromResponse(result.data));
        }

        function failure(error) {
            console.log(error);
        }
    }

    /* clear filter */
    $scope.clearFilter = function () {
        var pageSize = $('.page-size').text();
        var pageNum = $('.page-number.active').text();
        webUtilsService.post('/api/customers/clearfilter', {
            PageSize: pageSize, PageNum: pageNum,
            FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
        }, success, failure);

        function success(result) {
            reloadData(getDataFromResponse(result.data));
        }

        function failure(error) {
            console.log(error);
        }
    }

    /* filter */
    $scope.sendFilterCustomer = function () {
        var data = prepareFilterData($scope.filterCustomer);

        var pageSize = $('.page-size').text();
        var pageNum = $('.page-number.active').text();
        webUtilsService.post('/api/customers/filter', {
            Data: data, PageSize: pageSize, PageNum: pageNum,
            FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
        }, success, failure);

        $('#filterModal').modal('toggle');

        function success(result) {
            reloadData(getDataFromResponse(result.data));
        }

        function failure(error) {
            console.log(error);
        }
    }

    /* edit */
    $scope.sendEditedCustomer = function () {
        var data = prepareNewData($scope.editCustomer);
        data.ID = getSelectedObject().ID;

        selectedIds = [];
        $('#updateModal').modal('toggle');

        edit(data);
    }

    /* create */
    $scope.registerNewCustomer = function () {

        var data = prepareNewData($scope.newCustomer);
        data.RegistrationDate = new Date().toISOString();

        var pagesize = $('.page-size').text();
        var pageNum = $('.page-number.active').text();
        webUtilsService.post('/api/customers/register', {
            Item: data, PageSize: pagesize, PageNum: pageNum,
            FieldName: $scope.currentSorting.fieldName, Direction: $scope.currentSorting.direction
        }, success, failure);

        $('#exampleModal').modal('toggle');

        function success(result) {
            reloadData(getDataFromResponse(result.data));
        }

        function failure(error) {
            console.log(error);
        }
    }

    /* util */
    function getSelectedObject() {
        var index;
        for (index = 0; index < currentObjects.rows.length; index++)
            if (currentObjects.rows[index].ID === selectedIds[0]) {
                return currentObjects.rows[index];
            }
    }

    /* util */
    function prepareDate(value) {
        var date;
        if (value) {
            if (value instanceof Date) {
                date = value.toISOString();
            } else {
                date = new Date(value).toISOString();
            }
        }
        return date;
    }

    /* util */
    function prepareFilterData(model) {
        var fromDate = prepareDate(model.fromDayOfBirth);
        var toDate = prepareDate(model.toDayOfBirth);

        return {
            FirstName: { Condition: model.firstName.condition, Value: model.firstName.value },
            LastName: { Condition: model.lastName.condition, Value: model.lastName.value },
            Email: { Condition: model.email.condition, Value: model.email.value },
            DateOfBirth: { From: fromDate, To: toDate },
            Mobile: { Condition: model.mobile.condition, Value: model.mobile.value },
            PassportData: { Condition: model.passportData.condition, Value: model.passportData.value },
            Notes: { Condition: model.notes.condition, Value: model.notes.value }
        };
    }

    /* util */
    function prepareNewData(model) {
        var date = prepareDate(model.dayOfBirth);
       
        return {
            FirstName: model.firstName,
            LastName: model.lastName,
            DateOfBirth: date,
            Email: model.email,
            PassportData: model.passportData,
            Notes: model.notes,
            Mobile: model.mobile
        };
    }

    $scope.openCreationPopup = function () {
        alert('Creation Popup');
    }

}