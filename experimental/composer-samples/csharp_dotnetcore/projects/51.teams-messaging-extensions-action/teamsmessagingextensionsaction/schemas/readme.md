# How to update the schema file

Once the bot has been setup with Composer and we wish to make changes to the schema, the first step in this process is to eject the runtime through the `Runtime Config` in Composer. The ejected runtime folder will broadly have the following structure

```
bot
  /bot.dialog
  /language-generation
  /language-understanding
  /dialogs
     /customized-dialogs
  /schemas
     sdk.schema
```

##### Prequisites

Botframework CLI > 4.10

```
npm i -g @microsoft/botframework-cli
```

> NOTE: Previous versions of botframework-cli required you to install @microsoft/bf-plugin. You will need to uninstall for 4.10 and above.
>
> ```
> bf plugins:uninstall @microsoft/bf-dialog
> ```

- Navigate to to the `schemas (bot/schemas)` folder. This folder contains a Powershell script and a bash script. Run either of these scripts `./update-schema.ps1` or `sh ./update-schema.sh`.

The above steps should have generated a new sdk.schema file inside `schemas` folder for Composer to use. Reload the bot and you should be able to include your new custom action!

## Customizing Composer using the UI Schema

Composer's UI can be customized using the UI Schema. You can either customize one of your custom actions or override Composer defaults.

There are 2 ways to do this.

1. **Component UI Schema File**

To customize a specific component, simply create a `.uischema` file inside of the `/schemas` directory with the same name as the component, These files will be merged into a single `.uischema` file when running the `update-schema` script.

Example:

```json
// Microsoft.SendActivity.uischema
{
  "$schema": "https://schemas.botframework.com/schemas/ui/v1.0/ui.schema",
  "form": {
    "label": "A custom label"
  }
}
```

2. **UI Schema Override File**

This approach allows you to co-locate all of your UI customizations into a single file. This will not be merged into the `sdk.uischema`, instead it will be loaded by Composer and applied as overrides.

Example:

```json
{
  "$schema": "https://schemas.botframework.com/schemas/ui/v1.0/ui.schema",
  "Microsoft.SendActivity": {
    "form": {
      "label": "A custom label"
    }
  }
}
```

#### UI Customization Options

##### Form

| **Property** | **Description**                                                                        | **Type**            | **Default**          |
| ------------ | -------------------------------------------------------------------------------------- | ------------------- | -------------------- |
| description  | Text used in tooltips.                                                                 | `string`            | `schema.description` |
| helpLink     | URI to component or property documentation. Used in tooltips.                          | `string`            |                      |
| hidden       | An array of property names to hide in the UI.                                          | `string[]`          |                      |
| label        | Label override. Can either be a string or false to hide the label.                     | `string` \| `false` | `schema.title`       |
| order        | Set the order of fields. Use "\_" for all other fields. ex. ["foo", "_", "bar"]        | `string[]`          | `[*]`                |
| placeholder  | Placeholder override.                                                                  | `string`            | `schema.examples`    |
| properties   | A map of component property names to UI options with customizations for each property. | `object`            |                      |
| subtitle     | Subtitle rendered in form title.                                                       | `string`            | `schema.$kind`       |
| widget       | Override default field widget. See list of widgets below.                              | `enum`              |                      |

###### Widgets

- checkbox
- date
- datetime
- input
- number
- radio
- select
- textarea
