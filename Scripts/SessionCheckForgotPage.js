function handleBackFunctionality() {
    if (window.event) {
        if (window.event.clientX < 40 && window.event.clientY < 0) {
            alert("Clicked - Browser back button.");
        }
        else {
            alert("Clicked - Browser refresh button.");
        }
    }
    else {
        if (event.currentTarget.performance.navigation.type == 2) {
            alert("Clicked - Browser refresh button.");
        }
        else if (event.currentTarget.performance.navigation.type == 1) {
            alert("Clicked - Browser back button.");
        }
    }
}

window.onbeforeunload = handleBackFunctionality;
$(document).unload(handleBackFunctionality);

function myFunction() {
    if ('@Html.ValidationMessageFor(m => m.Email)' != null) {
        Materialize.toast('@Html.ValidationMessageFor(m => m.Email)', 3000, 'rounded');

    }
    if ('@ViewBag.message' != null) {
        Materialize.toast('@ViewBag.message', 3000, 'rounded');

    }

}
