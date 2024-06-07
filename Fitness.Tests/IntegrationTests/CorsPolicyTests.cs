using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net;

namespace Fitness.Tests.CORSTests
{
    public class CorsPolicyTests : WorkoutTests
    {
        [Theory]
        [InlineData("http://localhost:3000", "AllowLocalHost")]
        public async Task TestCorsPolicy(string origin, string policyName)
        {          
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/workout")
            {
                Headers = { { "Origin", origin }, { "Access-Control-Request-Method", "GET" } }
            };
           
            var response = await httpClient.SendAsync(request);
        
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"), "CORS header missing: Access-Control-Allow-Origin");
            Assert.Equal(origin, response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault());
            Assert.True(response.Headers.Contains("Access-Control-Allow-Methods"), "CORS header missing: Access-Control-Allow-Methods");
            Assert.Contains("GET", response.Headers.GetValues("Access-Control-Allow-Methods").FirstOrDefault());
        }
    }
}
