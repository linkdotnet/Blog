﻿using System;
using System.IO;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using Microsoft.AspNetCore.Components;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Services;

public sealed class SitemapServiceTests : IDisposable
{
    private const string OutputDirectory = "wwwroot";
    private const string OutputFilename = $"{OutputDirectory}/sitemap.xml";
    private readonly SitemapService sut;

    public SitemapServiceTests()
    {
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        sut = new SitemapService(repositoryMock, Substitute.For<NavigationManager>(), new XmlFileWriter());
        Directory.CreateDirectory("wwwroot");
    }

    [Fact]
    public async Task ShouldSaveSitemapUrlInCorrectFormat()
    {
        var urlSet = new SitemapUrlSet
        {
            Urls =
            [
                new SitemapUrl { Location = "here", }
            ],
        };
        await sut.SaveSitemapToFileAsync(urlSet);

        var lines = await File.ReadAllTextAsync(OutputFilename, TestContext.Current.CancellationToken);
        lines.ShouldBe(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<urlset xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
  <url>
    <loc>here</loc>
  </url>
</urlset>");
    }

    public void Dispose()
    {
        if (File.Exists(OutputFilename))
        {
            File.Delete(OutputFilename);
        }

        if (Directory.Exists(OutputDirectory))
        {
            Directory.Delete(OutputDirectory, true);
        }
    }
}