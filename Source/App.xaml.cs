// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    public partial class App
    {
        public App()
        {
            Windows8Palette.Palette.AccentColor = (Color)ColorConverter.ConvertFromString("#BF963E");
        }
    }
}