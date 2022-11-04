namespace NovelAIPromptParser.Entity;

public class ParseResult
{
    /// <summary>
    /// Prompt string
    /// </summary>
    public string Prompt { get; set; } = string.Empty;
    
    /// <summary>
    /// Image height
    /// </summary>
    public int Height { get; set; } = 0;
    
    /// <summary>
    /// Image width
    /// </summary>
    public int Width { get; set; } = 0;
    
    /// <summary>
    /// Image seed
    /// </summary>
    public string Seed { get; set; } = string.Empty;

    /// <summary>
    /// Steps
    /// </summary>
    public decimal Steps { get; set; } = 0;
    
    /// <summary>
    /// Sampler
    /// </summary>
    public string Sampler { get; set; } = string.Empty;
    
    /// <summary>
    /// Strength
    /// </summary>
    public decimal Strength { get; set; } = 0;
    
    /// <summary>
    /// Noise
    /// </summary>
    public decimal Noise { get; set; } = 0;
    
    /// <summary>
    /// Scale
    /// </summary>
    public decimal Scale { get; set; } = 0;
    
    /// <summary>
    /// Undesired content
    /// </summary>
    public string UndesiredContent { get; set; } = string.Empty;
    
}