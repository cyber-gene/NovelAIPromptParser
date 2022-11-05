# NovelAIPromptParser

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

### Parse prompt string

```csharp
var prompt = "prompt, [you], {like}";
var result = Parser.ParsePrompt(prompt);
```