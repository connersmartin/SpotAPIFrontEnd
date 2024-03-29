﻿$("#authBtn").click(function () {
    location.href = 'Home/Auth';
});
if (document.cookie.indexOf('spotauthtoke')!==-1) {
    $("#initLink").removeAttr("hidden");
    $("#authBtn").text("Authorized");
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

var errorText = "Oops, something went wrong, please click 'Authorized' and try again. If the problem persists please contact me";

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
        $("#spotParams").html("This may take a while, be patient. We are petitioning Spotify's API to create this playlist.");

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
                $("#ViewPlaylists").html("Oops, something went wrong, please check to see if the playlist was made");
            }

        });
    }
}


//ajax call for getting playlists

$("#getPlaylists").click(() => {
    $("#SpotParams").hide();
    $("#ViewTracks").hide();
    $("#ViewPlaylists").html("This may take a while, be patient. We are gathering all of your playlists and their associated tracks.");
    $("#ViewPlaylists").show();
    $.ajax({
        url: '/Home/GetPlaylists',
        success: function (data) {
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewPlaylists").html(errorText);
        }
    })
});

$("#updatePlaylist").click(function () {
    $("#SpotParams").hide();
    $("#ViewTracks").hide();
    $("#ViewPlaylists").html("This may take a while, be patient. We are gathering all of your playlists and their associated tracks.");
    $("#ViewPlaylists").show();
    $.ajax({
        url: '/Home/GetPlaylists?playListOnly=true',
        success: function (data) {
            $("#ViewPlaylists").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewPlaylists").html(errorText);
        }
    });
});

//ajax call for getting tracks
function getTracks(id)
{
    $("#SpotParams").hide();
    $("#ViewPlaylists").hide();
    $("#ViewTracks").html("This may take a while, be patient. We are gathering all of the tracks associated with this playlist.");
    $("#ViewTracks").show();
    $.ajax({
        url: '/Home/GetTracks/' + id,
        success: function(data) {
            $("#ViewTracks").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewTracks").html(errorText);
        }
    });
}
//ajax call to make liked songs playlist
$("#getSavedTracks").click(function () {
    if (window.confirm('Click ok to create a new playlist based off your liked songs')) {
        $('#initLink').hide();
        $("#SpotParams").hide();
        $("#ViewTracks").hide();
        $("#ViewPlaylists").html("This may take a while, be patient. We are petitioning Spotify's API to convert all your saved tracks into an actual playlist.");
        $("#ViewPlaylists").show();
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
                $("#ViewPlaylists").html("Oops, something went wrong, please check to see if the playlist was made");
            }
        });
    }
});

function newPlaylist(id) {
    $("#ViewPlaylists").hide();
    $("#ViewTracks").hide();
    $("#SpotParams").show();
    $("#SpotParams").html("This may take a while, be patient. We are petitioning Spotify's API to create this playlist.");

    $.ajax({
        url: '/Home/SpotParams/' + id,
        success: function (data) {
            $("#SpotParams").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#SpotParams").html(errorText);
        }
    });
}

function updatePlaylist(id) {
    $("#ViewPlaylists").hide();
    $("#SpotParams").hide();
    $("#ViewTracks").html("This may take a while, be patient. We are petitioning Spotify's API.");
    $("#ViewTracks").show();
    $.ajax({
        url: '/Home/UpdatePlaylist/' + id,
        success: function (data) {
            $("#ViewTracks").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#ViewTracks").html(errorText);
        }
    });
}


function copyPlaylist(id) {
    if (window.confirm('Do you really want to copy this playlist?')) {
        $("#ViewPlaylists").hide();
        $("#SpotParams").hide();
        $("#ViewTracks").show();
        $("#ViewTracks").html("This may take a while, be patient. We are petitioning Spotify's API.");
        $("#ViewPlaylists").html("This may take a while, be patient. We are petitioning Spotify's API.");
        $.ajax({
            url: '/Home/CopyPlaylist/' + id,
            success: function (data) {
                $("#ViewTracks").html(data);
                $("#ViewPlaylists").html(data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#ViewTracks").html(errorText);
                $("#ViewPlaylists").html(errorText);
            }
        });
    }
}

function deletePlaylist(id) {
    if (window.confirm('Do you really want to delete/unfollow this playlist?')) {
        $("#ViewPlaylists").html("Deleting playlist, you will see your playlists when this is completed");
        $("#ViewTracks").html("Deleting playlist, you will see your playlists when this is completed");
        $.ajax({
            url: '/Home/DeletePlaylist/' + id,
            success: () => {
                $("#SpotParams").hide();
                $("#ViewTracks").hide();
                $("#ViewPlaylists").html("This may take a while, be patient. We are gathering all of your playlists and their associated tracks.");
                $("#ViewPlaylists").show();
                $.ajax({
                    url: '/Home/GetPlaylists',
                    success: function (data) {
                        $("#ViewPlaylists").html(data);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $("#ViewPlaylists").html(errorText);
                    }
                })
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#ViewPlaylists").html(errorText);
                $("#ViewTracks").html(errorText);
            }
        });
    }
}