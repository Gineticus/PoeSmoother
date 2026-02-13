using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls; // For TextChangedEventArgs
using Xceed.Wpf.Toolkit; // For ColorPicker

namespace PoeSmoother;

public partial class MinimapEditor : Window
{
    public float UnrevealedR { get; private set; } = 0.0f;
    public float UnrevealedG { get; private set; } = 0.0f;
    public float UnrevealedB { get; private set; } = 0.0f;
    public float UnrevealedA { get; private set; } = 0.01f;
    public float OutlineR { get; private set; } = 1.0f;
    public float OutlineG { get; private set; } = 1.0f;
    public float OutlineB { get; private set; } = 1.0f;
    public float OutlineA { get; private set; } = 1.0f;
    public float FrontR { get; private set; } = 0.0f;
    public float FrontG { get; private set; } = 0.5f;
    public float FrontB { get; private set; } = 1.0f;
    public float FrontA { get; private set; } = 0.5f;

    private System.Windows.Media.Color unrevealedBaseColor = System.Windows.Media.Colors.Black;
    private System.Windows.Media.Color outlineBaseColor = System.Windows.Media.Colors.White;
    private System.Windows.Media.Color frontBaseColor = System.Windows.Media.Colors.Blue;

    public MinimapEditor()
    {
        InitializeComponent();

        UpdatePreviews();
    }

    private void UpdatePreviews()
    {
        UnrevealedPreview.Fill = new SolidColorBrush(unrevealedBaseColor);
        OutlinePreview.Fill = new SolidColorBrush(outlineBaseColor);
        FrontPreview.Fill = new SolidColorBrush(frontBaseColor);
    }

    private void UnrevealedPickColor_Click(object sender, RoutedEventArgs e)
    {
        var colorPicker = new ColorPicker { SelectedColor = unrevealedBaseColor };
        var dialog = new Window
        {
            Title = "Select Color",
            Content = colorPicker,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
            Owner = this
        };
        dialog.ShowDialog();
        unrevealedBaseColor = colorPicker.SelectedColor ?? System.Windows.Media.Colors.Black;
        UpdatePreviews();
    }

    private void OutlinePickColor_Click(object sender, RoutedEventArgs e)
    {
        var colorPicker = new ColorPicker { SelectedColor = outlineBaseColor };
        var dialog = new Window
        {
            Title = "Select Color",
            Content = colorPicker,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
            Owner = this
        };
        dialog.ShowDialog();
        outlineBaseColor = colorPicker.SelectedColor ?? System.Windows.Media.Colors.White;
        UpdatePreviews();
    }

    private void FrontPickColor_Click(object sender, RoutedEventArgs e)
    {
        var colorPicker = new ColorPicker { SelectedColor = frontBaseColor };
        var dialog = new Window
        {
            Title = "Select Color",
            Content = colorPicker,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
            Owner = this
        };
        dialog.ShowDialog();
        frontBaseColor = colorPicker.SelectedColor ?? System.Windows.Media.Colors.Blue;
        UpdatePreviews();
    }

    private void UnrevealedASlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UnrevealedA = (float)e.NewValue;
    }

    private void UnrevealedAValueText_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(UnrevealedAValueText.Text, out float val))
        {
            UnrevealedA = Math.Clamp(val, 0f, 1f);
            UnrevealedASlider.Value = val;
        }
    }

    private void OutlineASlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        OutlineA = (float)e.NewValue;
    }

    private void OutlineAValueText_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(OutlineAValueText.Text, out float val))
        {
            OutlineA = Math.Clamp(val, 0f, 1f);
            OutlineASlider.Value = val;
        }
    }

    private void FrontASlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        FrontA = (float)e.NewValue;
    }

    private void FrontAValueText_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(FrontAValueText.Text, out float val))
        {
            FrontA = Math.Clamp(val, 0f, 1f);
            FrontASlider.Value = val;
        }
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        UnrevealedR = unrevealedBaseColor.R / 255f;
        UnrevealedG = unrevealedBaseColor.G / 255f;
        UnrevealedB = unrevealedBaseColor.B / 255f;
        OutlineR = outlineBaseColor.R / 255f;
        OutlineG = outlineBaseColor.G / 255f;
        OutlineB = outlineBaseColor.B / 255f;
        FrontR = frontBaseColor.R / 255f;
        FrontG = frontBaseColor.G / 255f;
        FrontB = frontBaseColor.B / 255f;
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}