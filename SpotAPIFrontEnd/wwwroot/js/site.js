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
    });
});

function PostPlaylist() {

    var applecakes = $.validator.unobtrusive.parse($("#spotParams"));
    $("#spotParams").validate();
    if ($("#spotParams").valid()) {
        var submitData = $("#spotParams")['0'];
        var spotOb = new Object();
        spotOb.Id = submitData.Id.value;
        spotOb.name = submitData.Name.value;
        spotOb.length = parseInt(submitData.Length.value);
        if (spotOb.Id === "") {
            var genreList = [];

            for (var i = 0; i < submitData.Genres.selectedOptions.length; i++) {
                genreList.push(submitData.Genres.selectedOptions[i].value);
            }
            spotOb.genres = genreList;
            spotOb.artist = submitData.Artist.value;
            spotOb.tempo = submitData.Tempo.value;
            spotOb.dance = submitData.Dance.value;
            spotOb.energy = submitData.Energy.value;
            spotOb.valence = submitData.Valence.value;
        }
        $.ajax({
            type: "POST",
            url: "Home/SpotParams",
            contentType : 'application/json',
            data: JSON.stringify(spotOb),
            success: function (data) {
                $("#SpotParams").hide();
                $("#ViewPlaylists").show();
                $("#ViewPlaylists").html(data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#ViewPlaylists").html("Oops, something went wrong, please check to see if the playlist was made: " + XMLHttpRequest.responseText);
            }

        });
    }
}


//ajax call for getting playlists

$("#getPlaylists").click(function () {

    $("#SpotParams").hide();
    $("#ViewTracks").hide();
    $("#ViewPlaylists").show();
    $("#ViewPlaylists").html("This may take a while, be patient");
    $.ajax({
        url: '/Home/GetPlaylists',
        success: function (data) {
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewPlaylists").html("Oops, something went wrong, please try again: "+XMLHttpRequest.responseText);
        }
    });
});

//ajax call for getting tracks
function getTracks(id)
{
    $("#SpotParams").hide();
    $("#ViewPlaylists").hide();
    $("#ViewTracks").show();
    $.ajax({
        url: '/Home/GetTracks/' + id,
        success: function(data) {
            $("#ViewTracks").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewTracks").html("Oops, something went wrong, please try again: " + XMLHttpRequest.responseText);
        }
    });
}
//ajax call to make liked songs playlist
$("#getSavedTracks").click(function () {
    $("#SpotParams").hide();
    $("#ViewTracks").hide();
    $("#ViewPlaylists").show();
    $("#ViewPlaylists").html("This may take a while, be patient");
    var spotOb = new Object();
    spotOb.SavedTracks = true;
    $.ajax({
        type: "POST",
        url: "Home/SpotParams",
        contentType: 'application/json',
        data: JSON.stringify(spotOb),
        success: function (data) {
            $("#SpotParams").hide();
            $("#ViewPlaylists").show();
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewPlaylists").html("Oops, something went wrong, please check to see if the playlist was made: " + XMLHttpRequest.responseText);
        }
    });
});

function newPlaylist(id) {
    $("#SpotParams").show();
    $("#ViewPlaylists").hide();
    $("#ViewTracks").hide();
    $.ajax({
        url: '/Home/SpotParams/' + id,
        success: function (data) {
            $("#SpotParams").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#SpotParams").html("Oops, something went wrong, please try again: " + XMLHttpRequest.responseText);
        }
    });
}

function deletePlaylist(id) {    
    $.ajax({
        url: '/Home/DeletePlaylist/' + id,
        success: function (data) {
            $("#ViewPlaylists").show();
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewPlaylists").html("Oops, something went wrong, please try again: " + XMLHttpRequest.responseText);
        }
    });
}