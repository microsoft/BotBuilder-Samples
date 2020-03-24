# API reference for LG 

## Templates Class

### Fields
``` C#
/// <summary>
/// Gets get all templates from current lg file and reference lg files.
/// </summary>
/// <value>
/// All templates from current lg file and reference lg files.
/// </value>
public IList<Template> AllTemplates => new List<Templates> { this }.Union(References).SelectMany(x => x).ToList();

/// <summary>
/// Gets get all diagnostics from current lg file and reference lg files.
/// </summary>
/// <value>
/// All diagnostics from current lg file and reference lg files.
/// </value>
public IList<Diagnostic> AllDiagnostics => new List<Templates> { this }.Union(References).SelectMany(x => x.Diagnostics).ToList();

/// <summary>
/// Gets or sets delegate for resolving resource id of imported lg file.
/// </summary>
/// <value>
/// Delegate for resolving resource id of imported lg file.
/// </value>
public ImportResolverDelegate ImportResolver { get; set; }

/// <summary>
/// Gets or sets expression parser.
/// </summary>
/// <value>
/// expression parser.
/// </value>
public ExpressionParser ExpressionParser { get; set; }

/// <summary>
/// Gets or sets import elements that this LG file contains directly.
/// </summary>
/// <value>
/// import elements that this LG file contains directly.
/// </value>
public IList<TemplateImport> Imports { get; set; }

/// <summary>
/// Gets or sets all references that this LG file has from <see cref="Imports"/>.
/// Notice: reference includes all child imports from the LG file,
/// not only the children belong to this LG file directly.
/// so, reference count may >= imports count. 
/// </summary>
/// <value>
/// all references that this LG file has from <see cref="Imports"/>.
/// </value>
public IList<Templates> References { get; set; }

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

/// <summary>
/// Gets or sets lG file options.
/// </summary>
/// <value>
/// LG file options.
/// </value>
public IList<string> Options { get; set; }

/// <summary>
/// Gets a value indicating whether lG parser/checker/evaluate strict mode.
/// If strict mode is on, expression would throw exception instead of return
/// null or make the condition failed.
/// </summary>
/// <value>
/// A value indicating whether lG parser/checker/evaluate strict mode.
/// If strict mode is on, expression would throw exception instead of return
/// null or make the condition failed.
/// </value>
public bool StrictMode => GetStrictModeFromOptions(Options);
```

### Constructors
```C#
public Templates(
    IList<Template> templates = null,
    IList<TemplateImport> imports = null,
    IList<Diagnostic> diagnostics = null,
    IList<Templates> references = null,
    string content = null,
    string id = null,
    ExpressionParser expressionParser = null,
    ImportResolverDelegate importResolver = null,
    IList<string> options = null)
{
    if (templates != null)
    {
        this.AddRange(templates);
    }

    Imports = imports ?? new List<TemplateImport>();
    Diagnostics = diagnostics ?? new List<Diagnostic>();
    References = references ?? new List<Templates>();
    Content = content ?? string.Empty;
    ImportResolver = importResolver;
    Id = id ?? string.Empty;
    ExpressionParser = expressionParser ?? new ExpressionParser();
    Options = options ?? new List<string>();
}
```

#### Methods

```C#
/// <summary>
/// Parser to turn lg content into a <see cref="LanguageGeneration.Templates"/>.
/// </summary>
/// <param name="filePath"> absolut path of a LG file.</param>
/// <param name="importResolver">resolver to resolve LG import id to template text.</param>
/// <param name="expressionParser">expressionEngine Expression engine for evaluating expressions.</param>
/// <returns>new <see cref="LanguageGeneration.Templates"/> entity.</returns>
public static Templates ParseFile(
    string filePath,
    ImportResolverDelegate importResolver = null,
    ExpressionParser expressionParser = null);

/// <summary>
/// Parser to turn lg content into a <see cref="LanguageGeneration.Templates"/>.
/// </summary>
/// <param name="content">Text content contains lg templates.</param>
/// <param name="id">id is the identifier of content. If importResolver is null, id must be a full path string. </param>
/// <param name="importResolver">resolver to resolve LG import id to template text.</param>
/// <param name="expressionParser">expressionEngine parser engine for parsing expressions.</param>
/// <returns>new <see cref="LanguageGeneration.Templates"/> entity.</returns>
public static Templates ParseText(
    string content,
    string id = "",
    ImportResolverDelegate importResolver = null,
    ExpressionParser expressionParser = null);

/// <summary>
/// Evaluate a template with given name and scope.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <param name="scope">The state visible in the evaluation.</param>
/// <returns>Evaluate result.</returns>
public object Evaluate(string templateName, object scope = null);

/// <summary>
/// Use to evaluate an inline template str.
/// </summary>
/// <param name="text">inline string which will be evaluated.</param>
/// <param name="scope">scope object or JToken.</param>
/// <returns>Evaluate result.</returns>
public object EvaluateText(string text, object scope = null);

/// <summary>
/// Expand a template with given name and scope.
/// Return all possible responses instead of random one.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <param name="scope">The state visible in the evaluation.</param>
/// <returns>Expand result.</returns>
public IList<string> ExpandTemplate(string templateName, object scope = null);

/// <summary>
/// (experimental)
/// Analyzer a template to get the static analyzer results including variables and template references.
/// </summary>
/// <param name="templateName">Template name to be evaluated.</param>
/// <returns>analyzer result.</returns>
public AnalyzerResult AnalyzeTemplate(string templateName);

/// <summary>
/// update an exist template.
/// </summary>
/// <param name="templateName">origin template name. the only id of a template.</param>
/// <param name="newTemplateName">new template Name.</param>
/// <param name="parameters">new params.</param>
/// <param name="templateBody">new template body.</param>
/// <returns>updated LG file.</returns>
public Templates UpdateTemplate(string templateName, string newTemplateName, List<string> parameters, string templateBody);

/// <summary>
/// Add a new template and return LG File.
/// </summary>
/// <param name="templateName">new template name.</param>
/// <param name="parameters">new params.</param>
/// <param name="templateBody">new  template body.</param>
/// <returns>updated LG file.</returns>
public Templates AddTemplate(string templateName, List<string> parameters, string templateBody);

/// <summary>
/// Delete an exist template.
/// </summary>
/// <param name="templateName">which template should delete.</param>
/// <returns>updated LG file.</returns>
public Templates DeleteTemplate(string templateName);

```