Oidc.Log.logger = console;
Oidc.Log.level = Oidc.Log.DEBUG;
console.log("Using oidc-client version: ", Oidc.Version);
var url = window.location.origin;
var eventValidUser = new Event('validuser');


var oidcsettings = {
    authority: 'http://localhost:4000',
    client_id: 'api-pika-gd',
    redirect_uri: url + '/callback.html',
    post_logout_redirect_uri: url + '/logout.html',
    response_type: 'code',
    scope: 'openid profile pika-gd',

    //popup_redirect_uri: url + '/code-identityserver-sample-popup-signin.html',
    //popup_post_logout_redirect_uri: url + '/code-identityserver-sample-popup-signout.html',

    silent_redirect_uri: url + '/silent.html',
    automaticSilentRenew: true,
    validateSubOnSilentRenew: true,
    silentRequestTimeout: 10000,

    monitorAnonymousSession: true,

    filterProtocolClaims: true,
    loadUserInfo: true,
    revokeAccessTokenOnSignout: true,
    jwtstoragename: 'cjwt',
    uidstoragename: 'cuid'
};




var mgr = new Oidc.UserManager(oidcsettings);


///////////////////////////////
// events
///////////////////////////////
mgr.events.addAccessTokenExpiring(function () {
    console.log("token expiring");
 
    // maybe do this code manually if automaticSilentRenew doesn't work for you
    mgr.signinSilent().then(function (user) {
        console.log("silent renew success", user);
    }).catch(function (e) {
        console.log("silent renew error", e.message);
    })
});

mgr.events.addAccessTokenExpired(function () {
    console.log("token expired");
    mgr.signinSilent().then(function (user) {
        console.log("silent renew success", user);
    }).catch(function (e) {
        console.log("silent renew error", e.message);
    })
});

mgr.events.addSilentRenewError(function (e) {
    console.log("silent renew error", e.message);
});

mgr.events.addUserLoaded(function (user) {
    console.log("user loaded", user);
    mgr.getUser().then(function () {
        console.log("getUser loaded user after userLoaded event fired");
    });
});

mgr.events.addUserUnloaded(function (e) {
    console.log("user unloaded");
});

mgr.events.addUserSignedIn(function (e) {
    console.log("user logged in to the token server");
});
mgr.events.addUserSignedOut(function (e) {
    console.log("user logged out of the token server");
});

///////////////////////////////
// functions for UI elements
///////////////////////////////
function clearState() {
    mgr.clearStaleState().then(function () {
        console.log("clearStateState success");
    }).catch(function (e) {
        console.log("clearStateState error", e.message);
    });
}

function getUser() {
    mgr.getUser().then(function (user) {
        console.log("got user", user);
    }).catch(function (err) {
        console.log(err);
    });
}

function removeUser() {
    mgr.removeUser().then(function () {
        console.log("user removed");
    }).catch(function (err) {
        console.log(err);
    });
}

function startSigninMainWindow() {
    var someState = { message: 'some data' };
    mgr.signinRedirect({ state: someState, useReplaceToNavigate: true }).then(function () {
        console.log("signinRedirect done");
    }).catch(function (err) {
        console.log(err);
    });
}

function endSigninMainWindow() {
    mgr.signinCallback().then(function (user) {
        console.log("signed in", user);
        // this is how you get the state after the login:
        var theState = user.state;
        var theMessage = theState.message;
        console.log("here's our post-login state", theMessage);
    }).catch(function (err) {
        console.log(err);
    });
}

function startSigninMainWindowDiffCallbackPage() {
    mgr.signinRedirect({ state: 'some data', redirect_uri: url + '/code-identityserver-sample-callback.html' }).then(function () {
        console.log("signinRedirect done");
    }).catch(function (err) {
        console.log(err);
    });
}

function popupSignin() {
    mgr.signinPopup({ state: 'some data' }).then(function (user) {
        console.log("signed in", user);
    }).catch(function (err) {
        console.log(err);
    });
}

function popupSignout() {
    mgr.signoutPopup({ state: 'some data' }).then(function () {
        console.log("signed out");
    }).catch(function (err) {
        console.log(err);
    });
}

function iframeSignin() {
    mgr.signinSilent({ state: 'some data' }).then(function (user) {
        console.log("signed in", user);
    }).catch(function (err) {
        console.log(err);
    });
}

function querySessionStatus() {
    mgr.querySessionStatus().then(function (status) {
        console.log("user's session status", status);
    }).catch(function (err) {
        console.log(err);
    });
}

function startSignoutMainWindow() {
    mgr.signoutRedirect({ state: 'some data' }).then(function (resp) {
        //mgr.signoutRedirect().then(function(resp) {
        console.log("signed out", resp);
    }).catch(function (err) {
        console.log(err);
    });
};

function endSignoutMainWindow() {
    mgr.signoutCallback().then(function (resp) {
        console.log("signed out", resp);
    }).catch(function (err) {
        console.log(err);
    });
};


async function getAuthInfo () {
    var user = await mgr.getUser();
    authInfo = null;

    if (user) {
        if (!user.expired) {
            authInfo = {
                jwt: user.access_token,
                uid: user.profile.sub
            };

        };
    };
    return authInfo;
}


mgr.getUser().then(function (user) {

    if (user != null) {
        console.log("user ok")
    } else {
         startSigninMainWindow();
    }
    
}).catch(function (err) {
    console.log(err);
});

 