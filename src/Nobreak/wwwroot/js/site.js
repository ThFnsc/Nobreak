var loadedScripts = {};

function loadScript(scriptPath) {
    return new Promise(function (resolve, reject) {
        if (loadedScripts[scriptPath])
            return resolve();
        var script = document.createElement("script");
        script.src = scriptPath;
        script.type = "text/javascript";
        loadedScripts[scriptPath] = true;

        script.onload = resolve;

        script.onerror = reject;

        document["body"].appendChild(script);
    });
}

function getRecaptchaToken(siteKey, action) {
    return new Promise((resolve, reject) =>
        grecaptcha.ready(() =>
            grecaptcha.execute(siteKey, { action })
                .then(resolve)
                .catch(reject)));
}