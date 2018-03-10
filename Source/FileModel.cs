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
        private int currentPage;
        private double scaleFactor;

        public FileModel()
        {
            this.currentPage = 1;
            this.scaleFactor = 1.0;
            this.LastOpened = DateTime.Now;
        }

        public string FullName { get; set; }

        public string Path => System.IO.Path.GetDirectoryName(this.FullName);

        public string Name => System.IO.Path.GetFileName(this.FullName);

        public int CurrentPage
        {
            get => this.currentPage;

            set => this.currentPage = Math.Max(1, value);
        }

        public double ScaleFactor
        {
            get => this.scaleFactor;
            set => this.scaleFactor = Math.Max(0.2, value);
        }

        public DateTime LastOpened { get; set; }
    }
}