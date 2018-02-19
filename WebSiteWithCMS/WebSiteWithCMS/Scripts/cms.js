var DataLayer = (function () {
    var Data = {
        Elements: [],
        Images:[]
    };

    var elementData = function (pageid, id, content) {
        this.pageid = pageid,
        this.id = id,
        this.content = content
    };

    var imageData = function (pageid, id, originalUrl, updatedFileName) {
        this.pageid = pageid,
        this.id = id,
        this.originalUrl = originalUrl,
        this.updatedFileName = updatedFileName
    };

    var imageFiles = [];
    var imageFile = function (id, file, updatedFileName) {
        this.id = id,
        this.file = file;
        this.updatedFileName = updatedFileName
    };

    var ComponentElement = function (id, parentId, name) {
        this.id = id,
        this.parentId = parentId,
        this.name = name
    };

    var Component = function (id, componentElements) {
        this.id = id,
        this.componentElements = componentElements
    };
    var Components = [];

    return {
        Clear: function () {
            Data.Elements = [];
            Data.Images = [];
            imageFiles = [];
        },

        addToComponents: function(parentid, id, name) {
            //Parent
            var parentids = Components.map(function (current) { return current.id });
            var parentpos = parentids.indexOf(parentid);

            var component = new Component(parentid, []);
            if (parentpos === -1) {
                Components.push(component);
            }
            else {
                component = Components[parentpos];
            }

            //Children
            var childids = component.componentElements.map(function (current) { return current.id });
            var childpos = childids.indexOf(id);

            var componentElement = new ComponentElement(id, parentid, name);
            if (childpos === -1) {
                component.componentElements.push(componentElement);
            }
        },

        getComponents: function () {
            return Components;
        },

        getComponentElementId: function (parentid, name) {
            var parentids = Components.map(function (current) { return current.id });
            var parentpos = parentids.indexOf(parentid);

            if (parentpos === -1) {
                return "not found";
            }
            else {
                component = Components[parentpos];
            }

            var childnames = component.componentElements.map(function (current) { return current.name });
            var childpos = childnames.indexOf(name);

            if (childpos === -1) {
                return "not found";
            }
            else {
                return component.componentElements[childpos].id;
            }
        },

        getParentElementId: function (childid) {
            for (var i = 0; i < Components.length; i++) {
                var parent = Components[i];
                var childids = parent.componentElements.map(function (current) { return current.id });
                var childpos = childids.indexOf(childid);

                if (childpos !== -1) {
                    return parent.componentElements[childpos].parentId;
                }
            }
            return "";
        },

        writeUpdatedContent: function (pageid, id, content) {
            var ed = new elementData(pageid, id, content);

            var ids = Data.Elements.map(function (current) {
                return current.id;
            });

            var position = ids.indexOf(ed.id);

            if (position === -1) {
                Data.Elements.push(ed);
            }
            else {
                Data.Elements[position].content = ed.content;
            }
        },

        writeUpdatedImage: function (pageid, id, originalUrl, updatedFileName, file) {
            var imagedata = new imageData(pageid, id, originalUrl, updatedFileName);
            var image = new imageFile(id, file, updatedFileName);

            var ids = Data.Images.map(function (current) {
                return current.id;
            });
            var ids2 = imageFiles.map(function (current) {
                return current.id;
            });

            var position = ids.indexOf(imagedata.id);
            var position2 = ids2.indexOf(imagedata.id);

            if (position === -1) {
                if (updatedFileName != "") Data.Images.push(imagedata);
            }
            else {
                if (updatedFileName == "") {
                    Data.Images.splice(position, 1);
                }
                else {
                    Data.Images[position].updatedFileName = imagedata.updatedFileName;
                }
            }

            if (position2 === -1) {
                if (updatedFileName != "") imageFiles.push(image);
            }
            else {
                if (updatedFileName == "") {
                    imageFiles.splice(position2, 1);
                }
                else {
                    imageFiles[position2].file = file;
                    imageFiles[position2].updatedFileName = updatedFileName;
                }
            }

            console.log(Data.Images);
        },

        getDataJSON: function () {
            return JSON.stringify(Data);
        },

        getImageFiles: function () {
            return imageFiles;
        },

        getOriginalURL: function (id) {
            var ids = Data.Images.map(function (current) {
                return current.id;
            });
            
            var position = ids.indexOf(id);
            if (position === -1) {
                return "";
            }
            else {
                return Data.Images[position].originalUrl;
            }
        }
    };
})();

var ControllerLayer = (function () {

    function setUpEventHandlers() {
        var editButton = document.getElementById(UserInterfaceLayer.GetDomIds().editButton);
        if (editButton) editButton.addEventListener("click", switchToDesignMode);

        var saveButton = document.getElementById(UserInterfaceLayer.GetDomIds().saveButton);
        if (saveButton) saveButton.addEventListener("click", save);

        var backButton = document.getElementById(UserInterfaceLayer.GetDomIds().backButton);
        if (backButton) backButton.addEventListener("click", switchToDisplayMode);

        var YesButton = document.getElementById(UserInterfaceLayer.GetDomIds().messageBoxYesButton);
        if (YesButton) YesButton.addEventListener("click", reloadPage);

        var NoButton = document.getElementById(UserInterfaceLayer.GetDomIds().messageBoxNoButton);
        if (NoButton) NoButton.addEventListener("click", hideMessage);
    }

    function addToComponents(parentid, parentElement) {
        for (var x = 0; x < parentElement.children.length; x++) {
            var childElement = parentElement.children[x];
            if (childElement.id) {
                var name = childElement.id;
                childElement.id = parentid + "__" + childElement.id;
                DataLayer.addToComponents(parentid, childElement.id, name);
            }
            addToComponents(parentid, childElement);
        }
    }

    function switchToDesignMode() {
        //Open editable text elements for content change 
        var textElements = UserInterfaceLayer.GetEditableElements().Text;
        UserInterfaceLayer.OpenEditableElements(textElements, contentChange);

        //Open editable image elements for image change and add preview, getLastImage functionality 
        if (FileOperations.Check) {
            var imageElements = UserInterfaceLayer.GetEditableElements().Images;
            UserInterfaceLayer.DisplayMenuOnImages(imageElements);

            for (var i = 0; i < imageElements.length; i++) {
                var parentid = imageElements[i].id;
                var childname;

                for (j = 0; j < imageElements[i].children.length; j++) {
                    if (imageElements[i].children[j].id == "edit_button_template") {
                        addToComponents(parentid, imageElements[i].children[j]);
                        break;
                    }
                }

                childname = UserInterfaceLayer.GetDomIds().edit_button_template__file;
                var inputid = DataLayer.getComponentElementId(parentid, childname);
                var input = document.getElementById(inputid);
                input.addEventListener("change", previewImage);
                input.style.cursor = "pointer";

                childname = UserInterfaceLayer.GetDomIds().edit_button_template__link_undo;
                var buttonUndoId = DataLayer.getComponentElementId(parentid, childname);
                var buttonUndo = document.getElementById(buttonUndoId);
                buttonUndo.addEventListener("click", getLastImage);
                buttonUndo.style.cursor = "pointer";
            }
        }

        //Switch menu from display to design
        UserInterfaceLayer.SwitchMenu();
    }

    function previewImage(e) {
        var input = e.target;
        var preview = document.getElementById(DataLayer.getParentElementId(input.id));

        var name = input.files[0].name;
        var dotIndex = name.indexOf('.');
        var extension = "." + name.substr(dotIndex + 1, name.length - dotIndex);
        var pageid = UserInterfaceLayer.GetPageId();
        var updatedFileName = "img-" + pageid + "-" + preview.id + "-" + Date.now() + extension;
        
        DataLayer.writeUpdatedImage(pageid, preview.id, preview.style.backgroundImage, updatedFileName, input.files[0]);

        FileOperations.ReadFileAsDataURL(input, function (reader) { // reader => FileReader
            preview.style.backgroundImage = "url('" + reader.target.result + "')";
        });

        var buttonUndo = document.getElementById(DataLayer.getComponentElementId(preview.id, UserInterfaceLayer.GetDomIds().edit_button_template__link_undo));
        buttonUndo.style["display"] = "inline-block";
    }

    function switchToDisplayMode() {
        UserInterfaceLayer.SwitchMessageBox();
    }

    function reloadPage() {
        location.reload();
    }

    function hideMessage() {
        UserInterfaceLayer.SwitchMessageBox();
    }

    var contentChange = function (e) {
        DataLayer.writeUpdatedContent(UserInterfaceLayer.GetPageId(), e.currentTarget.id, e.currentTarget.innerText);
    }

    var save = function () {
        var dataJSON = DataLayer.getDataJSON();
        var xhttp = new XMLHttpRequest();

        //Send Text Content & Image File Names
        xhttp.onreadystatechange = function () {
            document.getElementById(UserInterfaceLayer.GetDomIds().message).innerHTML = this.status + "-" + this.responseText;
        };
        xhttp.open("POST", "../CMS/UpdateData", true);
        xhttp.setRequestHeader("Content-type", "application/json");
        xhttp.send(dataJSON);

        //Send Image Files
        xhttp = new XMLHttpRequest();
        var filedata = new FormData();

        var images = DataLayer.getImageFiles();
        for (var i = 0; i < images.length; i++) {
            var image = images[i];
            if (image.file) {
                filedata.append(image.updatedFileName, image.file);
            } 
        }

        xhttp.onreadystatechange = function () {
            document.getElementById(UserInterfaceLayer.GetDomIds().message).innerHTML = this.status + "-" + this.responseText;
        };
        xhttp.open("POST", "../CMS/UpdateImages", true);
        xhttp.send(filedata);

        DataLayer.Clear();
    }

    var getLastImage = function (e) {
        if (!e.currentTarget.id) return;

        var imageElement = document.getElementById(DataLayer.getParentElementId(e.currentTarget.id));
        imageElement.style.backgroundImage = DataLayer.getOriginalURL(imageElement.id);
        DataLayer.writeUpdatedImage(UserInterfaceLayer.GetPageId(), imageElement.id, "", "");

        var buttonUndo = document.getElementById(DataLayer.getComponentElementId(imageElement.id, UserInterfaceLayer.GetDomIds().edit_button_template__link_undo));
        buttonUndo.style["display"] = "none";

        //Getting the image URL from server --> Replaced with client solution :
        //-----------------------------------------------------------------------
        ////e.target.parentElement.style["display"] = "none";

        //var xhttp = new XMLHttpRequest();
        //xhttp.onreadystatechange = function () {
        //    if (this.readyState == 4 && this.status == 200) {
        //        document.getElementById(UserInterfaceLayer.GetDomIds().message).innerHTML = this.status + "-" + this.responseText;
        //        if (this.responseText != "") {
        //            imageElement.style.backgroundImage = "url('" + this.responseText + "')";
        //        }
        //        else {
        //            imageElement.style.backgroundImage = "";
        //        }
                    
        //    }
        //    //if (this.readyState==4 && this.status == 200) {
        //    //    var doc = new DOMParser().parseFromString(this.responseText, "text/html");
        //    //    imageElement.innerHTML = doc.getElementById(imageElement.id).innerHTML;
        //    //}
        //};
        //xhttp.open("POST", "CMS/GetLastImage?pageid=" + UserInterfaceLayer.GetPageId() + "&id=" + imageElement.id, true);
        ////xhttp.open("POST", "../Home/Index", true);
        ////xhttp.setRequestHeader("Content-type", "text/html");
        //xhttp.send();
    }

    return {
        init: function () {

            var designtoolbar = document.getElementsByClassName("designtoolbar");

            if (designtoolbar.length > 0) {
                var textElements = UserInterfaceLayer.GetEditableElements().Text;
                var imageElements = UserInterfaceLayer.GetEditableElements().Images;

                if(textElements.length == 0 && imageElements.length == 0)
                    designtoolbar[0].style.display = "none";
            }
            
            setUpEventHandlers();
    }
};

})();

var UserInterfaceLayer = (function () {

    var classNames = {
        Text: {
            SpecifierClass: "g-text",
            EditableClass: "g-edit"
        },
        Images: {
            SpecifierClass: "g-image",
            EditableClass: "g-edit"
        },
        MenuLink: "designtoolbar__list__item__link",
        Hide: "u-hide"
    };

    var DomIds = {
        pageId: "pageid",
        editButton: "designtoolbar__list__button--edit",
        saveButton: "designtoolbar__list__button--save",
        backButton: "designtoolbar__list__button--back",
        message: "designtoolbar__message",
        messageBox: "designtoolbar__message-box",
        messageBoxYesButton: "designtoolbar__message-box--yes",
        messageBoxNoButton: "designtoolbar__message-box--no",
        edit_button_template: "edit_button_template",
        edit_button_template__file: "edit_button_template__file",
        edit_button_template__link_undo: "edit_button_template__link--undo"
    };

    var PageId = document.getElementById(DomIds.pageId).innerText;

    return {
        GetEditableElements: function () {
            var EditableElements = {
                Text: [],
                Images: []
            };

            EditableElements.Text = document.getElementsByClassName(classNames.Text.SpecifierClass);
            EditableElements.Images = document.getElementsByClassName(classNames.Images.SpecifierClass);

            return EditableElements;
        },

        //Elements with "g-text" class name
        OpenEditableElements: function (Elements, fnContentChange) {
            for (var i = 0; i < Elements.length; i++) {
                Elements[i].contentEditable = true;
                Elements[i].addEventListener("keyup", fnContentChange);
                Elements[i].addEventListener("mouseup", fnContentChange);
                Elements[i].classList.toggle(classNames.Text.EditableClass);
            }
        },

        //Elements with "g-image" class name
        DisplayMenuOnImages: function (Elements) {
            var edit_button = document.getElementById(DomIds.edit_button_template);

            for (var i = 0; i < Elements.length; i++) {
                var edit_button_new = edit_button.cloneNode(true);
                edit_button_new.classList.toggle(classNames.Hide);
                Elements[i].appendChild(edit_button_new);
            }
        },

        SwitchMenu: function () {
            var MenuLinks = document.getElementsByClassName(classNames.MenuLink);
            for (var i = 0; i < MenuLinks.length; i++) {
                MenuLinks[i].classList.toggle(classNames.Hide);
            }
        },

        SwitchMessageBox: function () {
            var MessageBox = document.getElementById(DomIds.messageBox);
            MessageBox.classList.toggle(classNames.Hide);
        },

        GetDomIds: function () {
            return DomIds;
        },

        GetPageId: function () {
            return PageId;
        }
    };
})();

ControllerLayer.init();








