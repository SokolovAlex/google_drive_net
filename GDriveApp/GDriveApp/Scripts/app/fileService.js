(function (angular) {
    var fileService = angular.module('fileService', []);

    fileService.factory('fileService', ['$http', function ($http) {
        return {
            getFiles: function () {
                debugger;
                return $http.get('api/files');
            }
        };
    }]);
})(angular);