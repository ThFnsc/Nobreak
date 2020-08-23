/**
 * @param {Number} seconds
 * @returns {string}
 */
function displaySeconds(seconds) {
    var units = Object.entries({
        s: 1,
        m: 60,
        h: 60,
        d: 24,
        w: 7,
        mo: (365.25/12)/7,
        y: 12,
        dec: 10,
        c: 10
    });
    for (var i = 1; i < units.length; i++)
        units[i][1] *= units[i - 1][1];
    var values = [];
    for (var i = units.length - 1; i >= 0; i--) {
        var result = Math.floor(seconds / units[i][1]);
        if (result >= 1) {
            seconds %= units[i][1];
            values.push([units[i][0], result]);
        }
    }
    if (values.length == 0)
        values.push([units[0][0], 0]);
    return values.map(([key, value]) => value + key).join(" ");
}

/*function displaySeconds(seconds) {
    var units = {
        d: seconds / 86400,
        h: seconds % 86400 / 3600,
        m: seconds % 3600 / 60,
        s: seconds % 60 / 1
    };
    return Object.entries(units).map(([key, value]) => [key, Math.floor(value)]).map(([key, value]) => value > 0 || key == "s" ? value + key : null).filter(v => v).join(" ");
}*/

/**
 * @returns {string}
 * @param {string} date
 */
function formatDate(date) {
    return moment(date).format("DD/MM/YYYY hh:mm:ss A");
}

/**
 * @returns {string}
 * @param {boolean} v
 */
function simNao(v) {
    return v ? "Sim" : "Não";
}

/**
 * @returns {string}
 * @param {string} input
 */
function capitalizeFirstLetters(input) {
    return input.split(" ").map(v => v.substr(0, 1).toUpperCase() + v.substr(1)).join(" ");
}