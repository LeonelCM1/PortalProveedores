
String.prototype.format = function() {
    var txt = this;
    for (var i = 0; i < arguments.length; i++) {
        var exp = new RegExp('\\{' + (i) + '\\}', 'gm');
        txt = txt.replace(exp, arguments[i]);
    }
    return txt;
}