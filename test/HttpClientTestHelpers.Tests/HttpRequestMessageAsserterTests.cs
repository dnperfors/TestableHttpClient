﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;

using Xunit;

namespace HttpClientTestHelpers.Tests
{
    public class HttpRequestMessageAsserterTests
    {
        [Fact]
        public void Constructor_NullRequestList_ThrowsArgumentNullException()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>(() => new HttpRequestMessageAsserter(null));
#nullable restore
        }

#nullable disable
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithUriPattern_NullOrEmptyPattern_ThrowsArgumentNullException(string pattern)
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<ArgumentNullException>(() => sut.WithUriPattern(pattern));

            Assert.Equal("pattern", exception.ParamName);
        }
#nullable restore

        [Fact]
        public void WithUriPattern_RequestWithMatchingUri_DoesNotThrowException()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage(HttpMethod.Get, new Uri("https://example.com")) });

            sut.WithUriPattern("https://example.com");
        }

        [Fact]
        public void WithUriPattern_RequestWithMatchingUriAndNegationTurnedOn_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage(HttpMethod.Get, new Uri("https://example.com")) }, true);

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithUriPattern("https://example.com"));
            Assert.Equal("Expected no requests to be made with uri pattern 'https://example.com', but one request was made.", exception.Message);
        }

        [Fact]
        public void WithUriPattern_RequestWithNotMatchingUri_ThrowsHttpRequestMessageassertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage(HttpMethod.Get, new Uri("https://example.com")) });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithUriPattern("https://test.org"));
            Assert.Equal("Expected at least one request to be made with uri pattern 'https://test.org', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithUriPattern_RequestWithStarPatternAndNoRequests_ThrowsHttpRequestMessageassertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithUriPattern("*"));
            Assert.Equal("Expected at least one request to be made, but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHttpMethod_NullHttpMethod_ThrowsArgumentNullException()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

#nullable disable
            var exception = Assert.Throws<ArgumentNullException>(() => sut.WithHttpMethod(null));
#nullable restore

            Assert.Equal("httpMethod", exception.ParamName);
        }

        [Fact]
        public void WithHttpMethod_NoRequests_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHttpMethod(HttpMethod.Get));

            Assert.Equal("Expected at least one request to be made with HTTP Method 'GET', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHttpMethod_RequestsWithIncorrectHttpMethod_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage(HttpMethod.Post, new Uri("https://example.com")) });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHttpMethod(HttpMethod.Get));

            Assert.Equal("Expected at least one request to be made with HTTP Method 'GET', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHttpMethod_RequestsWithCorrectMethod_ReturnsHttpRequestMessageAsserter()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage(HttpMethod.Get, new Uri("https://example.com")) });

            var result = sut.WithHttpMethod(HttpMethod.Get);

            Assert.NotNull(result);
            Assert.IsType<HttpRequestMessageAsserter>(result);
        }

        [Fact]
        public void WithHttpVersion_NullHttpVersion_ThrowsArgumentNullException()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

#nullable disable
            var exception = Assert.Throws<ArgumentNullException>(() => sut.WithHttpVersion(null));
#nullable restore

            Assert.Equal("httpVersion", exception.ParamName);
        }

        [Fact]
        public void WithHttpVersion_NoRequests_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHttpVersion(HttpVersion.Version11));

            Assert.Equal("Expected at least one request to be made with HTTP Version '1.1', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHttpVersion_RequestsWithIncorrectHttpVersion_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage { Version = HttpVersion.Version20 } });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHttpVersion(HttpVersion.Version11));

            Assert.Equal("Expected at least one request to be made with HTTP Version '1.1', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHttpVersion_RequestsWithCorrectVersion_ReturnsHttpRequestMessageAsserter()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage { Version = HttpVersion.Version11 } });

            var result = sut.WithHttpVersion(HttpVersion.Version11);

            Assert.NotNull(result);
            Assert.IsType<HttpRequestMessageAsserter>(result);
        }

#nullable disable
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithHeader_NullOrEmptyHeaderName_ThrowsArgumentNullException(string headerName)
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<ArgumentNullException>(() => sut.WithHeader(headerName));

            Assert.Equal("headerName", exception.ParamName);
        }
#nullable restore

        [Fact]
        public void WithHeader_NoRequests_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHeader("api-version"));

            Assert.Equal("Expected at least one request to be made with header 'api-version', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHeader_NoMatchingRequests_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(new[] { new HttpRequestMessage() });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHeader("api-version"));

            Assert.Equal("Expected at least one request to be made with header 'api-version', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHeader_MatchingRequest_ReturnsHttpRequestMessageAsserter()
        {
            var request = new HttpRequestMessage();
            request.Headers.Add("api-version", "1.0");
            var sut = new HttpRequestMessageAsserter(new[] { request });

            var result = sut.WithHeader("api-version");

            Assert.NotNull(result);
            Assert.IsType<HttpRequestMessageAsserter>(result);
        }

#nullable disable
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithHeaderNameAndValue_NullOrEmptyHeaderName_ThrowsArgumentNullException(string headerName)
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<ArgumentNullException>(() => sut.WithHeader(headerName, "someValue"));

            Assert.Equal("headerName", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WithHeaderNameAndValue_NullOrEmptyValue_ThrowsArgumentNullException(string headerValue)
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<ArgumentNullException>(() => sut.WithHeader("someHeader", headerValue));

            Assert.Equal("headerValue", exception.ParamName);
        }
#nullable restore

        [Fact]
        public void WithHeaderNameAndValue_NoRequests_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHeader("someHeader", "someValue"));

            Assert.Equal("Expected at least one request to be made with header 'someHeader' and value 'someValue', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHeaderNameAndValue_RequestWithoutHeaders_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var request = new HttpRequestMessage();
            var sut = new HttpRequestMessageAsserter(new[] { request });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHeader("api-version", "1.0"));

            Assert.Equal("Expected at least one request to be made with header 'api-version' and value '1.0', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHeaderNameAndValue_RequestWithNotMatchingHeaderName_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var request = new HttpRequestMessage();
            request.Headers.Add("no-api-version", "1.0");
            var sut = new HttpRequestMessageAsserter(new[] { request });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHeader("api-version", "1.0"));

            Assert.Equal("Expected at least one request to be made with header 'api-version' and value '1.0', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHeaderNameAndValue_RequestWithNotMatchingHeaderValue_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var request = new HttpRequestMessage();
            request.Headers.Add("api-version", "unknown");
            var sut = new HttpRequestMessageAsserter(new[] { request });

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.WithHeader("api-version", "1.0"));

            Assert.Equal("Expected at least one request to be made with header 'api-version' and value '1.0', but no requests were made.", exception.Message);
        }

        [Fact]
        public void WithHeaderNameAndValue_RequestWithMatchinHeader_ReturnsHttpRequestMessageAssert()
        {
            var request = new HttpRequestMessage();
            request.Headers.Add("api-version", "1.0");
            var sut = new HttpRequestMessageAsserter(new[] { request });

            var result = sut.WithHeader("api-version", "1.0");

            Assert.NotNull(result);
            Assert.IsType<HttpRequestMessageAsserter>(result);
        }

        [Fact]
        public void Times_ValueLessThan0_ThrowsArgumentException()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<ArgumentException>(() => sut.Times(-1));

            Assert.Equal("count", exception.ParamName);
        }

        [Fact]
        public void Times_NoRequestsAndCount1_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.Times(1));

            Assert.Equal("Expected one request to be made, but no requests were made.", exception.Message);
        }

        [Fact]
        public void Times_NoRequestsAndCount2_ThrowsHttpRequestMessageAssertionExceptionWithSpecificMessage()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var exception = Assert.Throws<HttpRequestMessageAssertionException>(() => sut.Times(2));

            Assert.Equal("Expected 2 requests to be made, but no requests were made.", exception.Message);
        }

        [Fact]
        public void Times_NoRequestsAndCount0_ReturnsHttpRequestMessageAsserter()
        {
            var sut = new HttpRequestMessageAsserter(Enumerable.Empty<HttpRequestMessage>());

            var result = sut.Times(0);
            Assert.NotNull(result);
            Assert.IsType<HttpRequestMessageAsserter>(result);
        }
    }
}
