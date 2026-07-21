namespace CoverflowAltTab.Extensibility;

public sealed class ExtensionManifest
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string Version { get; init; } = "1.0.0";

    public required string Assembly { get; init; }

    public string? EntryType { get; init; }

    public string Description { get; init; } = string.Empty;
}
