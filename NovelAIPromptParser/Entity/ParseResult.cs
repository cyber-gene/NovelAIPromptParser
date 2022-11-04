namespace NovelAIPromptParser.Entity;

public class Prompt
{
    /// <summary>
    /// Prompt string
    /// </summary>
    public string Raw { get; set; } = string.Empty;

    /// <summary>
    /// List of Tag
    /// </summary>
    public List<Tag> Tags { get; set; } = new List<Tag>();

    public List<Tag> NegativeTags { get; set; } = new List<Tag>();
}