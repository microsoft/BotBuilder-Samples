/*!
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { OpenAPIV2 } from 'openapi-types';
import * as ppath from 'path'
import * as fs from 'fs-extra'
import * as sw from 'swagger-parser';

export enum FeedbackType {
  message,
  info,
  warning,
  error
}

export type Feedback = (type: FeedbackType, message: string) => void

// generate the parameter of the http request step
function generateParam(obj: any) {
  switch (obj.type) {
    case 'object':
      // todo: handle complex scenarios
      return undefined

    case 'integer':
      return {
        type: 'number',
        $entities: ['number']
      }

    case 'string':
      if (obj.format == 'date-time') {
        return {
          '$ref': 'template:datetime.schema#/datetime'
        }
      }
      else if ('enum' in obj) {
        let struct_type = {}
        struct_type['type'] = 'string'
        struct_type['enum'] = obj['enum']
        return struct_type
      } else {
        return {
          type: 'string',
          $entities: ['utterance']
        }
      }

    case 'array':
      // todo: handle arry
      return undefined

    case 'boolean':
      return {
        type: 'string',
        $entities: ['boolean']
      }
  }
}

function generateJsonSchema() {
  return {
    $schema: 'http://json-schema.org/draft-07/schema',
    type: 'object',
    properties: {
    },
    required: new Array(),
    $requires: [
      'http.schema'
    ]
  }
}

function generateJsonProperties(url: string, method: string, property: string) {
  return {
    swaggerApi: url,
    swaggerMethod: method,
    swaggerResponse: property,
    swaggerBody: {},
    swaggerHeaders: {}
  }
}

export async function generate(
  path: string,
  output: string,
  method: string,
  route: string,
  property: string,
  projectName: string,
  feedback: Feedback) {
  // need to dereference the swagger file
  let swfile = await sw.dereference(path) as OpenAPIV2.Document
  await fs.remove(output)
  await fs.ensureDir(output)

  let protocol = swfile.schemes ? `${swfile.schemes[0]}://` : 'http://';

  // the name of output schema file to be used in dialogGenerator
  let url = protocol + swfile.host as string + swfile.basePath as string + route;
  let firstTag = false

  // the output schema file structure, pass the swagger related param in
  let result = generateJsonSchema()
  let jsonProperties = generateJsonProperties(url, method, property)
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
            if (subVal.format == 'date-time') {
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
      } else if (param.in == 'path'){
        url = url.replace('{'+param.name, '${$' + param.name)
      }else {
        if (param.format == 'date-time') {
          body[param.name] = '${$' + param.name + '.timex' + '}'
        } else {
          body[param.name] = '${$' + param.name + '}'
        }
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
  let propertiesFile = ppath.join(output, `properties.json`)

  let headers = 	{'User-Agent': 'PostmanRuntime/7.22.0'}
  jsonProperties.swaggerHeaders = headers

  feedback(FeedbackType.info, `Output Dirctory: ${ppath.join(output, projectName)}`);
  feedback(FeedbackType.info, `Output Schema ${ppath.join(output, projectName)}`);
  feedback(FeedbackType.info, `Output Json Properties ${propertiesFile}`);

  await fs.ensureDir(output)
  await fs.writeFile(ppath.join(output, projectName), JSON.stringify(result, null, 4))
  await fs.writeFile(propertiesFile, JSON.stringify(jsonProperties, null, 4))
}

