namespace CoverflowAltTab.Extensibility;

public interface IExtension
{
    string Id { get; }

    string Name { get; }

    void Initialize(IHostContext context);
}
