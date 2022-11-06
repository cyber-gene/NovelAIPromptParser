namespace NovelAIPromptParser.Entity;

public class ParseResult
{
    /// <summary>
    /// Prompt string
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// List of Tag
    /// </summary>
    /// <remarks>
    /// <see cref="Prompt"/> split by comma.
    /// </remarks>
    public List<Tag> Tags { get; set; } = new();
    
    /// <summary>
    /// Prompt exists Quality tags
    /// </summary>
    /// <remarks>
    /// true when "Add Quality Tags" checked.
    /// </remarks>
    public bool ExistQualityTags { get; set; }

    /// <summary>
    /// Image height
    /// </summary>
    public int Height { get; set; }
    
    /// <summary>
    /// Image width
    /// </summary>
    public int Width { get; set; }
    
    /// <summary>
    /// Image seed
    /// </summary>
    public string Seed { get; set; } = string.Empty;

    /// <summary>
    /// Steps
    /// </summary>
    public decimal Steps { get; set; }
    
    /// <summary>
    /// Sampler
    /// </summary>
    public string Sampler { get; set; } = string.Empty;
    
    /// <summary>
    /// Strength
    /// </summary>
    public decimal Strength { get; set; }
    
    /// <summary>
    /// Noise
    /// </summary>
    public decimal Noise { get; set; }
    
    /// <summary>
    /// Scale
    /// </summary>
    public decimal Scale { get; set; }
    
    /// <summary>
    /// Undesired content
    /// </summary>
    public string UndesiredContent { get; set; } = string.Empty;

    /// <summary>
    /// NegativeTags
    /// </summary>
    /// <remarks>
    /// <see cref="UndesiredContent"/> split by comma.
    /// </remarks>
    public List<Tag> NegativeTags { get; set; } = new();

}