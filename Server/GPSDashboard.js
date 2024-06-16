// gps dashboard store number/market
function login1() {
    localStorage.setItem("dashboardConfig", '{"storeNo":"' + gpsStoreNumber + '","market":"AUSTRALIA"}');
    window.location.reload();
}

// microsoft email login
function login2() {
    let email = document.querySelector("input[name='loginfmt']");

    // When we enter the password the page is reloaded to show the "Stay signed in" page.
    if (email == null) {
        login4();
        return;
    }

    email.value = gpsEmail;
    email.dispatchEvent(new Event("input", { bubbles: true }));

    let submit = document.querySelector("input[type='submit']");
    submit.click();

    setTimeout(login3, 5000);
}

// microsoft password login
function login3() {
    var password = document.querySelector("input[name='passwd']");
    password.value = gpsPass;
    password.dispatchEvent(new Event("input", { bubbles: true }));

    let submit = document.querySelector("input[type='submit']");
    submit.click();

    // The page is reloaded here to show the "Stay signed in" page so do nothing
}

// microsoft "stay logged in"
function login4() {
    var dontShowAgain = document.querySelector("input[name='DontShowAgain']");

    ///@TODO redirect back to gps dashboard
    if (dontShowAgain == null)
        return;

    dontShowAgain.checked = true;

    let submit = document.querySelector("input[type='submit']");
    submit.click();

    // do nothing else, we will be redirected to gps dashboard
}

function getAuthToken() {
    for (var key of Object.keys(sessionStorage)) {
        var val = JSON.parse(sessionStorage.getItem(key));

        if (!val.hasOwnProperty("credentialType") || !val.hasOwnProperty("secret"))
            continue;

        if (val.credentialType != "AccessToken")
            continue;

        return val.secret;
    }

    return null;
}

function sendAuthToken() {
    const token = getAuthToken();

    if (token == null)
        return;

    fetch(server + "/api/AuthToken", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ bearerToken: "Bearer " + token })
    })
}

function route() {
    if (window.location.origin == "https://gps-dashboard.dominos.com.au") {
        if (document.getElementById("formStoreNumber") == null) {
            sendAuthToken();
        }
        else {
            login1();
        }
    }
    else if (window.location.origin == "https://login.microsoftonline.com") {
        login2();
    }
}

(function () {
    // 5 second time out to make sure the page is loaded
    setTimeout(route, 5000);
})();
