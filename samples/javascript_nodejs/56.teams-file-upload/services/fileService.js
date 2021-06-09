const fs = require('fs');
const util = require('util');
const axios = require('axios');
const readdir = util.promisify(fs.readdir);

// Generates File Name with Sequence
const geneFileName = async (fileDir) => {
    const filenameConst = 'UserAttachment';
    const files = await readdir(fileDir);
    const filteredFiles = files.filter(f => f.includes(filenameConst)).map(f => parseInt(f.split(filenameConst)[1].split('.')[0]));
    let maxSeq = 0;
    if (filteredFiles.length > 0) {
        maxSeq = Math.max.apply(Math, filteredFiles);
    } else {
        maxSeq = 0;
    }
    const filename = `${ filenameConst }${ maxSeq + 1 }.png`;
    return filename;
};

// Download and Save Streams into File
const writeFile = async (contentUrl, config, filePath) => {
    return new Promise((resolve) => {
        axios.get(contentUrl, config).then(response => {
            const stream = response.data.pipe(fs.createWriteStream(filePath));
            stream.on('finish', () => {
                resolve();
            });
        });
    });
};

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
