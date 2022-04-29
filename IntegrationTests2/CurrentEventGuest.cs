using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace IntegrationTests2
{
    public class CurrentEventGuest
    {
        [Fact]
        public async Task GET_retrieves_currentevents()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            var client = webApplicationFactory.CreateClient();

            var response = await client.GetAsync("/api/CurrentEvents/getallcurrentevents");

            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task GET_retrieves_posts()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            var client = webApplicationFactory.CreateClient();

            var response = await client.GetAsync("/api/Posts/getallposts");

            response.EnsureSuccessStatusCode();
        }
    }
}