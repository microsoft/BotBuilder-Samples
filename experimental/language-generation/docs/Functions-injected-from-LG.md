# Functions injected from LG library

## Functions
- [ActivityAttachment](#ActivityAttachment)
- [template](#template)
- [fromFile](#fromFile)
- [isTemplate](#isTemplate)

<a name="ActivityAttachment"></a>
### ActivityAttachment

Return an activityAttachment from objects.

```
ActivityAttachment(<collection-of-objects>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection-of-objects*> | Yes | Array or object  | A collection of objects|
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*activityAttachment*> | Object | An activityAttachment formed from the input |
||||

*Example*

This example converts an collection of objects to an activityAttachment.
Suppose we want to convert a JSON file to herocard attachment. 
the content in the herocard.json: 

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
    "type": "@{type}",
    "title": "@{title}",
    "text": "@{title}",
    "value": "@{value}"
  }
}
```

By calling ActivityAttachment in a template, type, title, value are passed from template name.

```
# externalHeroCardActivity(type, title, value)
[Activity
    attachments = @{ActivityAttachment(json(fromFile('.\\herocard.json')), 'herocard')}
]
```

And returns a herocard:

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
            "type": "@{type}",
            "title": "@{title}",
            "text": "@{title}",
            "value": "@{value}"
        }
    }
}
```

<a name="template"></a>
### template

Return the evaluated result of the template name and parameters.

```
template(<collection-of-objects>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection-of-objects*> | Yes | Array or object  | A collection of objects|
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*evaluated-result*> | Object | the result evaluated from the template as a function  |
||||

*Example*

This example evaluates the result of calling the template as a function :
Suppose we have template:
    
    # welcome(userName)

    - Hi @{userName}

    - Hello @{userName}

    - Hey @{userName}

```
template("welcome", new { userName = "DL" }.ToString())
```

And returns one of the results:

```
Hi DL
Hello DL
Hey DL
```

<a name="fromFile"></a>

### fromFile

Return the evaluated result from the expression in the given file

```
fromFile(<filepath>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*filepath*> | Yes | string  | path of the a file |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*function*> | Function | A function convert object to activityAttachment |
||||

*Example*

This example evaluate the result from the given filepath :
Suppose we have a file whose filepath is:  

`/home/user/test.txt`.

The content of the file is 

`add(1,2)`

```
fromFile('/home/user/test.txt')
```

And returns this result: 

`3`

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

This example use isTempalte to check whether the template name is in the evaluator:
Suppose we have evalutor contains these templates:

```
['welcome', 'show-alarms', 'add-to-do']
```

By calling

```
isTemplate("welcome")
```

And returns one of the results:

```
true
```

```
isTemplate("delete-to-do")
```

And returns one of the results:

```
false
```