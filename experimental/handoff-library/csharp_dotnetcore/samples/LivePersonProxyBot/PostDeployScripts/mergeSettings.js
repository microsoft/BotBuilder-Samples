const fs = require('fs');
const path = require('path');
const appSettingPath = path.resolve('./appsettings.json');
const envPath = path.resolve('./.env');

const settings = readSettingsFile(appSettingPath, envPath);
const env = process.env;

const envNames = Object.keys(env)
    .filter(function(x){ return x.indexOf("APPSETTING_") === 0;})
    .filter(function(x){ return x.indexOf("APPSETTING_WEBSITE_") < 0; });
const appSettings = envNames.reduce(function(cur, key) { return {...cur, [key.substring(11)]: env[key]};}, {});

const newSettings = {...settings, ...appSettings};

updateSettingsFile(appSettingPath, envPath, newSettings);

function readDotEnv(file) {
    const text = fs.readFileSync(file, 'utf-8');
    const lines = text.split(/[\r\n]/);
    const settings = lines.reduce(function(cur, line) {
        const parts = line.split('=');
        if (parts.length >= 2) {
            const name = parts.splice(0, 1);
            cur[name] = parts.join('=');
        }
        return cur;
    }, {});

    return settings;
}

function readSettingsFile(appSettingPath, envPath) {
    if (fs.existsSync(appSettingPath)) {
        return JSON.parse(fs.readFileSync(appSettingPath, 'utf-8'));
    }

    if (fs.existsSync(envPath)) {
        return readDotEnv(envPath);
    }

    return {};
}

function isNodeJSProject() {
    return fs.existsSync(path.resolve('./iisnode.yml'));
}

function updateSettingsFile(appSettingPath, envPath, settings) {
    if (isNodeJSProject()) {
        const keys = Object.keys(settings);
        const lines = keys.reduce(function(cur, key) {
            return [...cur, key + '=' + settings[key]];
        }, []);
        fs.writeFileSync(envPath, lines.join('\n'), {encoding: 'utf-8'});
    } else {
        fs.writeFileSync(appSettingPath, JSON.stringify(settings, null, 2), {encoding: 'utf-8'});
    }
}
