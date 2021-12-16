using System.Threading.Tasks;
using NSubstitute;
using Scriban;
using Xunit;
using Xunit.Abstractions;

namespace Neocra.Markgen.Tests;

public class RenderHtmlTests : BaseTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public RenderHtmlTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }
    

    [Fact]
    public async Task Should_render_page_to_html_When_build_directory()
    {
        AddFileProviderFactory(p =>
        {
            AddGetDirectoryContents(p, "", GetFileInfo("Toto.md", "/Toto.md"));
        });
        
        await Program.RunAsync(this.Services, new XuniTestConsole(this.testOutputHelper), "build", "--source", "/");

        await this.Scriban.Received(1)
            .RenderAsync(Arg.Any<string>(), Arg.Any<TemplateContext>());
    }

    [Fact]
    public async Task Should_readme_file_is_convert_to_index_html_When_build_directory()
    {
        AddFileProviderFactory(p =>
        {
            AddGetDirectoryContents(p, "", GetFileInfo("README.md", "/README.md"));
        });
        
        await Program.RunAsync(this.Services, new XuniTestConsole(this.testOutputHelper), "build", "--source", "/");

        await this.FileWriter.Received(1)
            .Received(1)
            .WriteAllTextAsync(".markgen/index.html", Arg.Any<string>());
    }
    
    [Fact]
    public async Task Should_readme_file_is_convert_to_index_html_When_build_directory_and_sub_directory()
    {
        AddFileProviderFactory(p =>
        {
            AddGetDirectoryContents(p, "", GetDirectoryInfo("subPath", "/subPath"));
            AddGetDirectoryContents(p, "subPath", GetFileInfo("README.md", "/subPath/README.md"));
        });
        
        await Program.RunAsync(this.Services, new XuniTestConsole(this.testOutputHelper), "build", "--source", "/");

        await this.FileWriter.Received(1)
            .Received(1)
            .WriteAllTextAsync(".markgen/subPath/index.html", Arg.Any<string>());
    }
}