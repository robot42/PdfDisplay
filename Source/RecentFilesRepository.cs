// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    internal class RecentFilesRepository : IRecentFilesQuery
    {
        private List<FileModel> files = new List<FileModel>();

        public IEnumerable<FileModel> Files => this.files;

        public void Load()
        {
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                using (var file = new StreamReader(Path.Combine(homePath, "PdfDisplayRecentFiles.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<FileModel>));
                    this.files = (List<FileModel>)serializer.Deserialize(file);
                }
            }
            catch
            {
            }
        }

        public void Save()
        {
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                using (var file = new StreamWriter(Path.Combine(homePath, "PdfDisplayRecentFiles.xml")))
                {
                    var serializer = new XmlSerializer(typeof(List<FileModel>));
                    serializer.Serialize(file, this.files);
                }
            }
            catch
            {
            }
        }

        public void Add(FileModel model)
        {
            var existingModel = this.files.FirstOrDefault(m => m.FullName == model.FullName);
            
            if (existingModel != null)
            {
                this.files.Remove(existingModel);
            }

            this.files.Insert(0, model);
            while (this.files.Count > 8)
            {
                this.files.RemoveAt(this.files.Count - 1);
            }
        }

        public void Remove(FileModel model)
        {
            this.files.Remove(model);
        }
    }
}