using NovelAIPromptParser.Entity;

namespace NovelAIPromptParserTest;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void LoadFromFile()
    {
        const string path = @".\TestData\Normal.png";
        var r = Parser.ParseImage(path);
        AssertNormalImage(r);
    }

    [TestMethod]
    public void LoadFromStream()
    {
        const string path = @".\TestData\Normal.png";
        var s = new FileStream(path, FileMode.Open);
        var r = Parser.ParseImage(s);
        AssertNormalImage(r);
    }

    private void AssertNormalImage(ParseResult r)
    {
        
        // assert prompt
        var tags = r.Prompt.Split(',').Select(s => s.Trim()).ToList();
        Assert.IsTrue(tags.Any(s => s == "{male}"));
        Assert.IsTrue(tags.Any(s => s == "short hair"));
        Assert.IsTrue(tags.Any((s => s == "jacket")));
        Assert.IsTrue(tags.Any(s => s == "street"));
        
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
}