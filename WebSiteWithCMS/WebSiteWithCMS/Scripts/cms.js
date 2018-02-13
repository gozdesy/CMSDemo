var DataLayer = (function () {
    var Data = {
        Elements: []
    };

    var elementData = function (pageid, id, content) {
        this.pageid = pageid,
        this.id = id,
        this.content = content
    };

    return {
        writeUpdatedContent : function (pageid, id, content) {
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
            console.log(Data.Elements);
            console.log(JSON.stringify(Data));
        },

        getDataJSON: function () {
            return JSON.stringify(Data);
        }
    };
})();



var ControllerLayer = (function () {

    function SetUpEventHandlers() {
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

    function switchToDesignMode() {
        var textElements = UserInterfaceLayer.GetEditableElements().Text;
        UserInterfaceLayer.OpenEditableElements(textElements, contentChange);
        UserInterfaceLayer.SwitchMenu();
    }

    function switchToDisplayMode() {
        UserInterfaceLayer.SwitchMessageBox();

        //Yes
        //UserInterfaceLayer.SwitchMenu();
        //location.reload();

        //No
        //UserInterfaceLayer.SwitchMessageBox();

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

        xhttp.onreadystatechange = function () {
            //if (this.readyState == 4 && this.status == 200) {
                document.getElementById(UserInterfaceLayer.GetDomIds().message).innerHTML = this.status + "-" + this.responseText;
            //}
        };
        xhttp.open("POST", "../CMS/UpdateData", true);
        xhttp.setRequestHeader("Content-type", "application/json");
        xhttp.send(dataJSON); 
    }

    return {
        init: function () {
            SetUpEventHandlers();
        }
    };

})();

var UserInterfaceLayer = (function () {

    var classNames = {
        Text: {
            SpecifierClass: "g-text",
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
        messageBoxNoButton: "designtoolbar__message-box--no"
    };

    var PageId = document.getElementById(DomIds.pageId).innerText;

    return {
        GetEditableElements: function () {
            var EditableElements = {
                Text: []
            };

            EditableElements.Text = document.getElementsByClassName(classNames.Text.SpecifierClass);

            return EditableElements;
        },

        OpenEditableElements: function (Elements, fnContentChange) {
            for (var i = 0; i < Elements.length; i++) {
                Elements[i].contentEditable = true;
                Elements[i].addEventListener("keyup", fnContentChange);
                Elements[i].addEventListener("mouseup", fnContentChange);
                Elements[i].classList.toggle(classNames.Text.EditableClass);
            }
        },

        SwitchMenu: function () {
            var MenuLinks = document.getElementsByClassName(classNames.MenuLink);
            for (var i = 0; i < MenuLinks.length; i++) {
                MenuLinks[i].classList.toggle(classNames.Hide);
            }
        },

        SwitchMessageBox: function() {
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







