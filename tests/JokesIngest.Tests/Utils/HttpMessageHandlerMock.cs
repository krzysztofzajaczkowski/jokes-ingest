using System.Linq.Expressions;
using JokesIngest.Model;
using Moq;
using Newtonsoft.Json;
using System.Net;
using Moq.Protected;

namespace JokesIngest.Tests.Utils;

internal static class HttpMessageHandlerMock
{
    private interface IHttpMessageHandlerProtectedMembers
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }

    private static IProtectedAsMock<HttpMessageHandler, IHttpMessageHandlerProtectedMembers>
        AsProtected(this Mock<HttpMessageHandler> mock) => mock.Protected().As<IHttpMessageHandlerProtectedMembers>();

    private static Expression<Func<IHttpMessageHandlerProtectedMembers, Task<HttpResponseMessage>>>
        SendAsyncExpression => x =>
            x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>());

    private static HttpResponseMessage CreateHttpResponseMessage(HttpStatusCode code = HttpStatusCode.OK, Joke joke = null)
    {
        return new HttpResponseMessage
        {
            StatusCode = code,
            Content = (joke == null ? default : new StringContent(JsonConvert.SerializeObject(joke)))!
        };
    }

    public static Mock<HttpMessageHandler> SetupFailureResult()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler.AsProtected()
            .Setup(SendAsyncExpression)
            .ReturnsAsync(CreateHttpResponseMessage(HttpStatusCode.BadGateway));

        return mockMessageHandler;
    }

    public static Mock<HttpMessageHandler> SetupSuccessResultsForJokes(IEnumerable<Joke> jokes)
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var sequenceSetup = mockMessageHandler.AsProtected()
            .SetupSequence(SendAsyncExpression);
        foreach (var joke in jokes)
        {
            sequenceSetup.ReturnsAsync(CreateHttpResponseMessage(joke: joke));
        }

        return mockMessageHandler;
    }

    public static Mock<HttpMessageHandler> SetupSuccessResultsForJokesWithErrorInTheMiddle(IEnumerable<Joke> jokes)
    {
        var results = jokes.Select(joke => CreateHttpResponseMessage(joke: joke)).ToList();
        results.Insert(results.Count / 2, CreateHttpResponseMessage(HttpStatusCode.BadGateway));

        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var sequenceSetup = mockMessageHandler.AsProtected()
            .SetupSequence(SendAsyncExpression);
        foreach (var result in results)
        {
            sequenceSetup.ReturnsAsync(result);
        }

        return mockMessageHandler;
    }
}