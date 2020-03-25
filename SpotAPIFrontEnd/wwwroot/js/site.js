$("#authBtn").click(function () {
    location.href = 'Home/Auth';
});
if (document.cookie.indexOf('spotauthtoke')!==-1) {
    $("#initLink").removeAttr("hidden");
}

$("#showPartial").click(function () {
    $("#ViewPlaylists").hide();
    $("#ViewTracks").hide();
    $("#SpotParams").show();
    $.ajax({
        url: '/Home/SpotParams',
        success: function (data) {
            $("#SpotParams").html(data);
        }
    });
});

var errorText = "Oops, something went wrong, please click 'Do it' and try again: ";

function PostPlaylist() {
    $('#initLink').hide();
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
                $('#initLink').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('#initLink').show();
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
    $("#ViewPlaylists").html("This may take a while, be patient. We are gathering all of your playlists and their associated tracks.");
    $.ajax({
        url: '/Home/GetPlaylists',
        success: function (data) {            
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewPlaylists").html(errorText+XMLHttpRequest.responseText);
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
            $("#ViewTracks").html(errorText + XMLHttpRequest.responseText);
        }
    });
}
//ajax call to make liked songs playlist
$("#getSavedTracks").click(function () {
    $('#initLink').hide();
    $("#SpotParams").hide();
    $("#ViewTracks").hide();
    $("#ViewPlaylists").show();
    $("#ViewPlaylists").html("This may take a while, be patient. We are petitioning Spotify's API to convert all your saved tracks into an actual playlist.");
    var spotOb = new Object();
    spotOb.SavedTracks = true;
    $.ajax({
        type: "POST",
        url: "Home/SpotParams",
        contentType: 'application/json',
        data: JSON.stringify(spotOb),
        success: function (data) {
            $('#initLink').show();
            $("#SpotParams").hide();
            $("#ViewPlaylists").show();
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $('#initLink').show();
            $("#ViewPlaylists").html("Oops, something went wrong, please check to see if the playlist was made: " + XMLHttpRequest.responseText);
        }
    });
});

function newPlaylist(id) {
    $("#ViewPlaylists").hide();
    $("#ViewTracks").hide();
    $("#SpotParams").show();
    $.ajax({
        url: '/Home/SpotParams/' + id,
        success: function (data) {
            $("#SpotParams").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#SpotParams").html(errorText + XMLHttpRequest.responseText);
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
            $("#ViewPlaylists").html(errorText + XMLHttpRequest.responseText);
        }
    });
}