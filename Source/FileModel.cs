// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;

    /// <summary>
    ///     This model represents a single document.
    /// </summary>
    public class FileModel
    {
        public FileModel()
        {
            this.LastOpened = DateTime.Now;
        }

        public static FileModel Default { get; } = new FileModel();

        public string FullName { get; set; }

        public string Path => System.IO.Path.GetDirectoryName(this.FullName);

        public string Name => System.IO.Path.GetFileName(this.FullName);

        public int CurrentPage { get; set; }

        public double ScaleFactor { get; set; }

        public DateTime LastOpened { get; set; }
    }
}