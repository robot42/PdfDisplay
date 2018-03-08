// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    public class RescentFileModel
    {
        public string FullName { get; set; }
    }

    public class RescentFileViewModel
    {
        private readonly RescentFileModel model;

        public RescentFileViewModel(RescentFileModel model)
        {
            this.model = model;
        }

        public string FullName => model.FullName;

        public string Name => System.IO.Path.GetFileName(model.FullName);

        public string Path => System.IO.Path.GetDirectoryName(model.FullName);
    }
}