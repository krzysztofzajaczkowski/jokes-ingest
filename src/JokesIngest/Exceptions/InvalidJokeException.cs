using System;

namespace JokesIngest.Exceptions
{
    public class InvalidJokeException : Exception
    {
        public InvalidJokeException() : base("Joke text is null or whitespace.") { }
    }
}