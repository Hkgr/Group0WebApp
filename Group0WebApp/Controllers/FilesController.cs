using Group_WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = Group_WebApp.Models.File;

namespace Group_WebApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult ManagementFiles(string username, string fullname, string folderPath, int userID)
        {
            ViewData["Title"] = "MangmentFiles";

            // استرداد البيانات المحملة لهذا المستخدم
            var uploadedFiles = GetUploadedFiles(userID);

            ViewData["Username"] = username;
            ViewData["FullName"] = fullname;
            ViewData["FolderPath"] = folderPath;
            ViewData["UserID"] = userID;
            ViewData["UploadedFiles"] = uploadedFiles;

            return View();
        }

        [HttpPost]
        public IActionResult UploadFiles(string folderPath)
        {
            try
            {
                string username = Request.Cookies["Username"];
                string fullname = Request.Cookies["FullName"];
                int userID = Convert.ToInt32(Request.Cookies["UserID"]);

                foreach (var file in Request.Form.Files)
                {
                    // تحقق من عدم وجود الملف في قاعدة البيانات لنفس المستخدم
                    if (!IsFileAlreadyUploaded(userID, file.FileName))
                    {
                        // نسخ الملف إلى المجلد المستهدف
                        string filePath = CopyFileToFolder(folderPath, file);

                        // إضافة بيانات الملف إلى قاعدة البيانات
                        AddFileToDatabase(userID, file, filePath);
                    }
                    else
                    {
                        // إذا تم العثور على الملف في قاعدة البيانات، عرض رسالة الخطأ
                        ViewData["ErrorMessage"] = $"File '{file.FileName}' already exists in the folder.";
                        ViewData["Username"] = username;
                        ViewData["FullName"] = fullname;
                        ViewData["FolderPath"] = folderPath;
                        ViewData["UserID"] = userID;
                        var uploadedFiles = GetUploadedFiles(userID);
                        ViewData["UploadedFiles"] = uploadedFiles;
                        return View("ManagementFiles");
                    }
                }

                // حفظ التغييرات في قاعدة البيانات
                _context?.SaveChanges();

                return RedirectToAction("ManagementFiles", new { username, fullname, userID, folderPath });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Error occurred while uploading files: " + ex.Message;
                return View("ManagementFiles");
            }
        }

        // دالة للتحقق من وجود الملف في قاعدة البيانات لنفس المستخدم
        private bool IsFileAlreadyUploaded(int userID, string fileName)
        {
            return _context.Files.Any(f => f.UserID == userID && f.FileName == fileName);
        }


        // دالة لنسخ الملفات إلى المجلد المطلوب
        private string CopyFileToFolder(string folderPath, Microsoft.AspNetCore.Http.IFormFile file)
        {
            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filePath;
        }

        // دالة لإضافة بيانات الملفات إلى قاعدة البيانات
        private void AddFileToDatabase(int userID, Microsoft.AspNetCore.Http.IFormFile file, string filePath)
        {
            // استخراج معلومات الملف
            string fileName = Path.GetFileName(file.FileName);
            string fileType = Path.GetExtension(fileName);
            long fileSize = file.Length;
            DateTime uploadDate = DateTime.Now;

            // إنشاء سلسلة نصية تحوي بيانات الملف
            string fileInfoText = $"UserID: {userID}\n" +
                                  $"File Name: {fileName}\n" +
                                  $"File Type: {fileType}\n" +
                                  $"File Size: {fileSize} bytes\n" +
                                  $"Upload Date: {uploadDate}\n" +
                                  $"File Path: {filePath}";

            // تحديد مسار ملف النصي على سطح المكتب
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string logFileName = "UploadedFilesInfo.txt";
            string logFilePath = Path.Combine(uploadsFolder, logFileName);

            // حفظ بيانات الملف في ملف نصي على سطح المكتب
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine("Uploaded File Info:");
                writer.WriteLine(fileInfoText);
                writer.WriteLine("-------------------------------");
            }

            // إضافة بيانات الملف إلى قاعدة البيانات
            var uploadedFile = new File
            {
                UserID = userID,
                FileName = fileName,
                FileType = fileType,
                FileSize = fileSize,
                UploadDate = uploadDate,
                FilePath = filePath
            };

            _context.Files.Add(uploadedFile);
        }


        // دالة لاسترداد البيانات المحملة لهذا المستخدم
        private List<File> GetUploadedFiles(int userID)
        {
            // جلب بيانات الملفات المحملة لهذا المستخدم
            var uploadedFiles = _context.Files?.Where(f => f.UserID == userID).ToList();
            return uploadedFiles;
        }
    }
}
