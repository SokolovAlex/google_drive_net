(function (angular) {
    var fileService = angular.module('fileService', ['angularFileUpload']);
    fileService.factory('fileService', [
        '$http', function($http) {
            return {
                getFiles: function (parent) {
                    var url = window.config.urls.getFile;
                    url = parent ? url + '/' + parent : url;
                    return $http.get(url);
                },
                deleteFile: function(id) {
                    return $http.delete(window.config.urls.remove + '/' + id);
                },
                createFolder: function(data) {
                    return $http.post(window.config.urls.createFolder, data);
                },
                selectFolder: function(data) {
                    return $http.post(window.config.urls.createFolder, data);
                },
                downloadFile: function(id) {
                    return $http.get(window.config.urls.download + '/' + id);
                },
                share: function (id) {
                    return $http.post(window.config.urls.share + '/' + id);
                }
            };
        }
    ]);

    fileService.controller('UploadController', ['$scope', 'FileUploader', function ($scope, FileUploader) {
        var uploader = $scope.uploader = new FileUploader({
            url: window.config.urls.upload
        });

        uploader.filters.push({
            name: 'customFilter',
            fn: function(item, options) {
                return this.queue.length < 10;
            }
        });

        // CALLBACKS

        uploader.onWhenAddingFileFailed = function(item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
        };
        uploader.onAfterAddingFile = function(fileItem) {
            console.info('onAfterAddingFile', fileItem);
        };
        uploader.onAfterAddingAll = function(addedFileItems) {
            console.info('onAfterAddingAll', addedFileItems);
        };
        uploader.onBeforeUploadItem = function(item) {
            console.info('onBeforeUploadItem', item);
        };
        uploader.onProgressItem = function(fileItem, progress) {
            console.info('onProgressItem', fileItem, progress);
        };
        uploader.onProgressAll = function(progress) {
            console.info('onProgressAll', progress);
        };
        uploader.onSuccessItem = function(fileItem, response, status, headers) {
            console.info('onSuccessItem', fileItem, response, status, headers);
        };
        uploader.onErrorItem = function(fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);
        };
        uploader.onCancelItem = function(fileItem, response, status, headers) {
            console.info('onCancelItem', fileItem, response, status, headers);
        };
        uploader.onCompleteItem = function(fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);
        };
        uploader.onCompleteAll = function() {
            console.info('onCompleteAll');
        };

        console.info('uploader', uploader);
    }]);

})(angular);