function Chat(input, button, chatContainer) {
    var _self = this;
    var inputElement = $(input);
    var chatContainer = $(chatContainer);
    var wsFunction = undefined;

    $(button).click(function () {
        send();
    });

    $(input).keyup(function (e) {
        if (e.keyCode == 13) {
            send();
            e.preventDefault();
        }
    });


    function send() {
        var msg = inputElement.val();
        inputElement.val("");
        addMessage(user, msg);
        wsFunction("chat", { User: user, Message: msg });
    }

    function addMessage(user, msg) {
        var container = document.createElement("div");
        var inp = document.createElement("div");
        var lab = document.createElement("label");
        var span = document.createElement("span");


        $(lab).text(user + getTime() + ':');
        $(span).text(msg);


        $(container).addClass("form-inline");
        $(inp).addClass("input-group");


        $(inp).append(lab);
        $(inp).append(span);
        $(container).append(inp);
        chatContainer.append(container);
        chatContainer.scrollTop(chatContainer[0].scrollHeight);
    }


    var getTime = function () {
        var date = new Date();

        var hour = date.getHours();
        hour = (hour < 10 ? '0' : '') + hour;
        var min = date.getMinutes();
        min = (min < 10 ? '0' : '') + min;

        return '(' + hour + ':' + min + ')';
    }

    this.message = function (obj) {
        if (obj.User === undefined || obj.Message === undefined)
            return;
        addMessage(obj.User, obj.Message);
    }

    this.WSFunction = function (e) {
        wsFunction = e;
    };
}