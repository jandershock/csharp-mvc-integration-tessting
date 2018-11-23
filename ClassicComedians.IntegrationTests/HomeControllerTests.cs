using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using ClassicComedians.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClassicComedians.IntegrationTests
{
    public class HomeControllerTests :
        IClassFixture<WebApplicationFactory<ClassicComedians.Startup>>
    {
        private readonly HttpClient _client;

        public HomeControllerTests(WebApplicationFactory<ClassicComedians.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessResponse()
        {
            // Arrange
            string url = "/";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_IndexContainsTwoLinksToComediansList()
        {
            // Arrange
            string url = "/";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);
            string comedianListUrl = "/Comedian";
            int expectedComedianListUrlCount = 2;

            IEnumerable<IElement> comedianListUrlAnchorTags =
                indexDom.QuerySelectorAll($"a[href='{comedianListUrl}']");

            Assert.Equal(
                expectedComedianListUrlCount, 
                comedianListUrlAnchorTags.Count());
        }
    }
}
