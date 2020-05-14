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
        output: flags.string({ char: 'o', description: 'output path'}),
    }

    async run() {
        const { argv, flags } = this.parse(DialogTransform)

        await this.transformDialog(flags.schemaName, flags.input, flags.output)
    }

    async  transformDialog(schemaName: string, input: string, output: string): Promise<boolean> {
        fs.ensureDir(ppath.join(output, 'en-us'))
        let template = await fs.readFile(ppath.join(input, schemaName + '.main.dialog'), 'utf8')
        let objDialog = JSON.parse(template)
        let listTriggers : any[] = []
        
        for(let trigger of objDialog['triggers']){
            let content = await fs.readFile(ppath.join(input, trigger + '.dialog'), 'utf8')
            let objTrigger = JSON.parse(content)
            objTrigger['id'] = trigger
            listTriggers.push(objTrigger)
        }

        objDialog['triggers'] = listTriggers
        await fs.writeFile(ppath.join(output, schemaName + '.main.dialog'), JSON.stringify(objDialog))
        fs.copyFileSync(ppath.join(input,  schemaName + '.schema.dialog'), ppath.join(output,  schemaName + '.schema.dialog'))
        fs.copySync(ppath.join(input, 'en-us'), ppath.join(output, 'en-us'))
        
        return true
    }

}


