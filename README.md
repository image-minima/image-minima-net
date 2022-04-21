# Image Minima API client for .NET

.NET client for the Image Minima API, used for [ImageMinima](https://imageminima.com). Image Minima compresses your images intelligently. Read more at [http://imageminima.com](http://imageminima.com).

## Documentation

[Go to the documentation for the .NET client](https://imageminima.com/developers/reference/dotnet).

## Installation

Install the API client:

```
Install-Package ImageMinima
```

Or add this to your `project.json`:

```json
{
  "dependencies": {
    "ImageMinima": "*",
  }
}
```

## Usage

```csharp
using ImageMinima;

class Compress
{
  static void Main()
  {
    var client = new ImageMinimaClient()
    {
        Key = "YOUR_API_KEY"
    };

    var source = new Source();
    source.FromFile("unoptimized.png").ToFile("optimized.png").Wait();
  }
}
```

## Running tests

```
dotnet restore
dotnet test test/ImageMinima.Tests
```

### Integration tests

```
dotnet restore
IMAGE_MINIMA_KEY=$YOUR_API_KEY dotnet test src/ImageMinima.Tests.Integration
```
Or add a `.env` file to the `/test/ImageMinima.Tests.Integration` directory in the format
```
IMAGE_MINIMA_KEY=<YOUR_API_KEY>
```