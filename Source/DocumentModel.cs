// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;

    /// <summary>
    ///     This model represents a single document.
    /// </summary>
    public class DocumentModel
    {
        public DocumentModel()
        {
            this.LastOpened = DateTime.Now;
        }

        public static DocumentModel Default { get; } = new DocumentModel();

        public string FullName { get; set; }

        public string Name => System.IO.Path.GetFileName(this.FullName);

        public string Path => System.IO.Path.GetDirectoryName(this.FullName);

        public int CurrentPage { get; set; }

        public double ScaleFactor { get; set; }

        public DateTime LastOpened { get; set; }
    }
}