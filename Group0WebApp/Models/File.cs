using System;

namespace Group_WebApp.Models
{
    public class File
    {
        public int FileID { get; set; }
        public int UserID { get; set; } // Foreign Key to User
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public long FileSize { get; set; } // تم تغيير نوع بيانات حجم الملف إلى long
        public string? FilePath { get; set; } // إضافة مسار الملف
        public DateTime UploadDate { get; set; }
    }
}
