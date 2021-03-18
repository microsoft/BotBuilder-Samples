# How To Version Our VSIX Templates

This document outlines the steps necessary to bump the version information of the VXIS generated templates.

## About

This VSIX uses parameter substituion to write the template name and version number as a comment into each `.cs` generated file.  This version information is pulled from the `.vstemplate` files.

## How to Update Version Information

Version information is in found in `EchoBot.vstemplate`, `CoreBot.vstemplate`, and `EmptyBot.vstemplate` files.  All 3 of these version strings should be the exact same value in order to ensure they are used consistently.  The version string is used to substitute `$templateversion$` values found in each `.cs` files.


### Update *.vstemplate files

The `.vstemplate` files have a `<CustomParameters></CustomParameters>` tag that needs to be updated.  This tag contains a `<CustomParameter></CustomParameter>` tag that defines a template variable set to a semver string.  It's this version string that is used to replace all occurances of `$templateversion$` in each `*.cs` file.  It looks like the following:

```xml
    <CustomParameters>
      <CustomParameter Name="$templateversion$" Value="4.12.1"/>         <<<-HAND-CRAFTED-semver
    </CustomParameters>
```



## How are the Version Strings Used

The version strings are used in the following ways:

- Provide version information used for string replacement activities during new bot project generation.  Template version information is written out in the following places:
    - Comment headers for all project generated `*.cs` files
