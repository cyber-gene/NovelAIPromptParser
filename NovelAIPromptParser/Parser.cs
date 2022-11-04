using Newtonsoft.Json;
using SixLabors.ImageSharp;
using NovelAIPromptParser.Entity;

namespace NovelAIPromptParser;

public static class Parser
{
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
    private static ParseResult GetMetaData(IImageInfo image)
    {
        try
        {
            var meta = image.Metadata;
            if (meta == null) throw new ParserException("Metadata is not found.");
            
            var pngMeta = meta.GetPngMetadata();
            if (pngMeta == null) throw new ParserException("Metadata is not found.");
            if (!pngMeta.TextData.Any()) throw new ParserException("Metadata is not found.");
            
            // description contains prompt
            var description = pngMeta.TextData.FirstOrDefault(m => m.Keyword == "Description");
            
            // comment contains model-specific settings
            var comment = pngMeta.TextData.FirstOrDefault(m => m.Keyword == "Comment");
            var commentDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(comment.Value);

            var result = new ParseResult()
            {
                Prompt = description.Value,
                Height = image.Height,
                Width = image.Width,
            };

            if (commentDic == null) return result;
            
            result.Seed = commentDic["seed"];
            if (decimal.TryParse(commentDic["steps"], out var steps)) result.Steps = steps;
            result.Sampler = commentDic["sampler"];
            if (decimal.TryParse(commentDic["strength"], out var strength)) result.Strength = strength;
            if (decimal.TryParse(commentDic["noise"], out var noise)) result.Noise = noise;
            if (decimal.TryParse(commentDic["scale"], out var scale)) result.Scale = scale;
            result.UndesiredContent = commentDic["uc"];

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
}