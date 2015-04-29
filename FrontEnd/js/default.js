$(document).ready(function () {
    $('#changeColor').change(function () {
        _canvas.color($(this).val());
    });

    $('.sizePen').click(function () {
        var newSize = parseInt($(this).children('b').text());
        $('#currentSize').text(newSize)
        _canvas.lineWidth(newSize);
    });

    $('#clearScreen').click(function () {
        _canvas.clearScreen();
    });

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
});