function success(pos) {
    var crd = pos.coords;
    console.log("Your current position is:");
    console.log("Latitude : "+crd.latitude);
    console.log("Longitude: "+crd.longitude);
    console.log("More or less "+crd.accuracy+" meters.");
    document.cookie = "latitude=" + crd.latitude;
    document.cookie = "longitude=" + crd.longitude;

}
function error(err) {
    console.warn("ERROR " + err.code + " : " + err.message);
    document.cookie = "latitude=" + 52;
    document.cookie = "longitude=" + -2;
}

navigator.geolocation.getCurrentPosition(success, error);