$("#authBtn").click(function () {
    location.href = 'Home/Auth';
});
if (document.cookie.indexOf('spotauthtoke')!==-1) {
    $("#initLink").removeAttr("hidden");
}

$("#showPartial").click(function () {
    $("#SpotParams").removeAttr("hidden");
    $.ajax({
        url: '/Home/SpotParams',
        success: function (data) {
            $("#SpotParams").html(data);
        }
    })
});

function PostPlaylist() {

    $.validator.unobtrusive.parse($("#spotParams"));
    $("#spotParams").validate();
    if ($("#spotParams").valid()) {

        $.ajax({
            type: "POST",
            url: "/Home/SpotParams",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify($("#spotParams").data),
            success: function (data) {
                $("#PlaylistResponse").html(data);
            }

        });

    }
}

//ajax call for getting playlists

//ajax call for getting tracks