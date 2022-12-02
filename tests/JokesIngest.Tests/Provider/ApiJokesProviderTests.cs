using JokesIngest.Model;
using JokesIngest.Provider;
using JokesIngest.Tests.Utils;
using Machine.Fakes;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

namespace JokesIngest.Tests.Provider;

[Subject(typeof(ApiJokesProvider))]
public class ApiJokesProviderTests : WithSubject<ApiJokesProvider>
{
    const int BatchSize = 2;

    #region SetupMethods

    static HttpClient SetupClient(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        
        return client;
    }

    static void SetupProvider(HttpMessageHandler messageHandlerMock)
    {
        Configure(x => x.For<HttpClient>()
            .Use(SetupClient(messageHandlerMock)));

        Configure(x => x.For<JokesProviderConfiguration>()
            .Use(new JokesProviderConfiguration(BatchSize, "jokes/random")));
    }

    #endregion

    class When_all_jokes_results_success
    {
        static IEnumerable<Joke> jokes;

        Establish ctx = () =>
            SetupProvider(HttpMessageHandlerMock.SetupSuccessResultsForJokes(new[]
            {
                New.Joke(value: "A joke"),
                New.Joke(value: "Another joke"),
                New.Joke(value: "Next Chuck joke"),
            }).Object);

        Because of = () => jokes = Subject.GetJokesAsync().ToEnumerable().ToArray();

        It should_get_results_successfully = () => jokes.ShouldEqual(new[]
        {
            New.Joke(value: "A joke"),
            New.Joke(value: "Another joke"),
        });

        It should_get_correct_batch_size = () => jokes.Count().ShouldEqual(BatchSize);
    }

    class When_failure
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => Subject.GetJokesAsync().ToEnumerable().ToArray());

        class When_first_joke_result_s_failure
        {
            Establish ctx = () => SetupProvider(HttpMessageHandlerMock.SetupFailureResult().Object);

            It should_throw_http_request_exception =
                () => exception.ShouldBeOfExactType<HttpRequestException>();
        }

        class When_middle_joke_result_is_failure
        {
            Establish ctx = () =>
                SetupProvider(HttpMessageHandlerMock.SetupSuccessResultsForJokesWithErrorInTheMiddle(new[]
                {
                    New.Joke(value: "A joke"),
                    New.Joke(value: "Another joke"),
                    New.Joke(value: "Next Chuck joke"),
                }).Object);

            It should_throw_http_request_exception =
                () => exception.ShouldBeOfExactType<HttpRequestException>();
        }
    }
}