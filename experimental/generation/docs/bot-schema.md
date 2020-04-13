# JSON Schema for generating dialogs

The JSON schema defines the dialogs that will be generated. Normally, you'll define a list of properties, their types, and optionally any entity types they map to (see [advanced-json-schema](#advanced-json-schema) for additional options).

An example skeleton schema:

```json
{
  "$schema": "http://json-schema.org/draft-07/schema",
  "type": "object",
  "properties": {
    "Name": {
      "type": "string",
    }
  },
  "required": [
    "Name"
  ],
  "$requires": [
      "standard.schema"
  ]  
}
```

## Top-level objects

### `properties`

The `properties` object defines a single piece of information your bot needs to collect.
You can either define the object in the file using the properties listed below, or use the `$ref` keyword to point to an alternative file.
When using one of the pre-built template definitions, use `$ref": "template:{template-name}#/{property}".

**`type`**

The type of expected input. One of `string`, `number`, or `array`. In advanced scenarios you can also define a complex object with `object` that concatenates multiple properties into a single top-level property.

With `string` you can define an `enum` array of allowed values.

```json
"Bread": {
  "type": "string",
  "enum": [
    "white",
    "wholeWheat"
  ]
}
```

With `number` you can define a range of expected values using `minimum` and `maximum`.

```json
"Quantity": {
  "type": "number",
  "minimum": 1,
  "maximum": 10
}
```

With `array` you define the `items` object that defines the items in the array.

```json
"Toppings": {
  "type": "array",
  "items": {
    "type": "string",
    "enum": [
      "tomato",
      "lettuce",
      "pickles"
    ],
    "maxItems": 3
  }
}
```

You can also use the `$ref` keyword to reference a property defined in a separate schema file.
The example below defines a property `Length` that makes use of `dimension` property defined in the [dimension.schema](../generator/templates/dimension.schema) file in the default templates.
In addition, it also automatically includes the appropriate generation templates to utilize and map the LUIS prebuilt dimension entity.
It makes use of the `template:` protocol which looks in your template files for the named schema (rather than referencing it by file path).

```json
"Length": {
  "$ref": "template:dimension.schema#/dimension"
}
```

### `required`

The `required` array is used to list all of the `properties` that are required. Any property included here will need to be successfully completed to proceed to the confirmation flow.

### `$requires`

The `$requires` section is used to provide an additional array of JSON schemas that should be included.
This is different than `$ref` in that you can either use a URL or just a filename which will be looked for in the template directories.
If you want to include the standard confirmation/cancel/navigation functionality you should include include `standard.schema`.

## Advanced JSON Schema

Globally there are a few extra keywords you can add to your schema.
Most of these keywords are automatically filled during generation and you do not need to worry about them if you make use of [standard.schema](../generator/templates/standard.schema).

Extra keywords include:

- **\$public** List of the public properties in the schema.
  By default these are the top-level properties in the root schema and this will be automatically generated if missing.
- **\$expectedOnly** A global list of entities which will only be bound to properties if the property is expected.
  This can be overridden for specific properties as well.
- **\$triggerIntent** Name of the trigger intent--by default the name of the schema.
- **\$templates** Global templates to include.
- **\$defaultOperation** Default operation to use for assigning entities to properties.
  This is overridden for a given Ask by setting `$dialog.expectedOperation`.

 The final schema and all of the required schemas will have the top-level `properties`, `definitions`, `required`, `$expectedOnly`, `$public` and`$templates` merged.
 For other properties, last definition is included.

In addition there are a few extra keywords per-property including:

- **\$entities** List of entity names that can map to a property.
  The order of the entities also defines the precedence to use when resolving entities.
  If not present, default \$entities can be specified in a `# entities` template definition.
- **\$templates** The template names to use for generating assets. The list usually comes from a `# templates` definition in the template for a property type.
- **\$expectedOnly** A list of entities that are only possible if they are expected.
  This overrides the top-level \$expectedOnly.

You can use expression syntax, i.e. `${<expr>}` to dynamically generate schema, either before generation or after generation is done if there are properties that are only available at the end.
`<schemaName>.schema.dialog` will have the resulting schema with all references resolved and expanded to simplify usage in language generation and dialogs.
