using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoverflowAltTab.UI;

public sealed class WindowListItemViewModel : INotifyPropertyChanged
{
    private bool _isSelected;
    private double _x;
    private double _y;
    private double _width;
    private double _height;
    private double _scale = 1d;
    private double _rotation;
    private double _opacity = 1d;
    private int _zIndex;

    public required string Title { get; init; }

    public required string Subtitle { get; init; }

    public required nint WindowHandle { get; init; }

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

    public double X
    {
        get => _x;
        set
        {
            if (Math.Abs(_x - value) < 0.01d)
            {
                return;
            }

            _x = value;
            OnPropertyChanged();
        }
    }

    public double Y
    {
        get => _y;
        set
        {
            if (Math.Abs(_y - value) < 0.01d)
            {
                return;
            }

            _y = value;
            OnPropertyChanged();
        }
    }

    public double Width
    {
        get => _width;
        set
        {
            if (Math.Abs(_width - value) < 0.01d)
            {
                return;
            }

            _width = value;
            OnPropertyChanged();
        }
    }

    public double Height
    {
        get => _height;
        set
        {
            if (Math.Abs(_height - value) < 0.01d)
            {
                return;
            }

            _height = value;
            OnPropertyChanged();
        }
    }

    public double Scale
    {
        get => _scale;
        set
        {
            if (Math.Abs(_scale - value) < 0.01d)
            {
                return;
            }

            _scale = value;
            OnPropertyChanged();
        }
    }

    public double Rotation
    {
        get => _rotation;
        set
        {
            if (Math.Abs(_rotation - value) < 0.01d)
            {
                return;
            }

            _rotation = value;
            OnPropertyChanged();
        }
    }

    public double Opacity
    {
        get => _opacity;
        set
        {
            if (Math.Abs(_opacity - value) < 0.01d)
            {
                return;
            }

            _opacity = value;
            OnPropertyChanged();
        }
    }

    public int ZIndex
    {
        get => _zIndex;
        set
        {
            if (_zIndex == value)
            {
                return;
            }

            _zIndex = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
