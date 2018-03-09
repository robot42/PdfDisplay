// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System.Collections.Generic;

    internal interface IRecentFilesQuery
    {
        IEnumerable<FileModel> Files { get; }
    }
}