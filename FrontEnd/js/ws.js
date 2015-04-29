function WS(status, btn) {
        var _self = this;
        var connected = false;
        var _ws = undefined;
        var _chatFunction = undefined;
        var _drawFunction = undefined;
        var _status = status || undefined;
        var _btn = btn || undefined;

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
                if (status)
                    $(status).attr("src", 'img/offline.png');
                if (btn)
                    $(btn).css('visibility', 'visible');
            }

            _ws.onopen = function () {
                console.log("Connected.");
                connected = true;
                if (status)
                    $(status).attr("src", 'img/online.png');
                if (btn)
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

