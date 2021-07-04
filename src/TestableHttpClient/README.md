# TestableHttpClient

Creating unittest for code that uses `HttpClient` can be difficult to test. It requires a custom HttpMessageHandler or a mocked version. TestableHttpClient provides a testable version of HttpMessageHandler and several helper functions to configure the `TestableHttpHandler` and several ways to assert which requests were made.

When you use [NFluent](https://n-fluent.net/) for asserting, then you would be interested in the package [TestableHttpClient.NFluent](https://www.nuget.org/packages/TestableHttpClient.NFluent/).

## Howto install

TestableHttpClient is released as a NuGet packages and can be installed via the NuGet manager in VisualStudio or by running the following command on the command line:
```
dotnet add package TestableHttpClient
```

## How to use

```csharp
var testHandler = new TestableHttpMessageHandler();
var httpClient = new HttpClient(testHandler); // or testHandler.CreateClient()

var result = await httpClient.GetAsync("http://httpbin.org/status/200");

testHandler.ShouldHaveMadeRequestsTo("https://httpbin.org/*");
```

More examples can be found in the [IntegrationTests project](https://github.com/dnperfors/TestableHttpClient/tree/master/test/TestableHttpClient.IntegrationTests)

## Authors

* **David Perfors** - [dnperfors](https://github.com/dnperfors)

See also the list of [contributors](https://github.com/dnperfors/TestableHttpClient/contributors) who participated in this project.

## License

This project is released under the MIT license, see [LICENSE.md](https://github.com/dnperfors/TestableHttpClient/blob/master/LICENSE.md) for more information.

## Acknowledgments

This library is largely inspired by the HttpTest functionality from [Flurl](https://flurl.dev).  
A lot of the ideas came from the thread about unit testing HttpClient code in [this dotnet issue](https://github.com/dotnet/runtime/issues/14535).
