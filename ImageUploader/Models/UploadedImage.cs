using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageUploader.Models
{
    public class UploadedImage
    {
        public String ContentType { get; set; }
        public Byte[] Data { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
    }
}