using Newtonsoft.Json;
using NovelAIPromptParser.Entity;
using SixLabors.ImageSharp;

namespace NovelAIPromptParser;

public static class Parser
{
    /// <summary>
    /// Quality tags
    /// </summary>
    /// <remarks>
    /// Tags assigned when "Add Quality Tags" is checked.
    /// </remarks>
    public static readonly string[] QualityTags = { "masterpiece", "best quality" };
    
    /// <summary>
    /// Parse image from file.
    /// </summary>
    /// <param name="path">File Path</param>
    /// <returns></returns>
    /// <exception cref="ParserException"></exception>
    public static ParseResult ParseImage(string path)
    {
        try
        {
            var image = Image.Load(path);
            return GetMetaData(image);
        }
        catch (UnknownImageFormatException e)
        {
            throw new ParserException("Not supported image.", e);
        }
    }

    /// <summary>
    /// Parse image from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="ParserException"></exception>
    public static ParseResult ParseImage(Stream stream)
    {
        try
        {
            var image = Image.Load(stream);
            return GetMetaData(image);
        }
        catch (UnknownImageFormatException e)
        {
            throw new ParserException("Not supported image.", e);
        }
        
    }
    
    /// <summary>
    /// Get NovelAI settings from image
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    /// <exception cref="ParserException"></exception>
    private static ParseResult GetMetaData(Image image)
    {
        try
        {
            var meta = image.Metadata;
            var pngMeta = meta.GetPngMetadata();
            if (!pngMeta.TextData.Any()) throw new ParserException("Metadata is not found.");
            
            // description contains prompt
            var description = pngMeta.TextData.FirstOrDefault(m => m.Keyword == "Description");
            
            // comment contains model-specific settings
            var comment = pngMeta.TextData.FirstOrDefault(m => m.Keyword == "Comment");
            Dictionary<string, string>? commentDic = null;
            if (comment.Value != null) commentDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(comment.Value);

            var result = new ParseResult()
            {
                Prompt = description.Value,
                Height = image.Height,
                Width = image.Width,
            };
            
            // split prompt
            result.Tags = DeserializePrompt(result.Prompt);

            if (result.Tags.Any(t => t.Word == QualityTags[0]) && result.Tags.Any(t => t.Word == QualityTags[1]))
            {
                result.ExistQualityTags = true;
            }
            
            if (commentDic == null) return result;
            
            result.Seed = commentDic["seed"];
            if (decimal.TryParse(commentDic["steps"], out var steps)) result.Steps = steps;
            result.Sampler = commentDic["sampler"];
            if (decimal.TryParse(commentDic["strength"], out var strength)) result.Strength = strength;
            if (decimal.TryParse(commentDic["noise"], out var noise)) result.Noise = noise;
            if (decimal.TryParse(commentDic["scale"], out var scale)) result.Scale = scale;
            result.UndesiredContent = commentDic["uc"];
            
            // split undesired content
            result.NegativeTags = DeserializePrompt(result.UndesiredContent);

            return result;
        }
        catch (ParserException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ParserException("An error has occurred.", e);
        }
        
    }

    /// <summary>
    /// Deserialize prompt string
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns>List of <see cref="Tag"/></returns>
    public static List<Tag> DeserializePrompt(string prompt)
    {
        return prompt.Split(',').Select(t => new Tag()
        {
            Raw = t.Trim(),
            Word = t.Trim().Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", ""),
            Strength = t.Contains('{') ? t.Count(c => c == '{') : t.Count(c => c == '[') * -1,
        }).ToList();

    }

    /// <summary>
    /// Serialize prompt string
    /// </summary>
    /// <param name="tags">List of <see cref="Tag"/></param>
    /// <returns>Prompt string</returns>
    public static string SerializePrompt(List<Tag> tags)
    {
        var prompt = string.Empty;
        foreach (var tag in tags)
        {
            var brackets = tag.Strength >= 0 ? new[] { '{', '}' } : new[] { '[', ']' };
            prompt +=
                $"{new string(brackets[0], Math.Abs(tag.Strength))}{tag.Word}{new string(brackets[1], Math.Abs(tag.Strength))}, ";
        }

        return prompt;
    }
}