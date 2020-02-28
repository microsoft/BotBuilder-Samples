# API reference for LG 

For Nuget packages, see [this MyGet C# feed][1] and [this MyGet js feed][2]

### LGFile Class

#### Fields
``` C#
/// <summary>
/// Gets get all templates from current lg file and reference lg files.
/// </summary>
/// <value>
/// All templates from current lg file and reference lg files.
/// </value>
public IList<LGTemplate> AllTemplates {get;}

/// <summary>
/// Gets get all diagnostics from current lg file and reference lg files.
/// </summary>
/// <value>
/// All diagnostics from current lg file and reference lg files.
/// </value>
public IList<Diagnostic> AllDiagnostics {get;}

/// <summary>
/// Gets or sets delegate for resolving resource id of imported lg file.
/// </summary>
/// <value>
/// Delegate for resolving resource id of imported lg file.
/// </value>
public ImportResolverDelegate ImportResolver { get; set; }

/// <summary>
/// Gets or sets templates that this LG file contains directly.
/// </summary>
/// <value>
/// templates that this LG file contains directly.
/// </value>
public IList<LGTemplate> Templates { get; set; }

/// <summary>
/// Gets or sets expression parser.
/// </summary>
/// <value>
/// expression parser.
/// </value>
public ExpressionEngine ExpressionEngine { get; set; }

/// <summary>
/// Gets or sets import elements that this LG file contains directly.
/// </summary>
/// <value>
/// import elements that this LG file contains directly.
/// </value>
public IList<LGImport> Imports { get; set; }

/// <summary>
/// Gets or sets all references that this LG file has from <see cref="Imports"/>.
/// Notice: reference includes all child imports from the LG file,
/// not only the children belong to this LG file directly.
/// so, reference count may >= imports count. 
/// </summary>
/// <value>
/// all references that this LG file has from <see cref="Imports"/>.
/// </value>
public IList<LGFile> References { get; set; }

/// <summary>
/// Gets or sets diagnostics.
/// </summary>
/// <value>
/// diagnostics.
/// </value>
public IList<Diagnostic> Diagnostics { get; set; }

/// <summary>
/// Gets or sets LG content.
/// </summary>
/// <value>
/// LG content.
/// </value>
public string Content { get; set; }

/// <summary>
/// Gets or sets id of this LG file.
/// </summary>
/// <value>
/// id of this lg source. For file, is full path.
/// </value>
public string Id { get; set; }
```

#### Constructors
```C#
public LGFile(
IList<LGTemplate> templates = null,
IList<LGImport> imports = null,
IList<Diagnostic> diagnostics = null,
IList<LGFile> references = null,
string content = null,
string id = null,
ExpressionEngine expressionEngine = null,
ImportResolverDelegate importResolver = null)
{
    Templates = templates ?? new List<LGTemplate>();
    Imports = imports ?? new List<LGImport>();
    Diagnostics = diagnostics ?? new List<Diagnostic>();
    References = references ?? new List<LGFile>();
    Content = content ?? string.Empty;
    ImportResolver = importResolver;
    Id = id ?? string.Empty;
    ExpressionEngine = expressionEngine ?? new ExpressionEngine();
}
```

#### Methods

```C#
/// <summary>
/// Evaluate a template with given name and scope.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <param name="scope">The state visible in the evaluation.</param>
/// <returns>Evaluate result.</returns>
public object EvaluateTemplate(string templateName, object scope = null)
```

```C#
/// <summary>
/// Expand a template with given name and scope.
/// Return all possible responses instead of random one.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <param name="scope">The state visible in the evaluation.</param>
/// <returns>Expand result.</returns>
public IList<string> ExpandTemplate(string templateName, object scope = null)
```

```C#
/// <summary>
/// (experimental)
/// Analyzer a template to get the static analyzer results including variables and template references.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <returns>analyzer result.</returns>
public AnalyzerResult AnalyzeTemplate(string templateName)
```

```C#
/// <summary>
/// update an exist template.
/// </summary>
/// <param name="templateName">origin template name. the only id of a template.</param>
/// <param name="newTemplateName">new template Name.</param>
/// <param name="parameters">new params.</param>
/// <param name="templateBody">new template body.</param>
/// <returns>updated LG file.</returns>
public LGFile UpdateTemplate(string templateName, string newTemplateName, List<string> parameters, string templateBody)
```

```C#
/// <summary>
/// Add a new template and return LG File.
/// </summary>
/// <param name="templateName">new template name.</param>
/// <param name="parameters">new params.</param>
/// <param name="templateBody">new  template body.</param>
/// <returns>updated LG file.</returns>
public LGFile AddTemplate(string templateName, List<string> parameters, string templateBody)
```

```C#
/// <summary>
/// Delete an exist template.
/// </summary>
/// <param name="templateName">which template should delete.</param>
/// <returns>updated LG file.</returns>
public LGFile DeleteTemplate(string templateName)
```


[1]:https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/Microsoft.Bot.Builder.LanguageGeneration
[1]:https://botbuilder.myget.org/feed/botbuilder-v4-js-daily/package/npm/botbuilder-lg