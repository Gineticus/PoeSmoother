using PoeSmoother.Models;
using System.Collections.ObjectModel;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PoeSmoother;

public partial class ColorModsEditor : Window
{
    public ColorModsEditor(ObservableCollection<ColorModsViewModel> colorMods)
    {
        InitializeComponent();
        ColorModsItemsControl.ItemsSource = colorMods;
        SourceInitialized += (s, e) => ApplyDarkTitleBar();
    }

    private void ApplyDarkTitleBar()
    {
        if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
        {
            IntPtr hwnd = hwndSource.Handle;

            // Use DWMWA_USE_IMMERSIVE_DARK_MODE (20) for Windows 11 / Windows 10 build 19041+
            int attribute = 20;
            int useImmersiveDarkMode = 1;
            DwmSetWindowAttribute(hwnd, attribute, ref useImmersiveDarkMode, sizeof(int));
        }
    }

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public static bool Show(ObservableCollection<ColorModsViewModel> colorMods)
    {
        var dialog = new ColorModsEditor(colorMods);

        if (Application.Current.MainWindow != null)
        {
            dialog.Owner = Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        return dialog.ShowDialog() == true;
    }
}
