// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using Telerik.Windows.Controls;

    public partial class MainView
    {
        public MainView()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            this.InitializeComponent();
        }
    }
}