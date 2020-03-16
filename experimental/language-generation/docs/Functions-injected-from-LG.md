# Functions injected from LG library

## Functions
- [Functions injected from LG library](#functions-injected-from-lg-library)
  - [Functions](#functions)
    - [ActivityAttachment](#activityattachment)
    - [template](#template)
    - [fromFile](#fromfile)
    - [isTemplate](#istemplate)

<a name="ActivityAttachment"></a>
### ActivityAttachment

Return an activityAttachment constructed from an object and a type.

```
ActivityAttachment(<collection-of-objects>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*content*> | Yes | Object  | Object contains the information of attachment |
| <*type*> | Yes | string  | A string represents the type of attachment |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*activityAttachment*> | Object | An activityAttachment formed from the inputs |
||||

*Example*

This example converts an collection of objects to an activityAttachment.

Using ActivityAttachment function in the template body, type, title, value are parameters in the template name.
Suppose we have a template:

```
# externalHeroCardActivity(type, title, value)
[Activity
    attachments = ${ActivityAttachment(json(fromFile('.\\herocard.json')), 'herocard')}
]
```

The content in herocard.json:

```
{
  "title": "titleContent",
  "text": "textContent",
  "Buttons": [
    {
      "type": "imBack",
      "Title": "titleContent",
      "Value": "textContent",
      "Text": "textContent"
    }
  ],
  "tap": {
    "type": "${type}",
    "title": "${title}",
    "text": "${title}",
    "value": "${value}"
  }
}
```

By calling externalHeroCardActivity as a function:

```
externalHeroCardActivity('signin', 'Signin Button', 'http://login.microsoft.com')
```

And it returns a herocard:

```
{
    "lgType" = "attachment",
    "contenttype" = "herocard",
    "content" = {
        "title": "titleContent",
        "text": "textContent",
        "Buttons": [
            {
            "type": "imBack",
            "Title": "titleContent",
            "Value": "textContent",
            "Text": "textContent"
            }
        ],
        "tap": {
            "type": "signin",
            "title": "Signin Button",
            "text": "Signin Button",
            "value": "http://login.microsoft.com"
        }
    }
}
```

<a name="template"></a>
### template

Return the evaluated result of given template name and scope.

```
template(<templateName>, '<param1>', '<param2>', ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*templateName*> | Yes | String  | A string represents the template name |
| <*param1*>,<*param2*>, ... | Yes | Object  | The  parameters passed to the template |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*evaluated-result*> | Object | the result evaluated from the template as a function  |
||||

*Example*

This example evaluates the result of calling the template as a function :
Suppose we have template:

```    
    # welcome(userName)

    - Hi ${userName}

    - Hello ${userName}

    - Hey ${userName}
```

```
template("welcome", "DL")
```

And it returns one of these results:

```
Hi DL
Hello DL
Hey DL
```

<a name="fromFile"></a>

### fromFile

Return the evaluated result of the expression in the given file. 

```
fromFile(<filePath>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*filePath*> | Yes | string  | relative or absolute path of a file contains expressions |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result*> | string | The string representation of the evaluated result |
||||

*Example*

This example evaluates the result from the given file:
Suppose we have a file whose filepath is:  

`/home/user/test.txt`


The content of the file is 


`you have ${add(1,2)} alarms`

```
fromFile('/home/user/test.txt')
```

The fromFile function will evaluate the expression part and the result will replace the original expression. So it returns the result: 

`'you have 3 alarms'`

<a name="isTemplate"></a>
### isTemplate

Return whether a given template name is included in the evaluator.

```
isTemplate(<tempalteName>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*tempalteName*> | Yes | String  | A tempalte name to check |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result*> | Boolean | whether the given template name is included in the evaluator  |
||||

*Example*

This example uses isTempalte function to check whether given template name is in the evaluator:
Suppose we have evalutor contains these templates:

```
# welcome
- hi

# show-alarms
- 7:am and 8:pm

# add-to-do
- you add a task at 7:pm
```

By calling

```
isTemplate("welcome")
```

And it returns the result:

```
true
```

```
isTemplate("delete-to-do")
```

And it returns the result:

```
false
```
