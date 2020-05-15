/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Command, flags } from '@microsoft/bf-cli-command';
import * as fs from 'fs-extra';
import * as ppath from 'path';

export default class DialogTransform extends Command {

    static flags: flags.Input<any> = {
        help: flags.help({ char: 'h' }),
        schemaName: flags.string({ char: 's', description: 'schemaName' }),
        input: flags.string({ char: 'i', description: 'input path' }),
        output: flags.string({ char: 'o', description: 'output path' }),
    }

    async run() {
        const { argv, flags } = this.parse(DialogTransform)

        await this.transformDialog(flags.schemaName, flags.input, flags.output)
    }

    /**
    feedback(FeedbackType.info, `*** Old and new both changed, manually merge from ${ppath.join(path, fileName)} ***`)	 * @description：Get all file paths from the specific dir.
 * @param dir Root dir.
 * @param fileList List of file paths.
 */
    getFiles(dir: string, fileList: string[]) {
        fileList = fileList || []
        let files = fs.readdirSync(dir)
        for (let file of files) {
            let name = dir + '/' + file
            if (fs.statSync(name).isDirectory()) {
                this.getFiles(name, fileList)
            } else {
                fileList.push(name)
            }
        }
        return fileList
    }

    async  transformDialog(schemaName: string, input: string, output: string): Promise<boolean> {
        let template = await fs.readFile(ppath.join(input, schemaName + '.main.dialog'), 'utf8')
        let objDialog = JSON.parse(template)
        let listTriggers: any[] = []
        let fileList: string[] = []
        this.getFiles(input, fileList)

        for (let trigger of objDialog['triggers']) {
            let path = fileList.filter(file => file.match(trigger + '.dialog'))[0]
            let content = await fs.readFile(path, 'utf8')
            let objTrigger = JSON.parse(content)
            objTrigger['id'] = trigger
            listTriggers.push(objTrigger)
        }

        objDialog['triggers'] = listTriggers
        await fs.writeFile(ppath.join(output, schemaName + '.main.dialog'), JSON.stringify(objDialog))
        return true
    }

}


