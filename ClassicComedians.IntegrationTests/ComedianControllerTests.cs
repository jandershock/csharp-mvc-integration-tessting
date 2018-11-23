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
    /// <summary>
    ///  Integration tests for the ComedianController
    /// </summary>
    public class ComedianControllerTests :
        IClassFixture<WebApplicationFactory<ClassicComedians.Startup>>
    {
        private readonly HttpClient _client;

        public ComedianControllerTests(WebApplicationFactory<ClassicComedians.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        ///  Makes a GET request to Comedian Index URL
        ///  verifies that the response contains a table with all the
        ///   comedians from the database.
        /// </summary>
        [Fact]
        public async Task Get_IndexReturnsTableOfComedians()
        {
            // Arrange
            string url = "/Comedian";

            // Get all the comedians from the database,
            //  and extract the full name from each comedian.
            // We'll use this collection of full names later in the test.
            IEnumerable<string> comedianFullNames = 
                Database.GetAllComedians()
                        .Select(c => $"{c.FirstName} {c.LastName}");

            // Act
            //  Make a GET request to the comedian's index page and save the response in a variable.
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            //  Convert the response into a DOM-like object.
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);

            // Use a query selector to get the first cell (a.k.a. "<td>") in each row of the HTML table.
            //  The resulting collection will be the first column of the table.
            IEnumerable<IElement> firstColumn =
                indexDom.QuerySelectorAll("table tbody tr td:first-child");

            // Verify that the number of records in the HTML table match the
            //  number of records returned form the database.
            Assert.Equal(
                comedianFullNames.Count(),
                firstColumn.Count()
            );

            // Verify that the text in each cell of the first column
            //  is a valid comedian full name from the database
            foreach (IElement td in firstColumn)
            {
                Assert.Contains(
                    comedianFullNames,
                    fullName => td.TextContent.Contains(fullName)
                );
            }
        }

        /// <summary>
        ///  Makes a POST request containing a new Comedian to the Comedian's Create URL,
        ///   and verifies that the newly created comedian appears in the index page.
        /// </summary>
        [Fact]
        public async Task Post_CreateMakesNewComeidan()
        {
            // Arrange

            // Get a group from the database
            //  This will be the new comedian's group.
            Group group = Database.GetAllGroups().First();

            // Create variables containing the new comedian's data.
            // NOTE: all of the variables MUST be strings

            // We use Guids in order to ensure that the first and last names will be
            //  unique in the database. A Guid is a "globally unique identifier".
            string firstName = "firstname-" + Guid.NewGuid().ToString();
            string lastName = "lastname-" + Guid.NewGuid().ToString();

            // For dates we use the .ToString("s") to create "sortable" DateTime strings.
            //  This is the format that the AngleSharp library expects and may be different
            //  from the format you use when manually filling out the form.
            //  The DateTime string will be something like: "1918-11-23T00:00:00"
            string birthDate = DateTime.Today.AddYears(-100).ToString("s");
            string deathDate = DateTime.Today.AddYears(-20).ToString("s");

            // String representation of the group's id.
            string groupId = group.Id.ToString();
            // We'll use the group name in our assertions below.
            string groupName = group.Name;

            // Make a GET request to get the page that contains the Create form.
            //  We must have the create form in order to make the POST request later.
            string url = "/Comedian/Create";
            HttpResponseMessage createResponse = await _client.GetAsync(url);
            IHtmlDocument createDom = await HtmlHelpers.GetDocumentAsync(createResponse);


            // Act

            // Make the POST request providing the create form and the values for the form inputs.
            //  The values for the form inputs are passed in as a Dictionary whose keys and values
            //   are strings.
            //  The keys of the dictionary are the HTML IDs of the input. Use your browser's 
            //   developer tools to find the ID for each input.
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

            // The response object above should be the comedian's index page.

            // Assert
            // Convert the repose to the index page DOM-like object.
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);

            // Verify that there is a <td> on the page that contains the data for the
            //  newly inserted comedian.
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

        /// <summary>
        ///  Makes a POST request to update an existing comedian,
        ///   and verifies that the updated comedian appears in the index page.
        /// </summary>
        [Fact]
        public async Task Post_EditWillUpdateComedian()
        {
            // Arrange

            // Find a comedian (along with their group) to edit.
            Comedian comedian = Database.GetAllComedians().Last();
            Group group = Database.GetAllGroups().Single(g => g.Id == comedian.GroupId);

            // Make a GET request to retrieve the Edit form.
            //  NOTE: we have to specify the comedian's id on the url.
            string url = $"/Comedian/Edit/{comedian.Id}";
            HttpResponseMessage editResponse = await _client.GetAsync(url);
            IHtmlDocument editDom = await HtmlHelpers.GetDocumentAsync(editResponse);


            // Pull the existing comedian's information out of the form.

            // We use the HTML ID to ge the input element.
            // NOTE: Since QuerySeelctor() returns a generic IElement and we need an
            //  IHtmlInputElement, we must convert (or "cast") to IHtmlInputElement.
            IHtmlInputElement firstNameInput = 
                editDom.QuerySelector("#Comedian_FirstName") as IHtmlInputElement;
            // Make sure we got the input we were looking for.
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

            // For the comedian's group, we get an IHtmlSelectElement instead of an IHtmlInputElement.
            IHtmlSelectElement groupIdSelect =
                editDom.QuerySelector("#Comedian_GroupId") as IHtmlSelectElement;
            Assert.NotNull(groupIdSelect);

            // Get the comedian's data from the form input fields.
            string firstName = firstNameInput.Value;
            string lastName = lastNameInput.Value;
            DateTime birthDate = DateTime.Parse(birthDateInput.Value);
            DateTime deathDate = DateTime.Parse(deathDateInput.Value);
            int groupId = int.Parse(groupIdSelect.Value);

            // Update the original data so we can submit new data to the Edit form.
            //  NOTE: All values MUST be strings.
            string newFirstName = firstName + Guid.NewGuid().ToString();
            string newLastName = lastName + Guid.NewGuid().ToString();
            string newBirthDate = birthDate.AddDays(1).ToString("s");
            string newDeathDate = deathDate.AddDays(1).ToString("s");

            Group newGroup = Database.GetAllGroups().First(g => g.Id != groupId);
            string newGroupId = newGroup.Id.ToString();
            string newGroupName = newGroup.Name;

 
            // Act
            // Make the POST request providing the edit form and the values for the form inputs.
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
            // Convert the repose to the index page DOM-like object.
            IHtmlDocument indexDom = await HtmlHelpers.GetDocumentAsync(response);

            // Verify that there is a <td> on the page that contains the data for the
            //  newly updated comedian.
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

    }
}
