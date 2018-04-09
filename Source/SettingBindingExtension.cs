// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System.Windows.Data;
    using PdfDisplay.Properties;

    internal class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            this.Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.Source = Settings.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }
}