namespace AiDevs2Reloaded.Api.Exceptions;

public class MissingTranscriptionException : Exception
{
    public MissingTranscriptionException() : base("Failed to get transcription")
    {
    }
}