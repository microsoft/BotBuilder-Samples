const path = require('path')
const fs = require('fs-extra')

const updateUrls = async () => {
  let plugins = ['config', 'chatdown', 'dialog', 'qnamaker', 'luis', 'plugins', 'cli']
  for (let i = 0; i < plugins.length; i++) {
      await cleanUrls(plugins[i])
  }
}

const cleanUrls = async function(plugin) {
  let readmePath = path.join(__dirname, `./../packages/${plugin}/README.md`)
  let fileContent = await fs.readFile(readmePath)
  fileContent = fileContent.toString().replace(/\/blob\/v1\.0\.0/g, "").replace(/\\/g, "/")
  await fs.writeFile(readmePath, fileContent)
}

const run = async () => {
  await updateUrls()
  process.exit(0)
}

run()