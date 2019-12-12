# Functions injected from LG library

## Functions
- [ActivityAttachment](#ActivityAttachment)
- [template](#template)
- [fromFile](#fromFile)

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

This example converts an collection of objects to an activityAttachment :
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

```
ActivityAttachment(json(fromFile('.\\herocard.json')), 'herocard')
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


### template

Return a function converts objects to an activityAttachment.

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
