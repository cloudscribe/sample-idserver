var configDev = {
    sts: 'http://localhost:50405/',
    authority: 'http://localhost:50405/',
    client_id: 'vuejs',
    redirect_uri: 'http://localhost:5900',
    post_logout_redirect_uri: 'http://localhost:5900',
    response_type: 'id_token token',
    scope: 'openid profile api1',
    filterProtocolClaims: true,
    loadUserInfo: true,
    apiUrl:"http://localhost:5901/api/identity"
}


export default configDev

function GetRootUrl() {
    return window.location.host.substring(window.location.host.lastIndexOf('.', window.location.host.lastIndexOf('.') - 1) + 1)
}


