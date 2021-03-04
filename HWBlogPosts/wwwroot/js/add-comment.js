$(() => {
    $("#name").on('keyup', function () {
        EnableButton();
    })

    $("#content").on('keyup', function () {
        EnableButton();
    })

    function EnableButton() {
        const name = $("#name").val();
        const text = $("#content").val();
        const isValid = name && text;
        $("#submit").prop('disabled', !isValid);
    }
})
