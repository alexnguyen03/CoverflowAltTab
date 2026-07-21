using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoverflowAltTab.UI;

public sealed class WindowListItemViewModel : INotifyPropertyChanged
{
    private bool _isSelected;

    public required string Title { get; init; }

    public required string Subtitle { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
