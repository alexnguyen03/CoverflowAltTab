using System.Windows;
using System.Windows.Input;

namespace CoverflowAltTab.UI;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();
    }

    public event EventHandler<OverlayCommandRequestedEventArgs>? CommandRequested;

    public event EventHandler<OverlayDeactivatedEventArgs>? OverlayDeactivated;

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        if (e.Key == Key.Tab)
        {
            var command = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
                ? OverlayCommand.Previous
                : OverlayCommand.Next;

            e.Handled = true;
            CommandRequested?.Invoke(this, new OverlayCommandRequestedEventArgs(command));
            return;
        }

        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            CommandRequested?.Invoke(this, new OverlayCommandRequestedEventArgs(OverlayCommand.Commit));
            return;
        }

        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            CommandRequested?.Invoke(this, new OverlayCommandRequestedEventArgs(OverlayCommand.Cancel));
        }
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        OverlayDeactivated?.Invoke(this, new OverlayDeactivatedEventArgs());
    }
}
