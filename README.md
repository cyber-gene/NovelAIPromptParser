# NovelAIPromptParser

![Nuget](https://img.shields.io/nuget/v/NovelAIPromptParser)

Load NovelAI prompt and settings from image.

## Usage

### From image file

```csharp
var path = "path\to\image\file.png";
var result = Parser.ParseImage(path);
```

### From Stream

```csharp
var stream = new FileStream("path\to\image\file.png", FileMode.Open);
var result = Parser.ParseImage(stream);
```

### Deserialize prompt string

```csharp
var prompt = "prompt, [you], {like}";
var result = Parser.DeserializePrompt(prompt);
```

### Serialize prompt string

```csharp
var tags = new List<Tag>
{
    new() { Word = "prompt", Strength = 0 },
    new() { Word = "you", Strength = -1 },
    new() { Word = "like", Strength = 1 }
};
var prompt = Parser.SerializePrompt(tags);
```