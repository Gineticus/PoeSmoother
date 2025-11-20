using System.ComponentModel;
using System.Runtime.CompilerServices;
using PoeSmoother.Patches;

namespace PoeSmoother.Models;

public class ColorModsViewModel : INotifyPropertyChanged
{
    private bool _isSelected;

    private string _selectedColor;

    public ColorModsOption Option { get; }
    public string Name => Option.Name;

    public List<string> AvailableColors { get; } = new() { "red", "green", "blue", "yellow", "pink" };

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                // Keep underlying option in sync so patches read current selection
                if (Option != null)
                {
                    Option.IsEnabled = _isSelected;
                }
                OnPropertyChanged();
            }
        }
    }

    public string SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;
                // Keep underlying option in sync so patches read current color
                if (Option != null)
                {
                    Option.Color = _selectedColor;
                }
                OnPropertyChanged();
            }
        }
    }

    public ColorModsViewModel(ColorModsOption option)
    {
        Option = option;
        _isSelected = option.IsEnabled;
        _selectedColor = option.Color;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
