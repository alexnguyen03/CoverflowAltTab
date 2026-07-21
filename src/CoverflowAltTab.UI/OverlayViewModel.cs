using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoverflowAltTab.UI.Rendering;

namespace CoverflowAltTab.UI;

public sealed class OverlayViewModel : INotifyPropertyChanged
{
    private string _headerTitle = string.Empty;
    private string _headerSubtitle = string.Empty;
    private string _rendererId = string.Empty;
    private bool _usesFreeformLayout;
    private double _stageWidth;
    private double _stageHeight;

    public ObservableCollection<WindowListItemViewModel> Windows { get; } = [];

    public string HeaderTitle
    {
        get => _headerTitle;
        set
        {
            if (_headerTitle == value)
            {
                return;
            }

            _headerTitle = value;
            OnPropertyChanged();
        }
    }

    public string HeaderSubtitle
    {
        get => _headerSubtitle;
        set
        {
            if (_headerSubtitle == value)
            {
                return;
            }

            _headerSubtitle = value;
            OnPropertyChanged();
        }
    }

    public string RendererId
    {
        get => _rendererId;
        set
        {
            if (_rendererId == value)
            {
                return;
            }

            _rendererId = value;
            OnPropertyChanged();
        }
    }

    public bool UsesFreeformLayout
    {
        get => _usesFreeformLayout;
        set
        {
            if (_usesFreeformLayout == value)
            {
                return;
            }

            _usesFreeformLayout = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UsesListLayout));
        }
    }

    public bool UsesListLayout => !UsesFreeformLayout;

    public double StageWidth
    {
        get => _stageWidth;
        set
        {
            if (Math.Abs(_stageWidth - value) < 0.01d)
            {
                return;
            }

            _stageWidth = value;
            OnPropertyChanged();
        }
    }

    public double StageHeight
    {
        get => _stageHeight;
        set
        {
            if (Math.Abs(_stageHeight - value) < 0.01d)
            {
                return;
            }

            _stageHeight = value;
            OnPropertyChanged();
        }
    }

    public void ShowModel(OverlayRenderModel model)
    {
        Windows.Clear();
        foreach (var item in model.Items)
        {
            Windows.Add(new WindowListItemViewModel
            {
                Title = item.Title,
                Subtitle = item.Subtitle,
                IsSelected = item.IsSelected,
                X = item.X,
                Y = item.Y,
                Width = item.Width,
                Height = item.Height,
                Scale = item.Scale,
                Rotation = item.Rotation,
                Opacity = item.Opacity,
                ZIndex = item.ZIndex,
            });
        }

        HeaderTitle = model.HeaderTitle;
        HeaderSubtitle = model.HeaderSubtitle;
        RendererId = model.RendererId;
        UsesFreeformLayout = model.UsesFreeformLayout;
        StageWidth = model.StageWidth;
        StageHeight = model.StageHeight;
    }

    public void UpdateSelection(OverlayRenderModel model)
    {
        for (var index = 0; index < Windows.Count; index++)
        {
            var source = model.Items[index];
            var target = Windows[index];
            target.IsSelected = source.IsSelected;
            target.X = source.X;
            target.Y = source.Y;
            target.Width = source.Width;
            target.Height = source.Height;
            target.Scale = source.Scale;
            target.Rotation = source.Rotation;
            target.Opacity = source.Opacity;
            target.ZIndex = source.ZIndex;
        }

        UsesFreeformLayout = model.UsesFreeformLayout;
        StageWidth = model.StageWidth;
        StageHeight = model.StageHeight;
    }

    public void Clear()
    {
        Windows.Clear();
        HeaderTitle = string.Empty;
        HeaderSubtitle = string.Empty;
        RendererId = string.Empty;
        UsesFreeformLayout = false;
        StageWidth = 0d;
        StageHeight = 0d;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
