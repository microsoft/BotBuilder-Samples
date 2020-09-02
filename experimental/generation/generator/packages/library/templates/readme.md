# Overview
These are the default templates included with dialog:generate which cover the standard JSON schema types/formats and LUIS entities.  These templates use the LG syntax to define templates for generating LG/LU/DIALOG files.

# Organization and Conventions
The files are organized so that `generator.lg` which contains non-language specific LG templates that are useful at generation time is at the root and there is a sub-directory for each major schema:
* **`standard`** contains templates for basic forms.
* **`professional`** contains templates professional chit-chat.
* **`swagger`** contains templates for the swagger generator.
Within each directory are language agnostic templates with all language specific files in `<locale>` sub-directories.  Each locale directory also has language specific generation templates in `<schema>.<locale>.lg`.

## Conventions
The overall definition of a template starts with a `<schema>.schema` file which defines JSON schema and a mixture of per-property and global templates for the machinery defined by the template.

There are also `<entity>.schema` files for defining the JSON schema and templates needed for defining how to map complex LUIS entities.

Root templates have a single template `templates` which defines the list of templates that need to be expanded.  These root templates include:
* **`<type>.lg`** defines all the templates to include for that type.
* **`<entity>Entity.lg` defines the templates needed for defining the entity, usually just LG for displaying entity values.
* **`<entity>-<property>.lg`** defines the templates needed to map between `entity` and `property`.

Per-property LG files are defined as `<type>Property.lg.lg` files which 

In addition to the root templates there are also language agnostic templates for generating dialogs.  They use a convention of `<entity>Entity-<operation>-<property>.dialog.lg`.  


## 

