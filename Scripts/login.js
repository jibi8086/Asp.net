window.onload = function () {
    var text = document.getElementById("error");
    if (text != "0") {
        error.show();
    }
    else {
        error.hide();
        error.style.visibility = 'hidden';
    }
}



$('.message a').click(function () {
    $('form').animate({ height: "toggle", opacity: "toggle" }, "slow");
});