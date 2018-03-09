// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using Microsoft.Win32;

    internal static class DocumentHelper
    {
        internal static bool TrySelectDocument(out string fileName)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".pdf",
                Filter = "PDF documents |*.pdf",
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                fileName = dialog.FileName;
                return true;
            }

            fileName = string.Empty;
            return false;
        }
    }
}