# Overview
These are the default templates included with dialog:generate which cover the standard JSON schema types/formats and LUIS entities.  These templates use the LG syntax to define templates for generating LG/LU/DIALOG files.  Template files can either define how to generate a specific file or containt `templates` to include other templates.

# Organization and Conventions
The files are organized so that `generator.lg` which contains non-language specific LG templates that are useful at generation time is at the root and there is a sub-directory for each major schema:
* **`standard`** contains templates for basic forms.
* **`professional`** contains templates for professional chit-chat.
* **`swagger`** contains templates for the swagger generator.
Within each directory are language agnostic templates with all language specific files in `<locale>` sub-directories.  Each locale directory also has common language specific generation templates in `<schema>.<locale>.lg`.

## Conventions
The overall definition of a template starts with a `<schema>.form` file which defines JSON schema and a mixture of per-property and global templates for the machinery defined by the template.

There are also `<entity>.template` files for defining the JSON schema and templates needed for defining how to map complex LUIS entities.  To use these you use `$ref` inside a property definition.

For language specific files like LG/LU, there is usually a language agnostic file in the root which includes templates that are specialized for the current locale.  This allows including a non-locale specific file in the templates of a schema that then expands into a locale specific file for each locale.

Root templates define a list of templates that need to be expanded.  These root templates include:
* **`<type>.lg`** defines all the templates to include for that type.  Usually this includes:
  * `<type>Property.lg` points to the locale specific file for the standard LG templates.
  * `<type>-missing.dialog` dialog for when property is missing.
  * `<type>-help.dialog` dialog for property specific help.
  * `<type>-clear.dialog` dialog for clearing a property.
  * `<type>-show.dialog` dialog for showing a property.
* **`<entity>Entity-<property>.lg`** defines the templates needed to map between `<entity>` and `<property>`.  Usually includes:
  * `<entity>Entity.lu` LU definitions for entity and property together.
  * `<entity>Entity.lg` LG definitions for entity.
  * `<entity>Entity-add-<type>.dialog` dialog for adding entity to property.
  * `<entity>Entity-remove-<type>.dialog` dialog for removing entity from property.

In addition to the property and entity templates there are also `form-` templates which are specific to the mechanisms that are generated. 

