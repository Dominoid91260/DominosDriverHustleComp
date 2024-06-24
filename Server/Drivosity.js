const fields = {
    "param_filtercase": "custom",
    "param_mytime": "2024-01-28 15:17:21",
    "param_offset": "0",
    "param_limit": "1000",
    "role": "ORGANIZATIONAL_STORE_ADMIN",
    "tripType": "normal",
    "filter[DriveScore][max]": "100",
    "filter[DriveScore][min]": "0",
    "filter[SpeedViolation][max]": "51",
    "filter[SpeedViolation][min]": "5",
    "filter[ExtendedStop][max]": "51",
    "filter[ExtendedStop][min]": "0",
    "filter[Duration][max]": "51",
    "filter[Duration][min]": "0",
    "filter[MaxSpeed][max]": "101",
    "filter[MaxSpeed][min]": "0",
    "filter[Distance][max]": "51",
    "filter[Distance][min]": "0",
    "unit": "Metric",
};

function generateFormBody(csrfToken, from, to) {
    let formBody = [
        "id=" + user.UserInfos.StoreId,
        "param_fromdate=" + getDateYYYMMDDString(from),
        "param_todate=" + getDateYYYMMDDString(to),
        "org_id=" + user.UserInfos.OrganizationId,
        "_csrf=" + csrfToken
    ];

    for (let property in fields) {
        let key = encodeURIComponent(property);
        let value = encodeURIComponent(fields[property]);
        formBody.push(key + "=" + value);
    }

    return formBody.join("&");
}

async function getCSRFToken() {
    const response = await fetch("https://dip.drivosity.com/csrfToken");
    const data = await response.json();
    return data._csrf;
}

function getWeekDates() {
    let today = new Date();

    let to = new Date();
    to.setDate(today.getDate() - today.getDay());// set the date to the last sunday

    let from = new Date();
    from.setDate(to.getDate() - 6);// set the date to the monday before that

    return {
        from: from,
        to: to
    };
}

// https://stackoverflow.com/a/29774197
function getDateYYYMMDDString(date) {
    const offset = date.getTimezoneOffset()
    let newDate = new Date(date.getTime() - (offset * 60 * 1000))
    return newDate.toISOString().split('T')[0]
}

async function getDriverOverspeedRecords() {
    const dates = getWeekDates();
    const response = await fetch("https://dip.drivosity.com/api/getTrips", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
        },
        body: generateFormBody(await getCSRFToken(), dates.from, dates.to)
    });

    const data = await response.json();

    if (data.body.totaltrips != data.body.tripsobject.length) {
        console.error("Total trips != Number of trips");
    }

    return data.body.tripsobject;
}

function calculateDriverTotalOverspeeds(overspeedRecords) {
    let overspeeds = {};

    for (let record of overspeedRecords) {
        const driverId = parseInt(record.DeviceId.split("/")[1]);
        const totalOverspeeds = record.drift_device_state["speedingcount_3-5"] +
            record.drift_device_state["speedingcount_5-10"] +
            record.drift_device_state["speedingcount_10+"] +
            record.drift_device_state["speedingcount_10-20"] +
            record.drift_device_state["speedingcount_20+"];
        const currentOverspeeds = overspeeds[driverId] || 0;

        overspeeds[driverId] = currentOverspeeds + totalOverspeeds;
    }

    return overspeeds;
}

async function sendOverspeeds() {
    console.log("Sending overspeeds...");
    const records = await getDriverOverspeedRecords();
    const driverOverspeeds = calculateDriverTotalOverspeeds(records);

    try {
        const dates = getWeekDates();
        await fetch("http://localhost:8080/api/Reports/Overspeeds/" + getDateYYYMMDDString(dates.to), {
            method: "POST",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ DriverOverspeeds: driverOverspeeds })
        });
    }
    catch (e) {
        console.error(e);
        setTimeout(sendOverspeeds, 5000);
    }
}

function login() {
    console.log("Logging in...");

    let email = document.querySelector("#loginForm #email");
    email.value = dipEmail;

    let password = document.querySelector("#loginForm #password");
    password.value = dipPass;

    let login = document.querySelector("#loginForm #login");
    login.click();
}

function route() {
    console.log("main run with origin: " + window.location.origin);
    if (window.location.href == "https://dip.drivosity.com/login") {
        login();
    }
    else if (window.location.href.includes("/live/")) {
        sendOverspeeds();
    }
}
(function () {
    // 5 second time out to make sure the page is loaded
    setTimeout(route, 5000);
})();
