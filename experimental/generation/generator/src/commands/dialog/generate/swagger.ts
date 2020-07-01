/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { Command, flags } from '@microsoft/bf-cli-command';
import * as gen from '../../../library/dialogGenerator';
import * as swaggerGen from '../../../library/swaggerDialogGenerator';
import * as ppath from 'path';

export default class Swagger extends Command {
  static description = '[PREVIEW] Generate JSON schema given swagger file.'

  static examples = [`
      $ bf dialog:generate:swagger ./petSwagger.json -o . -r /store/order -m post -p dialog.response -n petSearch.schema`]

  static args = [
    { name: 'path', required: true, description: 'The path to the swagger file' },
  ]

  static flags: flags.Input<any> = {
    method: flags.string({ char: 'm', description: 'API method.', required: true, default: 'GET' }),
    name: flags.string({ char: 'n', description: 'Define schema name.', required: true }),
    output: flags.string({ char: 'o', description: 'Output path for generated swagger schema files. [default: .]', default: '.', required: false }),
    route: flags.string({ char: 'r', description: 'Route to the specific api.', required: true }),
    verbose: flags.boolean({ description: 'Output verbose logging of files as they are processed.', default: false }),
  }

  async run() {
    const { args, flags } = this.parse(Swagger)
    try {
      let projectName = flags.name
      await swaggerGen.generate(args.path, flags.output, flags.method, flags.route, projectName,
        (type, msg) => {
          if (type === gen.FeedbackType.message
            || type === gen.FeedbackType.error
            || (type === gen.FeedbackType.info && flags.verbose)) {
            this.progress(msg)
          }
        });
      let schemaName = ppath.join(flags.output, projectName)
      this.progress(`Schema: ${schemaName}`)
      return true;
    } catch (e) {
      this.thrownError(e)
    }
  }

  thrownError(err: Error): void {
    this.error(err.message)
  }

  progress(msg: string): void {
    this.error(msg)
  }
}
