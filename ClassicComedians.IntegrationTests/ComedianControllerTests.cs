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

        [Fact]
        public async Task Post_EditWillUpdateComedian()
        {
            // Arrange
            Comedian comedian = GetAllComedians().Last();
            Group group = GetAllGroups().Single(g => g.Id == comedian.GroupId);

            string url = $"/Comedian/Edit/{comedian.Id}";
            HttpResponseMessage editResponse = await _client.GetAsync(url);
            IHtmlDocument editDom = await HtmlHelpers.GetDocumentAsync(editResponse);

            IHtmlInputElement firstNameInput = 
                editDom.QuerySelector("#Comedian_FirstName") as IHtmlInputElement;
            Assert.NotNull(firstNameInput);

            IHtmlInputElement lastNameInput = 
                editDom.QuerySelector("#Comedian_LastName") as IHtmlInputElement;
            Assert.NotNull(lastNameInput);

            IHtmlInputElement birthDateInput = 
                editDom.QuerySelector("#Comedian_BirthDate") as IHtmlInputElement;
            Assert.NotNull(birthDateInput);

            IHtmlInputElement deathDateInput = 
                editDom.QuerySelector("#Comedian_DeathDate") as IHtmlInputElement;
            Assert.NotNull(deathDateInput);

            IHtmlSelectElement groupIdSelect =
                editDom.QuerySelector("#Comedian_GroupId") as IHtmlSelectElement;
            Assert.NotNull(groupIdSelect);

            string firstName = firstNameInput.Value;
            string lastName = lastNameInput.Value;
            DateTime birthDate = DateTime.Parse(birthDateInput.Value);
            DateTime deathDate = DateTime.Parse(deathDateInput.Value);
            int groupId = int.Parse(groupIdSelect.Value);

            string newFirstName = firstName + Guid.NewGuid().ToString();
            string newLastName = lastName + Guid.NewGuid().ToString();
            string newBirthDate = birthDate.AddDays(1).ToString("s");
            string newDeathDate = deathDate.AddDays(1).ToString("s");

            Group newGroup = GetAllGroups().First(g => g.Id != groupId);
            string newGroupId = newGroup.Id.ToString();
            string newGroupName = newGroup.Name;

 
            // Act
            HttpResponseMessage response = await _client.SendAsync(
                editDom,
                new Dictionary<string, string> {
                    { "Comedian_FirstName", newFirstName },
                    { "Comedian_LastName", newLastName },
                    { "Comedian_BirthDate", newBirthDate },
                    { "Comedian_DeathDate", newDeathDate },
                    { "Comedian_GroupId", newGroupId },
                }
            );

            // Assert
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                indexDom.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newFirstName));
            Assert.Contains(
                indexDom.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newLastName));
            Assert.Contains(
                indexDom.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newGroupName));
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
