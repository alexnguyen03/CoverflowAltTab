using CoverflowAltTab.Extensibility;

namespace SampleExtension;

public sealed class SampleExtension : IExtension
{
    public string Id => "sample.dismiss-on-lost-focus";

    public string Name => "Dismiss On Lost Focus";

    public void Initialize(IHostContext context)
    {
        context.RegisterOverlayDismissBehavior(new LostFocusDismissBehavior());
        context.Logger.Info("SampleExtension registered LostFocusDismissBehavior.");
    }
}

public sealed class LostFocusDismissBehavior : IOverlayDismissBehavior
{
    public bool CancelOnOverlayLostFocus => true;
}
