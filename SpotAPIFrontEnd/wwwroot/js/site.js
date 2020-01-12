$("#authBtn").click(function () {
    location.href = 'Home/Auth';
});
if (document.cookie.indexOf('spotauthtoke')!==-1) {
    $("#initLink").removeAttr("hidden");
}

$("#showPartial").click(function () {
    $("#ViewPlaylists").hide();
    $("#SpotParams").show();
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
            data: JSON.stringify(spotOb),
            success: function (data) {
                $("#SpotParams").hide();
                $("#ViewPlaylists").show();
                $("#ViewPlaylists").html(data);
            }

        });
    }
}


//ajax call for getting playlists

$("#getPlaylists").click(function () {
    $("#ViewTracks").hide();
    $("#ViewPlaylists").show();
    $.ajax({
        url: '/Home/GetPlaylists',
        success: function (data) {
            $("#ViewPlaylists").html(data);
        }
    })
});

//ajax call for getting tracks
function getTracks(id)
{
    $("#ViewPlaylists").hide();
    $("#ViewTracks").show();
    $.ajax({
        url: '/Home/GetTracks/'+id,
        success: function (data) {
            $("#ViewTracks").html(data);
        }
    })
}