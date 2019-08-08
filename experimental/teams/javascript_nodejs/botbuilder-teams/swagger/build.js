let fs = require('fs');
let fsExt = require('fs-extra');
let childProc = require('child_process');
let rimraf = require("rimraf");
let replace = require('replace');

const out = process.argv[2] || './out';

async function autorest() {
  rimraf.sync(out);
  const args = [
    `--typescript`,
    `--input-file=./teamsAPI.json`,
    `--output-folder=${out}`,
    `--override-client-name=TeamsConnectorClient`
  ].join(' ');
  await childProc.execSync(`node ../node_modules/autorest/dist/app.js ${args}`, {stdio: 'inherit'})
}

function replaceStrings() {
  const runReplace= (regex, replacement) => 
    replace({
      regex,
      replacement,
      paths: ['.'],
      recursive: true,
      silent: true,
      include: '*.ts'
    });

  runReplace(`"`, `'`);
  runReplace(`from '../teamsConnectorClientContext'`, `from '../../'`);
}

function insertImportLines (lines, insertLines = []) {
  const firstImportLine = lines.findIndex(l => l.startsWith('import '));
  lines.splice(firstImportLine, 0, ...insertLines);
}

function deleteClassDeclaration (lines, className) {
  const lId = lines.findIndex(l => l.startsWith(`export interface ${className}`));
  if (lId >= 0) {
    let [begId, endId] = [lId, lId];
    while (--begId >= 0) {
      if (lines[begId].startsWith('/**')) {
        break;
      }
    }

    while (++endId < lines.length) {
      if (lines[endId].startsWith('}')) {
        break;
      }
    }
    
    if (begId >= 0 && endId < lines.length) {
      lines.splice(begId, endId - begId + 2); // delete 1 more empty line below
    }
  }
}

function imposeExtBaseClass (lines, extNSOfBotbuilder = 'builder', extNSOfTeams = 'teams', descriptorFile = './extension.json') {
  const json = JSON.parse(fs.readFileSync(descriptorFile).toString());
  const { extBotBuilderTypes, extTeamsTypes } = json;
  const extTypes = [...extBotBuilderTypes, ...extTeamsTypes];

  // 1. delete all existing classes
  extTypes.forEach(clsName => {
    deleteClassDeclaration(lines, clsName);
  });

  // 2. replace to external classes
  const replace = (l, clsName, ns) => {
    const old = l;
    l = l.replace(new RegExp(` ${clsName}(?!\\w)`, 'g'), ` ${ns}.${clsName}`);
    (old !== l) && console.log(`replace:\n${old}\n${l}\n\n`);
    return l;
  };

  lines.forEach((l, lId) => {
    extBotBuilderTypes.forEach(clsName => l = replace(l, clsName, extNSOfBotbuilder));
    extTeamsTypes.forEach(clsName => l = replace(l, clsName, extNSOfTeams));
    lines[lId] = l;
  });
}

function processExtTypes() {
  const modelsFile = `${out}/lib/models/index.ts`;
  let lines = fs.readFileSync(modelsFile).toString().split('\n');
  insertImportLines(lines, [
    `import * as teams from '../extension'`,
    `import * as builder from 'botbuilder'`
  ]);
  imposeExtBaseClass(lines, 'builder', 'teams', './extension.json');
  fs.writeFileSync(modelsFile, lines.join('\n'));
}

function releaseAndcleanup() {
  rimraf.sync(`../src/schema/models`);
  rimraf.sync(`../src/schema/operations`);
  fsExt.moveSync(`${out}/lib/models`, `../src/schema/models`);
  fsExt.moveSync(`${out}/lib/operations`, `../src/schema/operations`);
  rimraf.sync(out);
}

async function build() {
  await autorest();
  replaceStrings();
  processExtTypes();
  releaseAndcleanup();
}

build();