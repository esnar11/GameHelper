namespace OpticalReader.Chat
{
    public class ChatSettings
    {
        public string[] HighlightWords { get; } =
        {
            "Tank"
        };

        public string[] IgnoreWords { get; } =
        {
            "has reached", "Ukrain"
        };
    }
}
