using System.Windows;

namespace PdfDisplay
{
    using System.Windows.Media;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Windows8Palette.Palette.AccentColor = (Color)ColorConverter.ConvertFromString("#FF80397B");
        }
    }
}
