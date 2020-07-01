/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { OpenAPIV2 } from 'openapi-types'
import * as ppath from 'path'
import * as fs from 'fs-extra'
import * as sw from 'swagger-parser'
import {Feedback, FeedbackType} from './dialogGenerator'

// generate the parameter of the http request step
function generateParam(obj: any) {
  switch (obj.type) {
    case 'integer':
      return {
        type: 'number',
        $entities: ['number']
      }

    case 'string':
      if (obj.format === 'date-time') {
        return {
          $ref: 'template:datetime.schema#/datetime'
        }
      } else if ('enum' in obj) {
        let structType = {}
        structType['type'] = 'string'
        structType['enum'] = obj['enum']
        return structType
      } else {
        return {
          type: 'string',
          $entities: ['utterance']
        }
      }

    case 'boolean':
      return {
        type: 'string',
        $entities: ['boolean']
      }
  }
}

function generateJsonSchema() {
  return {
    // tslint:disable-next-line:no-http-string
    $schema: 'http://json-schema.org/draft-07/schema',
    type: 'object',
    properties: {
    },
    $parameters : {}
    ,
    required: new Array(),
    $requires: [
      'http.schema',
      'standard.schema'
    ]
  }
}

function generateJsonProperties(url: string, method: string, dialogResponse: string) {
  return {
    swaggerApi: url,
    swaggerMethod: method,
    swaggerResponse: dialogResponse,
    swaggerBody: {},
    swaggerHeaders: {}
  }
}

/**
 * Generate JSON schema given the swagger file.
 * @param path Path to swagger file.
 * @param output Where to put generated JSON schema.
 * @param method API method.
 * @param route Route to the specific api.
 * @param projectName JSON schema name.
 * @param feedback Callback function for progress and errors.
 */
export async function generate(
  path: string,
  output: string,
  method: string,
  route: string,
  projectName: string,
  feedback: Feedback) {
  // need to dereference the swagger file
  let swfile = await sw.dereference(path) as OpenAPIV2.Document
  // tslint:disable-next-line:no-http-string
  let protocol = swfile.schemes ? `${swfile.schemes[0]}://` : 'http://';

  // the name of output schema file to be used in dialogGenerator
  let url = protocol + swfile.host as string + swfile.basePath as string + route;
  let firstTag = false

  // the output schema file structure, pass the swagger related param in
  let result = generateJsonSchema()
  let dialogResponse = 'dialog.response'
  let jsonProperties = generateJsonProperties(url, method, dialogResponse)
  let body = {}
  
  for (let param of swfile.paths[route][method].parameters) {
    if (param.type === undefined) {
      if (param.schema !== undefined && param.schema.properties !== undefined) {
        let subBody = {}
        for (let subParam in param.schema.properties) {
          let subVal = param.schema.properties[subParam]
          let subGenerated = generateParam(subVal)
          if (subGenerated) {
            result.properties[subParam] = generateParam(subVal)
            if (subVal.format === 'date-time') {
              subBody[subParam] = '${$' + subParam + '.timex' + '}'
            } else {
              subBody[subParam] = '${$' + subParam + '}'
            }
            result.required.push(subParam)
          }
        }
        body[param.name] = subBody
      }
    } else {
      if (param.in === 'query') {
        if (!firstTag) {
          url = url + '?' + param.name + '=${$' + param.name + '}'
          firstTag = true
        } else {
          url = url + '&' + param.name + '=${$' + param.name + '}'
        }
      } else if (param.in === 'path') {
        url = url.replace('{'+param.name, '${$' + param.name)
      } 
      result.properties[param.name] = generateParam(param)
      result.required.push(param.name)
    }
  }

  jsonProperties.swaggerApi = url;
  if ('body' in body) {
    jsonProperties.swaggerBody = body['body'];
  } else {
    jsonProperties.swaggerBody = body;
  }

  let headers = 	{'User-Agent': 'Mozilla/5.0'}
  jsonProperties.swaggerHeaders = headers

  result.$parameters = jsonProperties

  feedback(FeedbackType.info, `Output Dirctory: ${ppath.join(output, projectName)}`);
  feedback(FeedbackType.info, `Output Schema ${ppath.join(output, projectName)}`);

  await fs.writeFile(ppath.join(output, projectName), JSON.stringify(result, null, 4))
}
