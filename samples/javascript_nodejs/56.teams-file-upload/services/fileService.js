const fs = require('fs');
const util = require('util');
const axios = require('axios');
const readdir = util.promisify(fs.readdir);

// Generates File Name with Sequence
const GeneFileName = async (FileDir)=>{
    let filenameConst = `UserAttachment`
    let files = await readdir(FileDir)
    let filteredFiles  = files.filter(f => f.includes(filenameConst)).map(f => parseInt(f.split(filenameConst)[1].split('.')[0]))
    let maxSeq = 0
    if(filteredFiles.length > 0){
        maxSeq = Math.max.apply(Math, filteredFiles);
    }else{
        maxSeq = 0
    }
    let filename = `${filenameConst}${maxSeq + 1}.png`
    return filename;
}

// Download and Save Streams into File
const WriteFile = async (contentUrl, config, filePath) =>{
    return new Promise(async (resolve)=>{
        const response = await axios.get(contentUrl, config);
        let stream = response.data.pipe(fs.createWriteStream(filePath));
        stream.on('finish', () =>{
            resolve();
        });
    })
}

// Returns File Size
const GetFileSize = async (FilePath) =>{
    stats = fs.statSync(FilePath)
    return stats.size;
}

module.exports = {
    GeneFileName,
    GetFileSize,
    WriteFile
}