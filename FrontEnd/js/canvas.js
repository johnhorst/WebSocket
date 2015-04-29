function Canvas(id) {
    var _self = this;   
    var canvas = document.getElementById(id);
    var ctx = canvas.getContext("2d");
    var drawing = false;
    var wsFunction = undefined;
    var color = '#000000';
    var lineWidth = 5;

    $(canvas).mousedown(function (e) {
        ctx.lineWidth = lineWidth;
        ctx.strokeStyle = color;
        drawing = true;
        var obj = getPosition(e);
        obj.Type = "start";
        obj.Color = color;
        obj.LineWidth = lineWidth;
        wsFunction("draw", obj);
        draw(obj);
    });

    $(canvas).mouseup(function (e) {
        drawing = false;
    });

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
            return { X: e.originalEvent.layerX, Y: e.originalEvent.layerY };
        return { X: e.offsetX, Y: e.offsetY };
    };

    var _clearScreen = function () {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
    }

    var draw = function (obj) {
        switch (obj.Type.toLowerCase()) {
            case "start":
                ctx.beginPath();
                ctx.moveTo(obj.X, obj.Y);
                ctx.strokeStyle = obj.Color;
                ctx.lineWidth = obj.LineWidth;
                break;
            case "move":
                ctx.lineTo(obj.X, obj.Y);
                ctx.stroke();
                break;            
            case "clearscreen":
                console.log(obj);
                _clearScreen();
                break;
            default:
                break;
        }
    };

    this.draw = function (obj) {
        draw(obj);
    };
    this.WSFunction = function (e) {
        wsFunction = e;
    };

    this.color = function (c) {
        color = c;
    };

    this.lineWidth = function (size) {
        lineWidth = size;
    };

    this.clearScreen = function () {
        wsFunction("draw", { Type: "clearscreen" });
        _clearScreen();
    }
}