﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace HttpClientTestHelpers.Tests
{
    public class TestableHttpMessageHandlerTests
    {
        [Fact]
        public async Task SendAsync_WhenRequestsAreMade_LogsRequests()
        {
            using var sut = new TestableHttpMessageHandler();
            using var client = new HttpClient(sut);
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

            _ = await client.SendAsync(request);

            Assert.Contains(request, sut.Requests);
        }

        [Fact]
        public async Task SendAsync_WhenMultipleRequestsAreMade_AllRequestsAreLogged()
        {
            using var sut = new TestableHttpMessageHandler();
            using var client = new HttpClient(sut);
            using var request1 = new HttpRequestMessage(HttpMethod.Get, "https://example1.com");
            using var request2 = new HttpRequestMessage(HttpMethod.Post, "https://example2.com");
            using var request3 = new HttpRequestMessage(HttpMethod.Delete, "https://example3.com");
            using var request4 = new HttpRequestMessage(HttpMethod.Head, "https://example4.com");

            _ = await client.SendAsync(request1);
            _ = await client.SendAsync(request2);
            _ = await client.SendAsync(request3);
            _ = await client.SendAsync(request4);

            Assert.Equal(new[] { request1, request2, request3, request4 }, sut.Requests);
        }

        [Fact]
        public async Task SendAsync_ByDefault_ReturnsHttpStatusCodeOK()
        {
            using var sut = new TestableHttpMessageHandler();
            using var client = new HttpClient(sut);

            var result = await client.GetAsync(new Uri("https://example.com"));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task SendAsync_WhenRespondWithIsSet_SetRespondIsUsed()
        {
            using var sut = new TestableHttpMessageHandler();
            using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            sut.RespondWith(response);
            using var client = new HttpClient(sut);

            var result = await client.GetAsync(new Uri("https://example.com"));

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Same(response, result);
        }

#nullable disable
        [Fact]
        public void RespondWith_NullValue_ThrowsArgumentNullException()
        {
            using var sut = new TestableHttpMessageHandler();
            var exception = Assert.Throws<ArgumentNullException>(() => sut.RespondWith(null));
            Assert.Equal("httpResponseMessage", exception.ParamName);
        }
#nullable restore
    }
}
