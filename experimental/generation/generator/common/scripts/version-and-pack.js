// @ts-check
// Ensure using node 12 because of recursive mkdir
if (
    !process.env.GEN_CLDR_DATA_IGNORE_NODE_VERSION &&
    process.version.split('.')[0] < 'v12'
) {
    console.error(`
Your node version appears to be below v12: ${ process.version}. 
This script will not run correctly on earlier versions of node. 
Set 'GEN_CLDR_DATA_IGNORE_NODE_VERSION' environment variable to truthy to override`);
}

const fs = require('fs');
const path = require('path');
const cp = require('child_process');
const os = require('os');

// from: https://semver.org/
const semverRegex = /^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?$/
const npm = (process.platform === 'win32' ? 'npm.cmd' : 'npm');
const packDirName = "./.output";
const packDir = path.join(__dirname, "../..", packDirName);

async function main() {
    const plog = prettyLogger('version-and-pack', 'main');

    // Extract version from command line
    const version = extractVersion(process.argv);
    if (version === undefined) {
        throw new TypeError("You must specifiy --version as an argument");
    }
    if (!semverRegex.test(version)) {
        throw new RangeError(`Version must match semver V1 format (i.e. X.X.X-label). Received: "${version}"`);
    }
    plog(`Setting version: ${version}`);

    // Load all modules we care about from rush
    let projects;
    try {
        const rushConfig = require('../../rush.json');
        projects = rushConfig.projects;
        plog("Loading projects:");
        plog(JSON.stringify(projects, null, ' '));
    } catch (e) {
        plog("Could not load projects from rush.json");
        throw e;
    }

    try {
        plog("Creating output directory: " + packDir);
        createIfNotExistSync(packDir);
    } catch (e) {
        plog("Could not create output directory");
        throw e;
    }
    for (const project of projects) {
        plog("Bumping version for " + project.packageName + " to " + version);
        const pathToPackage = path.join(__dirname, "../../", project.projectFolder);
        await exec(npm, ['version', version, '--allow-same-version'], { cwd: pathToPackage });

        plog("Updating dependencies in package.json");
        const packageJsonPath = path.join(pathToPackage, 'package.json');
        const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));

        for (const dep of projects) {
            if (packageJson.dependencies[dep.packageName]) {
                plog("Updating version in " + project.packageName + ": " + dep.packageName + " -> " + version);
                packageJson.dependencies[dep.packageName] = version;
            }
        }

        fs.writeFileSync(packageJsonPath, JSON.stringify(packageJson, null, ' ') + '\n', {
            encoding: 'utf8'
        });

        plog("Packing " + project.packageName);
        const output = await exec(npm, ['pack'], { cwd: pathToPackage });
        const tgz = parseTgz(output);
        plog('found tgz: ' + tgz);
        fs.copyFileSync(path.join(pathToPackage, tgz), path.join(packDir, tgz));
    }

    plog('Complete');
}

function extractVersion(argv) {
    for (let i = 1; i < argv.length; i++) {
        if (argv[i - 1] === '--version') {
            return argv[i].trim();
        }
    }
}

function parseTgz(text) {
    const lines = text.split('\n');
    for (const line of lines) {
        if (line.endsWith('.tgz')) {
            return line;
        }
    }
}

async function exec(command, args, opts) {
    const stdout = prettyLogger(command, 'stdout');
    const stderr = prettyLogger(command, 'stderr');
    const error = prettyLogger(command, 'error');

    return new Promise((resolve, reject) => {
        const p = cp.spawn(command, args, opts);
        let buffer = '';

        p.stdout.on('data', data => {
            buffer += data;
            stdout(`[${command}][stdout]: ${data}`);
        });

        p.stderr.on('data', data => {
            stderr(`[${command}][stderr]: ${data}`);
        });

        p.on('error', err => {
            error(err);
        });

        p.on('close', code => {
            if (code !== 0) {
                return reject(new Error(`"${command} ${args.join(' ')}" returned unsuccessful error code: ${code}`));
            } else {
                resolve(buffer);
            }
        });
    });
}

function createIfNotExistSync(path) {
    try {
        fs.mkdirSync(path, { recursive: true });
    } catch (e) {
        if (!e.code === 'EEXIST') {
            throw e;
        }
    }
}

function prettyLogger(...labels) {
    const header = `[${labels.join('][')}]: `;
    return (content) => {
        if (typeof content !== 'string') {
            content = JSON.stringify(content, null, ' ');
        }
        const lines = content.split('\n');
        lines.forEach((v) => console.log(header + v));
    };
}

main().catch(err => {
    console.error(err);
    process.exit(1);
}).then(() => {
    process.exit(0);
});
