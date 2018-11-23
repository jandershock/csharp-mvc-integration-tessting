using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using ClassicComedians.Data;
using ClassicComedians.IntegrationTests.Helpers;
using ClassicComedians.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace ClassicComedians.IntegrationTests
{
    public class ComedianControllerTests :
        IClassFixture<WebApplicationFactory<ClassicComedians.Startup>>
    {
        private readonly HttpClient _client;

        public ComedianControllerTests(WebApplicationFactory<ClassicComedians.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsTableOfComedians()
        {
            // Arrange
            string url = "/Comedian";
            IEnumerable<string> comedianFullNames = 
                GetAllComedians().Select(c => $"{c.FirstName} {c.LastName}");

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);
            IEnumerable<IElement> firstColumn =
                indexDom.QuerySelectorAll("table tbody tr td:first-child");

            Assert.Equal(
                comedianFullNames.Count(),
                firstColumn.Count()
            );

            foreach (IElement td in firstColumn)
            {
                Assert.Contains(
                    comedianFullNames,
                    fullName => td.TextContent.Contains(fullName)
                );
            }
        }

        [Fact]
        public async Task Post_CreateMakesNewComeidan()
        {
            // Arrange
            Group group = GetAllGroups().First();

            string firstName = "firstname-" + Guid.NewGuid().ToString();
            string lastName = "lastname-" + Guid.NewGuid().ToString();
            string birthDate = DateTime.Today.AddYears(-100).ToString("s");
            string deathDate = DateTime.Today.AddYears(-20).ToString("s");
            string groupId = group.Id.ToString();
            string groupName = group.Name;

            string url = "/Comedian/Create";
            HttpResponseMessage createResponse = await _client.GetAsync(url);
            IHtmlDocument createDom = await HtmlHelpers.GetDocumentAsync(createResponse);


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createDom,
                new Dictionary<string, string> {
                    { "Comedian_FirstName", firstName },
                    { "Comedian_LastName", lastName },
                    { "Comedian_BirthDate", birthDate },
                    { "Comedian_DeathDate", deathDate },
                    { "Comedian_GroupId", groupId },
                }
            );

            // Assert
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                indexDom.QuerySelectorAll("td"),
                td => td.TextContent.Contains(firstName));
            Assert.Contains(
                indexDom.QuerySelectorAll("td"),
                td => td.TextContent.Contains(lastName));
            Assert.Contains(
                indexDom.QuerySelectorAll("td"),
                td => td.TextContent.Contains(groupName));
        }

        private IEnumerable<Comedian> GetAllComedians()
        {
            return Database.GetAllComedians();
        }

        private IEnumerable<Group> GetAllGroups()
        {
            return Database.GetAllGroups();
        }
    }
}
