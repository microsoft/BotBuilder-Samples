const fs = require('fs');
const util = require('util');
const axios = require('axios');
const readdir = util.promisify(fs.readdir);

// Generates File Name with Sequence
const geneFileName = async (fileDir) => {
    const filenameConst = 'UserAttachment';
    const files = await readdir(fileDir);
    const filteredFiles = files.filter(f => f.includes(filenameConst)).map(f => parseInt(f.split(filenameConst)[1].split('.')[0]));
    const maxSeq = Math.max(0, filteredFiles);
    const filename = `${ filenameConst }${ maxSeq + 1 }.png`;
    return filename;
};

// Download and Save Streams into File
const response = await axios({ method: 'GET', url: contentUrl, responseType: 'stream' });
await new Promise((resolve, reject) => response.pipe(fs.createWriteStream(filePath)).once('finish', resolve).once('error', reject));

// Returns File Size
const getFileSize = async (FilePath) => {
    const stats = fs.statSync(FilePath);
    return stats.size;
};

module.exports = {
    geneFileName,
    getFileSize,
    writeFile
};
