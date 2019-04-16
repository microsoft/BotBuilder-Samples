# API reference for LG 

For Nuget packages, see [this MyGet feed][1]

### TemplateEngine Class

#### Fields
``` C#
/// <summary>
/// Parsed LG templates
/// </summary>
public List<LGTemplate> Templates = new List<LGTemplate>();
```

#### Constructors
```C#
/// <summary>
/// Return an empty engine, you can then use AddFile\AddFiles to add files to it, 
/// or you can just use this empty engine to evaluate inline template
/// </summary>
public TemplateEngine()
```
```C#
/// <summary>
/// Create a template engine from files, a shorthand for 
///    new TemplateEngine().AddFiles(filePath)
/// </summary>
/// <param name="filePaths">paths to LG files</param>
/// <returns>Engine created</returns>
public static TemplateEngine FromFiles(params string[] filePaths)
```

```C#
/// <summary>
/// Create a template engine from text, equivalent to 
///    new TemplateEngine.AddText(text)
/// </summary>
/// <param name="text">Content of lg file</param>
/// <returns>Engine created</returns>
public static TemplateEngine FromText(string text)
```

#### Methods
```C#
/// <summary>
/// Load .lg files into template engine
/// 
/// You can add one file, or mutlple file as once
/// 
/// If you have multiple files referencing each other, make sure you add them all at once,
/// otherwise static checking won't allow you to add it one by one
/// </summary>
/// <param name="filePaths">Paths to .lg files</param>
/// <returns>Teamplate engine with parsed files</returns>
public TemplateEngine AddFiles(params string[] filePaths)
```


```C#
/// <summary>
/// Add text as lg file content to template engine
/// </summary>
/// <param name="text">Text content contains lg templates</param>
/// <returns>Template engine with the parsed content</returns>
public TemplateEngine AddText(string text)
```

```C#
/// <summary>
/// Evaluate a template with given name and scope
/// </summary>
/// <param name="templateName">Template name to be evaluated</param>
/// <param name="scope">The state visible in the evaluation</param>
/// <param name="methodBinder">Optional methodBinder to extend or override functions</param>
/// <returns></returns>
public string EvaluateTemplate(string templateName, object scope, IGetMethod methodBinder = null)
```

```C#
/// <summary>
/// Analyze a given template and return it's referenced variables
/// </summary>
/// <param name="templateName">Template name</param>
/// <returns>list of variable names</returns>
public List<string> AnalyzeTemplate(string templateName)
```

[1]:https://botbuilder.myget.org/feed/botbuilder-declarative/package/nuget/Microsoft.Bot.Builder.LanguageGeneration