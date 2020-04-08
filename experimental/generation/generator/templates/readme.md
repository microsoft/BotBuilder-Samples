# Standard Templates

The tool includes a standard set of templates described below. They can be extended or
overridden by your own templates. Current functionality includes:
- Generating .lg, .lu and .dialog files that robustly handle out of order and
  multiple responses for simple and array properties.
- Add, remove, clear and show for properties.
- Support for choosing between ambiguous entity values and entity property mappings.
- Recognizing and mapping all LUIS prebuilt entities.
- Help including auto-help on multiple retries.
- Cancel
- Confirmation

## Usage
To include the standard schema in your file you would add the below at the top
level of your JSON schema file.  
```
    "$requires": [
        "standard.schema"
    ]
``` 

### LUIS Prebuilt Entities
You can also refer to schemas to define more complex properties.  For example
this would define a property `Length` that makes use of the
[dimension.schema](dimension.schema) to define a type with number and units. In
addition, it also automatically includes the appropriate generation templates to
utilize and map the LUIS prebuilt dimension entity. It makes use of the
`template:` protocol which will look in your template files for the named
schema.  
```
        "Length": {
            "$ref": "template:dimension.schema#/dimension"
        }
```

Prebuilt structured LUIS entities:
* [age.schema](age.schema) LUIS prebuilt `age`.
* [datetime.schema](datetime.schema) LUIS prebuilt `datetimeV2`.
* [dimension.schema](dimension.schema) LUIS prebuilt `dimension`.
* [geography.schema](geography.schema) LUIS prebuilt `geographyV2`.
* [money.schema](money.schema) LUIS prebuilt `money`.
* [ordinal.schema](ordinal.schema) LUIS prebuilt `ordinalV2`.
* [temperature.schema](temperature.schema) LUIS prebuilt `temperature`.

## Organization
Each possible JSON Schema type has a root template file, like
[string.lg](string.lg) or [enum.lg](enum.lg) which defines the default `#
entities` and `# templates` for that type. The templates make use of a
generation time libray [generator.lg](generator.lg) which defines template
commands that are useful when writing generation templates. In the template
descriptions below, italics are place holders that are defined by entity or
property types. Templates provide a lot of flexibility so the patterns below
might have additional templates.  

Most property types have template files for:
* $type$-missing.dialog.lg -- Ask for a missing property of $type$.
* $type$-clear.dialog.lg -- Clear a property of $type$.
* $type$-show.dialog.lg -- Show a property of $type$.
* $type$Property.lg.lg -- Define the .lg resources to name, ask for and show a
  property of $type$.

There are also template files for the entities that are defin in a `# entities` template,
or a schema `$entities`.  
For each entity there is usually:
* $entity$Entity.lg.lg -- Define the .lg resources for showing an $entity$
  value.
* $entity$Entity.lu.lg -- Define the .lu resources need for recognizing an
  $entity$ value and its property.
* $entity$Entity-$type$.lg -- Define the templates needed for mapping $entity$
  to a property of $type$.
* $entity$Entity-add-$type$.dialog.lg -- Define how to add an $entity$ to a
  property of $type$.
* $entity$Entity-remove-$type$.dialog.lg -- Define how to remove an $entity$
  from a property of $type$.

In addition to property and entity templates there are also schema files like
[standard.schema](standard.schema) or [age.schema](age.schema) that can be
explicitly included into your schema in order to bring in more advanced
functionality.

The standard templates extend the naming conventions to add -$operation$ to the
end of files to reflect a particular built-in operation.  



