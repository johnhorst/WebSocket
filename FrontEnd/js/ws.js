$(document).ready(function () {

    $('#user').click(function () {
        var name = prompt('Please enter your name', user);
        if (name != null && name.length > 0) {
            user = name;
        }
    });

    $('#connect').click(function () {
        _ws.connect();
    });

    var _canvas = new Canvas('canvas');
    var _ws = new WS("#imgstatus", "#connect");
    var _chat = new Chat('#message', '#send', '#chatContainer');

    _canvas.WSFunction(_ws.send);
    _chat.WSFunction(_ws.send);

    _ws.chatFunction(_chat.message);
    _ws.drawFunction(_canvas.draw);
    _ws.connect();

    

    function WS(status, btn) {
        var connected = false;
        var _ws = undefined;
        var _chatFunction = undefined;
        var _drawFunction = undefined;

        window.onbeforeunload = function () {
            if (_ws !== undefined)
                _ws.close();
        }

        this.send = function (type, message) {
            if (connected) {
                var obj = JSON.stringify({ Type: type, Message: message });
                _ws.send(obj);
            }
        }
        this.chatFunction = function (e) {
            _chatFunction = e;
        };
        this.drawFunction = function (e) {
            _drawFunction = e;
        };

        this.connect = function () {
            _ws = new WebSocket(SERVER_IP);

            _ws.onclose = function () {
                connected = false;
                console.log("Connection close.");
                $(status).attr("src", 'img/offline.png');
                $(btn).css('visibility', 'visible');
            }

            _ws.onopen = function () {
                console.log("Connected.");
                connected = true;
                $(status).attr("src", 'img/online.png');
                $(btn).css('visibility', 'hidden');
            }

            _ws.onerror = function (e) {
                console.log("Error:" + e);
            }

            _ws.onmessage = function (e) {
                var data = JSON.parse(e.data);
                switch (data.Type.toLowerCase()) {
                    case "draw":
                        _drawFunction(data.Message);
                        break;
                    case "chat":
                        _chatFunction(data.Message);
                        break;
                    default:
                        break;
                }
            }
        };
    };

    function Chat(input, button, chatContainer) {

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

    function Canvas(id) {

        var canvas = document.getElementById(id);
        var ctx = canvas.getContext("2d");
        var drawing = false;
        var wsFunction = undefined;

        $(window).mousedown(function (e) {
            drawing = true;
            var obj = getPosition(e);
            obj.Type = "start";
            wsFunction("draw", obj);
            draw(obj);
        });

        $(window).mouseup(function (e) {
            drawing = false;
        })

        $(canvas).mousemove(function (e) {
            if (drawing) {
                var obj = getPosition(e);
                obj.Type = "move";
                wsFunction("draw", obj);
                draw(obj);
            }
        });

        var getPosition = function (e) {
            if (e.offsetX === undefined || e.offsetY === undefined)
                return { X: e.pageX - canvas.offsetLeft, Y: e.pageY - canvas.offsetTop }
            return { X: e.offsetX, Y: e.offsetY };
        }

        var draw = function (obj) {
            switch (obj.Type.toLowerCase()) {
                case "start":
                    ctx.beginPath();
                    ctx.moveTo(obj.X, obj.Y);
                    break;
                case "move":
                    ctx.lineTo(obj.X, obj.Y);
                    ctx.stroke();
                    break;
                default:
                    break;
            }
        }

        this.draw = function (obj) {
            draw(obj);
        }
        this.WSFunction = function (e) {
            wsFunction = e;
        };
    }
});