(function (angular) {
    var main = angular.module('main', ['fileService']);

    main.controller('mainCtrl', ['$scope', 'fileService',
         function ($scope, fileService) {
             var ctrl = this;
             var rootFolder = {
                 Name: 'Root',
                 Id: null
             };

             ctrl.inited = false;

             ctrl.files = [];
             ctrl.folders = [];

             ctrl.refresh = refresh;
             ctrl.deleteFile = deleteFile;
             ctrl.createFolder = createFolder;
             ctrl.selectFolder = selectFolder;
             ctrl.download = download;
             ctrl.share = fileService.share;

             ctrl.newFolderName = '';
             ctrl.newFolderDesc = '';
             ctrl.selectedFolder = rootFolder;

             refresh();

             function download(file) {
                 window.open(file.WebContentLink);
             }

             function selectFolder(folder) {
                 ctrl.selectedFolder = folder || rootFolder;
                 refresh();
             }

             function refresh() {
                 var parent = ctrl.selectedFolder.Id || null;
                 fileService.getFiles(parent).then(function (response) {
                     console.log('refresh --> ', response);
                     ctrl.files = response.data.files;
                     ctrl.folders = response.data.folders;
                     ctrl.gallery = prepareGallery(ctrl.files);
                     ctrl.inited = true;
                 });
             }

             function prepareGallery(files) {
                 var images = _.filter(files, function(item) {
                     return item.MimeType.indexOf('image') !== -1;
                 });
                 return _.map(images, function (file, i) {
                     return {
                         num: i,
                         className: i === 0 ? 'active' : '',
                         name: file.Name,
                         src: file.WebContentLink
                    };
                 });
             }

             function createFolder() {
                 if (!ctrl.newFolderName) return;
                 var data = {
                     folderName: ctrl.newFolderName,
                     folderDesc: ctrl.newFolderDesc,
                     parentId: ctrl.selectedFolder.Id
                 };
                 fileService.createFolder(data).then(function() {
                     ctrl.newFolderName = "";
                     ctrl.newFolderDesc = "";
                     refresh();
                 });
             }

             function deleteFile(id) {
                 fileService.deleteFile(id).then(refresh);
             }
         }]);

    angular.element(document).ready(function () {
        angular.bootstrap(document, ['main']);
    });
})(angular);