namespace NovelAIPromptParser.Entity;

public class Tag
{
    /// <summary>
    /// Raw tag
    /// </summary>
    /// <remarks>
    /// Include {} and []
    /// </remarks>
    public string Raw { get; set; }
    
    /// <summary>
    /// Tag word
    /// </summary>
    public string Word { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of {}, or [] if negative value
    /// </summary>
    public int Strength { get; set; }
}