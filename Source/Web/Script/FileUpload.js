function UploadConfig() {
}

UploadConfig.prototype.ContainerId = null; //容器编号
UploadConfig.prototype.Url = null; //处理地址
UploadConfig.prototype.RunTimeTypes = null; //运行时设置
UploadConfig.prototype.MaxFileSize = "10m"; //上传文件大小设置
UploadConfig.prototype.ChunkSize = "1m"; //分块大小
UploadConfig.prototype.Width = 1024; //默认图片宽度
UploadConfig.prototype.Heigh = 768; //默认图片高度
UploadConfig.prototype.PlUploadFolder = null; //文件上传组件路径
UploadConfig.prototype.UniqueNames = false; //上传时文件名称唯一
UploadConfig.prototype.Filters = null; //允许上传的文件类型设置
UploadConfig.prototype.CompleteEvent = null;
UploadConfig.prototype.ErrorEvent = null;
UploadConfig.prototype.ProgressEvent = null;
UploadConfig.prototype.AddedFileEvent = null;
UploadConfig.prototype.BrowButtonId = null;
UploadConfig.prototype.FileListId = null;

//Plupload控件上传
function PluploadProcessFile(uploadConfig) {
    if (uploadConfig.Width == 0) {
        uploadConfig.Width = 1024;
    }
    if (uploadConfig.Heigh == 0) {
        uploadConfig.Heigh = 768;
    }
    this._uploadConfig = uploadConfig;
    var uploader = $('#' + uploadConfig.ContainerId).pluploadQueue({
        runtimes: uploadConfig.RunTimeTypes,
        url: uploadConfig.Url,
        //browse_button: 'btnBrower_plupload', 该模式下此按钮设置无效
        max_file_size: uploadConfig.MaxFileSize,
        chunk_size: uploadConfig.ChunkSize,
        unique_names: uploadConfig.UniqueNames,
        resize: { width: uploadConfig.Width, height: uploadConfig.Heigh, quality: 100 },
        filters: uploadConfig.Filters,
        silverlight_xap_url: uploadConfig.PlUploadFolder + '/plupload.silverlight.xap',
        flash_swf_url: uploadConfig.PlUploadFolder + '/plupload.flash.swf',

        preinit: {
            Init: function (up, info) {

            },
            UploadFile: function (up, file) {
            }
        },
        init: {
            Refresh: function (up) {
                // Called when upload shim is moved
                //log('[Refresh]');
            },

            QueueChanged: function (up) {
            },
            UploadProgress: function(up,file) {
                if (uploadConfig.ProgressEvent) {
                    uploadConfig.ProgressEvent(file);
                }
            },
            FileUploaded: function (up, file, info) {
                if (up.state == 2 && up.total.queued == 0 && uploadConfig.CompleteEvent) {
                    uploadConfig.CompleteEvent();
                }
            },

            ChunkUploaded: function (up, file, info) {
                // Called when a file chunk has finished uploading
                // log('[ChunkUploaded] File:', file, "Info:", info);
            },
            Error: function (up, args) {
                if (args.code == 9999) {
                    if (uploadConfig.ErrorEvent) {
                        uploadConfig.ErrorEvent(args);
                        return;
                    }
                }
            },
            FilesAdded: function (up, files) {
                if (uploadConfig.AddedFileEvent) {
                    $.each(files, function (i, file) {
                        uploadConfig.AddedFileEvent(i, file);
                    });
                }
            }
        }
    });
}

function SingleFileUpload(uploadConfig) {
    if (uploadConfig.Width == 0) {
        uploadConfig.Width = 1024;
    }
    if (uploadConfig.Heigh == 0) {
        uploadConfig.Heigh = 768;
    }
    this._uploadConfig = uploadConfig;
    
    var uploader = new plupload.Uploader({
        runtimes: uploadConfig.RunTimeTypes,
        browse_button: uploadConfig.BrowButtonId,
        container: uploadConfig.ContainerId,
        max_file_size: uploadConfig.MaxFileSize,
        chunk_size: uploadConfig.ChunkSize,
        unique_names: uploadConfig.UniqueNames,
        multi_selection: false,
        url: uploadConfig.Url,
        flash_swf_url: uploadConfig.PlUploadFolder + '/plupload.flash.swf',
        silverlight_xap_url: uploadConfig.PlUploadFolder + '/plupload.silverlight.xap',
        resize: { width: uploadConfig.Width, height: uploadConfig.Heigh, quality: 90 },
        filters: uploadConfig.Filters
    });

    uploader.bind('Init', function (up, params) {
        $("#" + uploadConfig.FileListId).html("<b>请选择需要上传的文件</b>");
    });

    this.uploadFile = function () {
        uploader.start();
    };
    this.hasFile = function () {
        return uploader.files.length == 1;
    };

    uploader.init();

    uploader.bind('FilesAdded', function (up, files) {
        if (uploader.files.length > 1) {
            uploader.removeFile(uploader.files[0]);
        }
        if (uploadConfig.AddedFileEvent) {
            $.each(files, function (i, file) {
                uploadConfig.AddedFileEvent(i, file);
            });
        }
        up.refresh(); // Reposition Flash/Silverlight
    });

    uploader.bind('UploadProgress', function (up, file) {
        if (uploadConfig.ProgressEvent) {
            uploadConfig.ProgressEvent(file);
        }
    });

    uploader.bind('Error', function (up, err) {
        if (err.code == 9999) {
            if (uploadConfig.ErrorEvent) {
                uploadConfig.ErrorEvent(err);
                return;
            }
        }
        alert(err.message);
        up.refresh();
    });

    uploader.bind('StateChanged', function (up) {
        if (up.state == plupload.STOPPED) {
            uploadConfig.CompleteEvent();
        }
    });
};