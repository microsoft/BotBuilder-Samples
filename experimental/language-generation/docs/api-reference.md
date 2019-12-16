# API reference for LG 

<<<<<<< HEAD
=======
For Nuget packages, see [this MyGet C# feed][1] and [this MyGet js feed][2]

>>>>>>> 60e3a4e07558797a53e0ca1abd3862a0617996eb
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

#### Methods
```C#
 /// <summary>
/// Load .lg files into template engine
/// You can add one file, or mutlple file as once
/// If you have multiple files referencing each other, make sure you add them all at once,
/// otherwise static checking won't allow you to add it one by one.
/// </summary>
/// <param name="filePaths">Paths to .lg files.</param>
/// <param name="importResolver">resolver to resolve LG import id to template text.</param>
/// <returns>Teamplate engine with parsed files.</returns>
public TemplateEngine AddFiles(IEnumerable<string> filePaths, ImportResolverDelegate importResolver = null)
```


```C#
 /// <summary>
/// Load single .lg file into template engine.
/// </summary>
/// <param name="filePath">Path to .lg file.</param>
/// <param name="importResolver">resolver to resolve LG import id to template text.</param>
/// <returns>Teamplate engine with single parsed file.</returns>
public TemplateEngine AddFile(string filePath, ImportResolverDelegate importResolver = null)
```

```C#
/// <summary>
/// Add text as lg file content to template engine. A fullpath id is needed when importResolver is empty, or simply pass in customized importResolver.
/// </summary>
/// <param name="content">Text content contains lg templates.</param>
/// <param name="id">id is the content identifier. If <see cref="importResolver"/> is null, id should must be a full path string. </param>
/// <param name="importResolver">resolver to resolve LG import id to template text.</param>
/// <returns>Template engine with the parsed content.</returns>
public TemplateEngine AddText(string content, string id = "", ImportResolverDelegate importResolver = null)
```

```C#
/// <summary>
/// Evaluate a template with given name and scope.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <param name="scope">The state visible in the evaluation.</param>
/// <param name="methodBinder">Optional methodBinder to extend or override functions.</param>
/// <returns>Evaluate result.</returns>
public string EvaluateTemplate(string templateName, object scope = null, IGetMethod methodBinder = null)
```

```C#
/// <summary>
/// Expand a template with given name and scope.
/// Return all possible responses instead of random one.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <param name="scope">The state visible in the evaluation.</param>
/// <param name="methodBinder">Optional methodBinder to extend or override functions.</param>
/// <returns>Expand result.</returns>
public List<string> ExpandTemplate(string templateName, object scope = null, IGetMethod methodBinder = null)
```

```C#
/// <summary>
/// Analyze a given template and return it's referenced variables
/// </summary>
/// <param name="templateName">Template name</param>
/// <returns>list of variable names</returns>
public List<string> AnalyzeTemplate(string templateName)
```

```C#
/// <summary>
/// Use to evaluate an inline template str.
/// </summary>
/// <param name="inlineStr">inline string which will be evaluated.</param>
/// <param name="scope">scope object or JToken.</param>
/// <param name="methodBinder">input method.</param>
/// <returns>Evaluate result.</returns>
public string Evaluate(string inlineStr, object scope = null, IGetMethod methodBinder = null)
```

[1]:https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/Microsoft.Bot.Builder.LanguageGeneration
[1]:https://botbuilder.myget.org/feed/botbuilder-v4-js-daily/package/npm/botbuilder-lg