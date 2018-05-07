/// <reference path="oidc-client.js" />

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);

var config = {
    authority: getStsPublicAddress(),
    client_id: "js",
    redirect_uri: getJsPublicAddress() + "/callback.html",
    response_type: "id_token token",
    scope:"openid profile api1",
    post_logout_redirect_uri: getJsPublicAddress() +"/index.html",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function getStsPublicAddress(){
       return "http://localhost:50405"
}

function getJsPublicAddress(){
        return "http://localhost:5800"
}

function getApiPublicAddress(){
    return "http://localhost:5901"
}

function login() {
    log("start logging");
    mgr.signinRedirect();
}

function api() {
    log("start call to api");
    mgr.getUser().then(function (user) {
        var url = getApiPublicAddress() +"/api/identity";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}