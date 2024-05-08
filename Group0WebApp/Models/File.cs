using System;

namespace Group_WebApp.Models
{
    public class File
    {
        public int FileID { get; set; }
        public int UserID { get; set; } 
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public long FileSize { get; set; } 
        public string? FilePath { get; set; } 
        public DateTime UploadDate { get; set; }
    }
}
