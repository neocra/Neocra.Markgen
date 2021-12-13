using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Neocra.Markgen.Domain;

public class HtmlEngine
{
    private readonly HtmlEngineLoader htmlEngineLoader;

    public HtmlEngine(HtmlEngineLoader htmlEngineLoader)
    {
        this.htmlEngineLoader = htmlEngineLoader;
    }

    public async Task<string> CompileRenderAsync<T>(string view, T modelMarkdownFile)
    {
        var scriptObject = new ScriptObject();
        scriptObject.Import(modelMarkdownFile);

        var context = new TemplateContext
        {
            TemplateLoader = this.htmlEngineLoader
        };

        context.PushGlobal(scriptObject);

        var path = this.htmlEngineLoader.GetPath(context, new SourceSpan(), view);

        var template = await this.htmlEngineLoader.LoadAsync(context, new SourceSpan(), path);

        return await Template.Parse(template).RenderAsync(context);
    }
}