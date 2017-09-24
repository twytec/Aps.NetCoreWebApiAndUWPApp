using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
{
    public class FileDataModel
    {
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public string Base64String { get; set; }
        public string FileName { get; set; }
    }
}
