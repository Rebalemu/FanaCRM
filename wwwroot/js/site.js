// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition, showError);
    } else {
        alert("Geolocation is not supported by this browser.");
    }
}

function showPosition(position) {
    let lat = position.coords.latitude;
    let lon = position.coords.longitude;

    // Save coordinates
    document.getElementById("latitude").value = lat;
    document.getElementById("longitude").value = lon;

    // Show readable text
    document.getElementById("locationInput").value =
        "Lat: " + lat + ", Lng: " + lon;

    // Update map
    document.getElementById("mapFrame").src =
        `https://maps.google.com/maps?q=${lat},${lon}&z=15&output=embed`;
}

function showError(error) {
    alert("Unable to retrieve your location.");
}
document.addEventListener("DOMContentLoaded", function () {
    var deleteModal = document.getElementById('deleteModal');

    if (!deleteModal) return;

    deleteModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        var id = button.getAttribute('data-id');

        document.getElementById('deleteId').value = id;
    });
});