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
/// Add text as lg file content to template engine.
/// </summary>
/// <param name="content">Text content contains lg templates.</param>
/// <param name="name">Text name.</param>
/// <param name="importResolver">resolver to resolve LG import id to template text.</param>
/// <returns>Template engine with the parsed content.</returns>
public TemplateEngine AddText(string content, string name, ImportResolverDelegate importResolver)
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