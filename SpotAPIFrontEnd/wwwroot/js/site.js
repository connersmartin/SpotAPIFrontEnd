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

    var applecakes = $.validator.unobtrusive.parse($("#spotParams"));
    $("#spotParams").validate();
    if ($("#spotParams").valid()) {
        var submitData = $("#spotParams")['0'];
        var genreList = []
        for (var i = 0; i < submitData.Genres.selectedOptions.length; i++) {
            genreList.push(submitData.Genres.selectedOptions[i].value);
        }
        var spotOb = new Object();
        spotOb.name = submitData.Name.value;
        spotOb.length = parseInt(submitData.Length.value);
        spotOb.genres = genreList;
        spotOb.artist = submitData.Artist.value;
        spotOb.tempo =  submitData.Tempo.value;
        spotOb.dance = submitData.Dance.value;
        spotOb.energy = submitData.Energy.value;
        spotOb.instrumental = submitData.Instrumental.value;

        $.ajax({
            type: "POST",
            url: "Home/SpotParams",
            contentType : 'application/json',
            dataType: 'json',
            data: JSON.stringify(spotOb),
            success: function (data) {
                $("#SpotParams").attr("hidden");

                $("#PlaylistResponse").html(data);
            }

        });
    }
}


//ajax call for getting playlists

//ajax call for getting tracks