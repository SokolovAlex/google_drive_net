(function (angular) {
    var main = angular.module('main', ['fileService']);

    main.controller('mainCtrl', ['$scope', 'fileService',
         function ($scope, fileService) {
             var ctrl = this;
             ctrl.files = [];
             debugger;
             fileService.getFiles().then(function (response) {
                 debugger;
                 ctrl.files = response.data;
             });
         }]);

    angular.element(document).ready(function () {
        angular.bootstrap(document, ['main']);
    });
})(angular);