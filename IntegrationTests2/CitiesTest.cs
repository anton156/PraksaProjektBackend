using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests2
{
    public class CitiesTest : ControllerTestsBase
    {
        public CitiesTest(JwtAuth factory) : base(factory) { }

        [Fact]
        public async Task should_return_uauthorized()
        {
            var response = await client.GetAsync("/api/Cities");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
