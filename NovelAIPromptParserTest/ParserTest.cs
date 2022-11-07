namespace NovelAIPromptParserTest;

/// <summary>
/// Testing <see cref="Parser"/>
/// </summary>
[TestClass]
public class ParserTest
{
    /// <summary>
    /// Success pattern image file path
    /// </summary>
    private const string NormalPngPath = @"TestData/Normal.png";
    
    /// <summary>
    /// Metadata removed image file path
    /// </summary>
    private const string MetadataRemovedPngPath = @"TestData/MetadataRemoved.png";
    
    /// <summary>
    /// Not image file path
    /// </summary>
    private const string NotImageFilePath = @"TestData/NotImageFile.txt";
    
    /// <summary>
    /// png metadata comment removed image file path
    /// </summary>
    private const string CommentRemovedPngPath = @"TestData/WithoutComment.png";
    
    /// <summary>
    /// parse from image file
    /// </summary>
    [TestMethod]
    public void LoadFromFile()
    {
        var r = Parser.ParseImage(NormalPngPath);
        AssertNormalImage(r);
    }

    /// <summary>
    /// parse from stream
    /// </summary>
    [TestMethod]
    public void LoadFromStream()
    {
        var s = new FileStream(NormalPngPath, FileMode.Open);
        var r = Parser.ParseImage(s);
        AssertNormalImage(r);
    }

    /// <summary>
    /// assert result object
    /// </summary>
    /// <param name="r"></param>
    private static void AssertNormalImage(ParseResult r)
    {

        var exceptedTags = new[] { "{male}", "short hair", "jacket", "street" };
        
        // assert prompt and tags
        var resultTags = r.Prompt.Split(',').Select(s => s.Trim()).ToList();
        foreach (var t in exceptedTags)
        {
            Assert.IsTrue(resultTags.Any(s => s == t));

            var tag = r.Tags.FirstOrDefault(m => m.Raw == t);
            Assert.IsNotNull(tag);
            Assert.AreEqual(t.Any(c => c == '{') ? t.Count(c => c == '{') : t.Count(c => c == '[') * -1, tag.Strength);
            Assert.AreEqual(t.Replace("{", "").Replace("}","").Replace("[", "").Replace("]",""), tag.Word);
            
        }
        
        // assert quality tags
        Assert.IsTrue(r.Tags.Any(t => t.Word == Parser.QualityTags[0]));
        Assert.IsTrue(r.Tags.Any(t => t.Word == Parser.QualityTags[1]));
        Assert.IsTrue(r.ExistQualityTags);
        
        // assert image resolution
        Assert.AreEqual(512, r.Width);
        Assert.AreEqual(768, r.Height);
        
        // assert Model-Specific settings
        // undesired content
        var uc = r.UndesiredContent.Split(',').Select(s => s.Trim()).ToList();
        Assert.IsTrue(uc.Any(s => s == "bad anatomy"));
        Assert.IsTrue(uc.Any(s => s == "error"));
        Assert.IsTrue(uc.Any(s => s == "bad hands"));

        Assert.AreEqual(28, r.Steps);
        Assert.AreEqual(11, r.Scale);
        Assert.AreEqual("1052277634", r.Seed);
        Assert.AreEqual("k_euler_ancestral", r.Sampler);

    }

    /// <summary>
    /// deserialize prompt
    /// </summary>
    [TestMethod]
    public void DeserializePrompt()
    {
        const string prompt = "{male}, short hair, jacket, street, [very short hair],";

        var tags = Parser.DeserializePrompt(prompt);

        var expectedTags = prompt.Split(',').Select(s => s.Trim());

        foreach (var expected in expectedTags)
        {
            var tag = tags.First(m => m.Raw == expected);
            Assert.IsNotNull(tag);
            Assert.AreEqual(expected.Any(c => c == '{') ? expected.Count(c => c == '{') : expected.Count(c => c == '[') * -1, tag.Strength);
            Assert.AreEqual(expected.Replace("{","").Replace("}","").Replace("[","").Replace("]",""), tag.Word);
        }
    }

    /// <summary>
    /// serialize prompt
    /// </summary>
    [TestMethod]
    public void SerializePrompt()
    {
        var tags = new List<Tag>
        {
            new() { Word = "male", Strength = 1 },
            new() { Word = "short hair", Strength = 0 },
            new() { Word = "very short hair", Strength = -1 }
        };

        var prompt = Parser.SerializePrompt(tags);

        const string excepted = "{male}, short hair, [very short hair], ";
        Assert.AreEqual(excepted, prompt);
    }

    /// <summary>
    /// image without comment of png metadata
    /// </summary>
    [TestMethod]
    public void ImageCommentMissing()
    {
        var r = Parser.ParseImage(CommentRemovedPngPath);
        
        // assert tags
        var exceptedTags = new[] { "{male}", "short hair", "jacket", "street" };
        foreach (var tag in exceptedTags)
        {
            var word = tag.Replace("{", "").Replace("}", "");
            Assert.IsTrue(r.Tags.Any(t => t.Word == word));
        }
        
        // assert image resolution
        Assert.AreEqual(512, r.Width);
        Assert.AreEqual(768, r.Height);
    }

    /// <summary>
    /// image is not png file
    /// </summary>
    [TestMethod]
    public void NotPngImage()
    {
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage("TestData/NotPngImage.jpg");
        });
        Assert.AreEqual("Metadata is not found.", e.Message);
    }

    /// <summary>
    /// metadata not include
    /// </summary>
    [TestMethod]
    public void MetadataNotFound()
    {
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage(MetadataRemovedPngPath);
        });
        
        Assert.AreEqual("Metadata is not found.", e.Message);
    }

    /// <summary>
    /// unsupported file
    /// </summary>
    [TestMethod]
    public void NotImageFile()
    {
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage(NotImageFilePath);    
        });
        Assert.AreEqual("Not supported image.", e.Message);
    }

    /// <summary>
    /// unsupported stream
    /// </summary>
    [TestMethod]
    public void NotImageStream()
    {
        var fs = new FileStream(NotImageFilePath, FileMode.Open);
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage(fs);
        });
        Assert.AreEqual("Not supported image.", e.Message);
    }
}