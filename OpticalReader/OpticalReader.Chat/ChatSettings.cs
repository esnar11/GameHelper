namespace OpticalReader.Chat
{
    public class ChatSettings
    {
        public string[] HighlightWords { get; set; } =
        {
            "Tank"
        };

        public string[] IgnoreWords { get; set; } =
        {
            "has+reached", "Ukrain", "A6OP", "ADEKBAT", "lfg+tank", "lfg+DD"
        };
    }
}
