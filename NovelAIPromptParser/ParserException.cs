using System.Runtime.Serialization;
using NovelAIPromptParser.Entity;

namespace NovelAIPromptParser;

public class ParserException : Exception
{
    public ParserException() : base()
    {
    }

    public ParserException(string message) : base(message)
    {
    }

    public ParserException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}