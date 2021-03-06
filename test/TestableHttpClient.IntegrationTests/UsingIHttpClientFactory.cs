﻿#if !NETCOREAPP2_1
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NFluent;

using TestableHttpClient.NFluent;

using Xunit;

namespace TestableHttpClient.IntegrationTests
{
    public class UsingIHttpClientFactory
    {
        [Fact]
        public async Task ConfigureIHttpClientFactoryToUseTestableHttpClient()
        {
            // Create TestableHttpMessageHandler as usual.
            using var testableHttpMessageHandler = new TestableHttpMessageHandler();
            testableHttpMessageHandler.RespondWith(response => response.WithHttpStatusCode(HttpStatusCode.OK));

            var services = new ServiceCollection();
            // Register an HttpClient and configure the TestableHttpMessageHandler as the PrimaryHttpMessageHandler
            services.AddHttpClient(string.Empty).ConfigurePrimaryHttpMessageHandler(() => testableHttpMessageHandler);

            var serviceProvider = services.BuildServiceProvider();

            // Request the IHttpClientFactory
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            // Create the HttpClient
            using var client = httpClientFactory.CreateClient();
            // And use it...
            _ = await client.GetAsync("https://httpbin.com/get");

            // Now use the assertions to make sure the request was actually made.
            testableHttpMessageHandler.ShouldHaveMadeRequestsTo("https://httpbin.com/get");
        }

        [Fact]
        public async Task ConfigureMultipleHttpMessageHandlers()
        {
            // Create multiple TestableHttpMessageHandlers as usual, if the response is not important, a single TestableHttpMessageHandler can be used.
            using var testableGithubHandler = new TestableHttpMessageHandler();
            testableGithubHandler.RespondWith(response => response.WithHttpStatusCode(HttpStatusCode.OK).WithResponseHeader("Server", "github"));

            using var testableHttpBinHandler = new TestableHttpMessageHandler();
            testableHttpBinHandler.RespondWith(response => response.WithHttpStatusCode(HttpStatusCode.NotFound).WithResponseHeader("Server", "httpbin"));

            var services = new ServiceCollection();
            // Register named HttpClients and configure the correct TestableHttpMessageHandler as the PrimaryHttpMessageHandler
            services.AddHttpClient("github").ConfigurePrimaryHttpMessageHandler(() => testableGithubHandler);
            services.AddHttpClient("httpbin").ConfigurePrimaryHttpMessageHandler(() => testableHttpBinHandler);
            var serviceProvider = services.BuildServiceProvider();

            // Request the IHttpClientFactory
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Create the named HttpClient
            using var githubClient = httpClientFactory.CreateClient("github");
            // And use it.
            var result = await githubClient.GetAsync("https://github.com/api/users");
            Check.That(result).HasResponseHeader("Server", "github").And.HasHttpStatusCode(HttpStatusCode.OK);

            // Create another named HttpClient
            using var httpbinClient = httpClientFactory.CreateClient("httpbin");
            // And use it...
            result = await httpbinClient.GetAsync("https://httpbin.com/get");
            Check.That(result).HasResponseHeader("Server", "httpbin").And.HasHttpStatusCode(HttpStatusCode.NotFound);

            // Now assert every TestableHttpMessageHandlers to make sure we made the requests.
            testableGithubHandler.ShouldHaveMadeRequestsTo("https://github.com/*");
            testableHttpBinHandler.ShouldHaveMadeRequestsTo("https://httpbin.com/*");
        }

        [Fact]
        public async Task ConfigureViaConfigureTestServicesOnHostBuilder()
        {
            // Create TestableHttpMessageHandler as usual.
            using var testableHttpMessageHandler = new TestableHttpMessageHandler();
            testableHttpMessageHandler.RespondWith(response => response.WithHttpStatusCode(HttpStatusCode.OK));

            // Setup a TestServer
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer()
                        // Configure the startup class, in this case it already configures a default HttpClient
                        .UseStartup<StartUpWithDefaultHttpHandler>()
                        // Reconfigure the default HttpClient and set the TestableHttpMessageHandler as the primary HttpMessageHandler
                        .ConfigureTestServices(services => services.AddHttpClient(string.Empty).ConfigurePrimaryHttpMessageHandler(() => testableHttpMessageHandler));
                })
                .StartAsync();

            using var client = host.GetTestClient();
            // Make a request to the testserver
            _ = await client.GetAsync("/");

            // Assert that the code in the test server made the expected request.
            testableHttpMessageHandler.ShouldHaveMadeRequestsTo("https://httpbin.com/get");
        }

        private class StartUpWithDefaultHttpHandler
        {
            public static void ConfigureServices(IServiceCollection services)
            {
                services.AddHttpClient();
            }

            public static void Configure(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
                    var httpClient = httpClientFactory.CreateClient();
                    await httpClient.GetAsync("https://httpbin.com/get");
                    await context.Response.WriteAsync("Hello");
                });
            }
        }
    }
}
#endif
