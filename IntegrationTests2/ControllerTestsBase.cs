using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Net.Http;
using Xunit;

namespace IntegrationTests2
{
    public class ControllerTestsBase : IClassFixture<JwtAuth>
    {
        protected readonly JwtAuth factory;
        protected HttpClient client;
        protected dynamic token;

        public ControllerTestsBase(JwtAuth factory)
        {
            this.factory = factory;
            client = factory.CreateClient();

            token = new ExpandoObject();
            token.sub = Guid.NewGuid();
            token.role = new[] { "sub_role", "Admin" };
        }
    }
}
