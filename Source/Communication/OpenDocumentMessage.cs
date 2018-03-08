// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay.Communication
{
    public class OpenDocumentMessage
    {
        public OpenDocumentMessage(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; }
    }
}