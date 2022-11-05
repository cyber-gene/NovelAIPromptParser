using NovelAIPromptParser.Entity;

namespace NovelAIPromptParserTest;

[TestClass]
public class ParserTest
{
    /// <summary>
    /// parse from image file
    /// </summary>
    [TestMethod]
    public void LoadFromFile()
    {
        const string path = @".\TestData\Normal.png";
        var r = Parser.ParseImage(path);
        AssertNormalImage(r);
    }

    /// <summary>
    /// parse from stream
    /// </summary>
    [TestMethod]
    public void LoadFromStream()
    {
        const string path = @".\TestData\Normal.png";
        var s = new FileStream(path, FileMode.Open);
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
    /// metadata not include
    /// </summary>
    [TestMethod]
    public void MetadataNotFound()
    {
        const string path = @".\TestData\MetadataRemoved.png";
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage(path);
        });
        
        Assert.AreEqual("Metadata is not found.", e.Message);
    }

    /// <summary>
    /// unsupported file
    /// </summary>
    [TestMethod]
    public void NotImageFile()
    {
        const string path = @"TestData\NotImageFile.txt";
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage(path);    
        });
        Assert.AreEqual("Not supported image.", e.Message);
    }

    /// <summary>
    /// unsupported stream
    /// </summary>
    [TestMethod]
    public void NotImageStream()
    {
        const string path = @"TestData\NotImageFile.txt";
        var fs = new FileStream(path, FileMode.Open);
        var e = Assert.ThrowsException<ParserException>(() =>
        {
            Parser.ParseImage(fs);
        });
        Assert.AreEqual("Not supported image.", e.Message);
    }
}