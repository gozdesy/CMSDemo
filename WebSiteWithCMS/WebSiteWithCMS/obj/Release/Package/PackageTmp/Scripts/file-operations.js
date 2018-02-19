var FileOperations = (function () {
    return {

        // Check for the various File API support.
        Check: function () {
            if (window.File && window.FileReader && window.FileList && window.Blob) {
                // Great success! All the File APIs are supported.
                return true;
            } else {
                alert('The File APIs are not fully supported in this browser.');
                return false;
            }
        },

        ReadFileAsDataURL: function(input, targetFunction) {
            if (input.files.length > 0) {
                var file = input.files[0];
                var reader = new FileReader();

                reader.addEventListener("load", targetFunction, false);

                if (file) {
                    reader.readAsDataURL(file);
                }
            }
        }

    };
})();

//function handleFileSelect(evt) {
//    var file = evt.target.file; // File

//    // files is a FileList of File objects. List some properties.
//    //var output = [];
//    //for (var i = 0, f; f = files[i]; i++) {
//    //    output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
//    //        f.size, ' bytes, last modified: ',
//    //        f.lastModifiedDate ? f.lastModifiedDate.toLocaleDateString() : 'n/a',
//    //        '</li>');
//    //}
//    //document.getElementById('list').innerHTML = '<ul>' + output.join('') + '</ul>';

//    document.getElementById('file_name').innerHTML = file.name;
//}

//document.getElementById('file').addEventListener('change', handleFileSelect, false);
