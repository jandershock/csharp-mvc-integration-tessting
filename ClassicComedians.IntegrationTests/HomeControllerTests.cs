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
    /// <summary>
    ///  Integration tests for the HomeController
    /// </summary>
    public class HomeControllerTests :
        IClassFixture<WebApplicationFactory<ClassicComedians.Startup>>
    {
        // We use an HttpClient to make GET and POST requests to the application.
        private readonly HttpClient _client;

        /// <summary>
        ///  Make a new instance of HomeControllerTests.
        /// </summary>
        /// <param name="factory">
        ///  A helpful object provided by the Microsoft MVC testing library.
        /// </param>
        public HomeControllerTests(WebApplicationFactory<ClassicComedians.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        ///  Makes a GET request to root URL of the app and 
        ///  verifies that the response is successful.
        /// </summary>
        [Fact]
        public async Task Get_IndexReturnsSuccessResponse()
        {
            // Arrange
            //  Do whatever setup you need to do in order to run the test.
            //  This normally includes creating variables for use later in the test.
            //  In this test we make a new variable to hold the application's root URL.
            string url = "/";


            // Act
            //  Execute the code that you wish to test.
            //  In this test we make a request to the application's root URL and get the response.
            HttpResponseMessage response = await _client.GetAsync(url);


            // Assert
            //  Verify that the code executed in the "Act" step worked properly
            //  In this test we confirm that the response returns a status code of 200 - 299 and
            //   that the response's type is HTML.
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        ///  Makes a GET request to root URL of the application and 
        ///  verifies that the response contains a table containing all the
        ///  comedians from the database.
        /// </summary>
        [Fact]
        public async Task Get_IndexContainsLinksToComedianIndex()
        {
            // Arrange
            
            // The root URL
            string url = "/"; 

            // These will be used to assert that the index page contains the correct number of links
            //  to the Comedian index page
            string comedianIndexUrl = "/Comedian";
            int expectedComedianIndexLinkCount = 2;


            // Act
            HttpResponseMessage response = await _client.GetAsync(url);


            // Assert

            // Turn the response into a DOM-like object so we can query it with
            //  QuerySelector() and QuerySelectorAll() methods
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);

            // Find all the anchor tags (e.g. "links") that point to the comedian index page.
            IEnumerable<IElement> comedianIndexUrlAnchorTags =
                indexDom.QuerySelectorAll($"a[href='{comedianIndexUrl}']");

            // Verify that we've found the expected number of links on the page.
            Assert.Equal(
                expectedComedianIndexLinkCount, 
                comedianIndexUrlAnchorTags.Count());
        }
    }
}
