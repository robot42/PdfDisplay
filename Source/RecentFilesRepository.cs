// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Xml.Serialization;

    internal class RecentFilesRepository : IRecentFilesQuery
    {
        private const string RecentFilesName = "PdfDisplayRecentFiles.xml";
        private const int MaximumFilesInHistory = 8;
        private List<FileModel> files = new List<FileModel>();

        public IEnumerable<FileModel> Files => this.files;

        public void Load()
        {
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(homePath, RecentFilesName);

                if (File.Exists(filePath) == false)
                {
                    return;
                }

                using (var file = new StreamReader(Path.Combine(homePath, RecentFilesName)))
                {
                    var serializer = new XmlSerializer(typeof(List<FileModel>));
                    this.files = (List<FileModel>)serializer.Deserialize(file);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Save()
        {
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(homePath, RecentFilesName);

                using (var file = new StreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(List<FileModel>));
                    serializer.Serialize(file, this.files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public FileModel GetOrAdd(string fileName)
        {
            var existingModel = this.files.FirstOrDefault(model => model.FullName == fileName);

            if (existingModel != null)
            {
                existingModel.LastOpened = DateTime.Now;
                return existingModel;
            }

            var result = new FileModel {FullName = fileName};

            this.files.Add(result);
            this.RemoveOldestFiles();
            return result;
        }

        private void RemoveOldestFiles()
        {
            if (this.files.Count <= MaximumFilesInHistory)
            {
                return;
            }

            var sortedFiles = this.files.OrderByDescending(file => file.LastOpened);

            foreach (var fileModel in sortedFiles.Skip(MaximumFilesInHistory))
            {
                this.files.Remove(fileModel);
            }
        }

        public void Remove(string fileName)
        {
            this.files.RemoveAll(model => model.FullName == fileName);
        }
    }
}