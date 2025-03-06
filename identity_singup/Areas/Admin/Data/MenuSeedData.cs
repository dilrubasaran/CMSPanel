using identity_singup.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace identity_singup.Areas.Admin.Data
{
    
    public static class MenuSeedData
    {
        public static async Task SeedMenuItemsAsync(IServiceProvider serviceProvider)
        {
            try 
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Veritabanının oluşturulduğundan emin olalım
                await dbContext.Database.EnsureCreatedAsync();

                // Mevcut menü öğelerini kontrol et
                var existingMenuCount = await dbContext.MenuItems.CountAsync();
                Console.WriteLine($"Mevcut menü öğesi sayısı: {existingMenuCount}");

                if (existingMenuCount > 0)
                {
                    Console.WriteLine("Menü verileri zaten mevcut, seed işlemi atlanıyor.");
                    return;
                }

                var allMenuItems = new List<MenuItem>();

                #region Admin Menüleri
                var adminMenuItems = new[]
                {
                    new MenuItem {  Name = "Dashboard", Icon = "bi bi-speedometer2", AreaName = "Admin", ControllerName = "Home", ActionName = "Index", Role = "admin", SortNumber = 1, SubMenuItems = new List<MenuItem>() },

                    new MenuItem {  Name = "Kullanıcı Yönetimi", Icon = "bi bi-people", Role = "admin", SortNumber = 2, ControllerName = "#", ActionName = "#", },
                    new MenuItem { Name = "Kullanıcı Listesi", Icon = "bi bi-person-lines-fill", AreaName = "Admin", ControllerName = "Home", ActionName = "UserList", ParentId = 2, Role = "admin", SortNumber = 3 },
                    new MenuItem { Name = "Kullanıcı Ekleme", Icon = "bi bi-person-plus", AreaName = "Admin", ControllerName = "User", ActionName = "Create", ParentId = 2, Role = "admin", SortNumber = 4 },
                    new MenuItem { Name = "Onay Bekleyenler", Icon = "bi bi-person-check", AreaName = "Admin", ControllerName = "User", ActionName = "PendingApprovals", ParentId = 2, Role = "admin", SortNumber = 5 },
                    new MenuItem { Name = "Aktif/Pasif Kullanıcılar", Icon = "bi bi-toggle-on", AreaName = "Admin", ControllerName = "User", ActionName = "UserStatus", ParentId = 2, Role = "admin", SortNumber = 6 },

                    new MenuItem {  Name = "Rol Yönetimi", Icon = "bi bi-shield-lock", AreaName = "Admin", ControllerName = "Role", ActionName = "Index", Role = "admin", SortNumber = 7 },

                    new MenuItem {  Name = "Eğitim Yönetimi", Icon = "bi bi-mortarboard", Role = "admin", SortNumber = 8, ControllerName = "#", ActionName = "#",},
                    new MenuItem {  Name = "Eğitim Listesi", Icon = "bi bi-list-ul", AreaName = "Instructor", ControllerName = "Edu", ActionName = "EduList", ParentId = 8, Role = "admin", SortNumber = 9 },
                    new MenuItem {  Name = "Eğitim Ekle", Icon = "bi bi-plus-circle", AreaName = "Instructor", ControllerName = "Edu", ActionName = "EduCreate", ParentId = 8, Role = "admin", SortNumber = 10 },
                    new MenuItem {  Name = "Eğitmen Onay İşlemleri", Icon = "bi bi-check-circle", AreaName = "Admin", ControllerName = "Permission", ActionName = "PendingRequests", ParentId = 8, Role = "admin", SortNumber = 11 },

                    new MenuItem {  Name = "Kategori Yönetimi", Icon = "bi bi-diagram-2", Role = "admin", SortNumber = 12, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Kategori Listesi", Icon = "bi bi-list-check", AreaName = "Admin", ControllerName = "Category", ActionName = "List", ParentId = 12, Role = "admin", SortNumber = 13 },
                    new MenuItem {  Name = "Kategori Ekle", Icon = "bi bi-plus-circle", AreaName = "Admin", ControllerName = "Category", ActionName = "Create", ParentId = 12, Role = "admin", SortNumber = 14 },
                    new MenuItem {  Name = "Kategori Güncelle", Icon = "bi bi-pencil", AreaName = "Admin", ControllerName = "Category", ActionName = "Update", ParentId = 12, Role = "admin", SortNumber = 15 },
                    new MenuItem {  Name = "Eğitim Gruplandırma", Icon = "bi bi-grid", AreaName = "Admin", ControllerName = "Category", ActionName = "GroupCourses", ParentId = 12, Role = "admin", SortNumber = 16 },

                    new MenuItem { Name = "Güvenlik Yönetimi", Icon = "bi bi-shield-check", Role = "admin", SortNumber = 17,ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Giriş İstatistikleri", Icon = "bi bi-graph-up", AreaName = "Admin", ControllerName = "Security", ActionName = "LoginStats", ParentId = 17, Role = "admin", SortNumber = 18 },
                    new MenuItem {  Name = "Tehdit İçeren Giriş İşlemleri", Icon = "bi bi-exclamation-triangle", AreaName = "Admin", ControllerName = "Security", ActionName = "Threats", ParentId = 17, Role = "admin", SortNumber = 19 },
                    new MenuItem { Name = "İşlem Logları", Icon = "bi bi-activity", AreaName = "Admin", ControllerName = "Security", ActionName = "LoginAttempts", ParentId = 17, Role = "admin", SortNumber = 20 },

                    new MenuItem {  Name = "Ödeme ve Satış", Icon = "bi bi-cash-stack", Role = "admin", SortNumber = 21, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Satış Raporları", Icon = "bi bi-graph-up", AreaName = "Admin", ControllerName = "Payment", ActionName = "SalesReport", ParentId = 21, Role = "admin", SortNumber = 22 },
                    new MenuItem { Name = "Ödeme Durumu", Icon = "bi bi-credit-card", AreaName = "Admin", ControllerName = "Payment", ActionName = "Status", ParentId = 21, Role = "admin", SortNumber = 23 },
                    new MenuItem { Name = "İade Yönetimi", Icon = "bi bi-arrow-return-left", AreaName = "Admin", ControllerName = "Payment", ActionName = "Refunds", ParentId = 21, Role = "admin", SortNumber = 24 },
                    new MenuItem { Name = "Ödeme Onaylama", Icon = "bi bi-check-circle", AreaName = "Admin", ControllerName = "Payment", ActionName = "Approve", ParentId = 21, Role = "admin", SortNumber = 25 },

                    new MenuItem {  Name = "Bildirim Yönetimi", Icon = "bi bi-bell", Role = "admin", SortNumber = 26, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Genel Duyurular", Icon = "bi bi-megaphone", AreaName = "Admin", ControllerName = "Notification", ActionName = "Announcements", ParentId = 26, Role = "admin", SortNumber = 27 },
                    new MenuItem {  Name = "Özel Bildirimler", Icon = "bi bi-envelope", AreaName = "Admin", ControllerName = "Notification", ActionName = "Custom", ParentId = 26, Role = "admin", SortNumber = 28 },
                    new MenuItem { Name = "Sistem Bildirimleri", Icon = "bi bi-gear", AreaName = "Admin", ControllerName = "Notification", ActionName = "System", ParentId = 26, Role = "admin", SortNumber = 29 },

                    new MenuItem {  Name = "Form Yönetimi", Icon = "bi bi-file-text", Role = "admin", SortNumber = 30,ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "İstek Formları", Icon = "bi bi-chat-right-text", AreaName = "Admin", ControllerName = "Form", ActionName = "Requests", ParentId = 30, Role = "admin", SortNumber = 31 },
                    new MenuItem {  Name = "Şikayet Formları", Icon = "bi bi-exclamation-circle", AreaName = "Admin", ControllerName = "Form", ActionName = "Complaints", ParentId = 30, Role = "admin", SortNumber = 32 },
                    new MenuItem {  Name = "Öneri Formları", Icon = "bi bi-lightbulb", AreaName = "Admin", ControllerName = "Form", ActionName = "Suggestions", ParentId = 30, Role = "admin", SortNumber = 33 },
                    new MenuItem {  Name = "Form Yönetimi", Icon = "bi bi-gear", AreaName = "Admin", ControllerName = "Form", ActionName = "Management", ParentId = 30, Role = "admin", SortNumber = 34 },

                    new MenuItem { Name = "Yorum Yönetimi", Icon = "bi bi-chat-dots", Role = "admin", SortNumber = 35, ControllerName = "#", ActionName = "#", },
                    new MenuItem { Name = "Yorum Listesi", Icon = "bi bi-list-stars", AreaName = "Admin", ControllerName = "Comment", ActionName = "List", ParentId = 35, Role = "admin", SortNumber = 36 },
                    new MenuItem { Name = "Onay Bekleyenler", Icon = "bi bi-clock", AreaName = "Admin", ControllerName = "Comment", ActionName = "Pending", ParentId = 35, Role = "admin", SortNumber = 37 },
                    new MenuItem { Name = "Kullanıcı Yorumları", Icon = "bi bi-person-lines-fill", AreaName = "Admin", ControllerName = "Comment", ActionName = "UserComments", ParentId = 35, Role = "admin", SortNumber = 38 },

                    new MenuItem { Name = "Raporlar ve Analiz", Icon = "bi bi-file-earmark-bar-graph", Role = "admin", SortNumber = 39, ControllerName = "#", ActionName = "#", },
                    new MenuItem { Name = "Eğitim Analizi", Icon = "bi bi-graph-up", AreaName = "Admin", ControllerName = "Report", ActionName = "CourseAnalytics", ParentId = 39, Role = "admin", SortNumber = 40 },
                    new MenuItem { Name = "Kullanıcı Raporları", Icon = "bi bi-people", AreaName = "Admin", ControllerName = "Report", ActionName = "UserProgress", ParentId = 39, Role = "admin", SortNumber = 41 },
                    new MenuItem { Name = "Gelir Raporları", Icon = "bi bi-cash", AreaName = "Admin", ControllerName = "Report", ActionName = "Revenue", ParentId = 39, Role = "admin", SortNumber = 42 }
                };
                allMenuItems.AddRange(adminMenuItems);
                Console.WriteLine($"Admin menüleri eklendi. Toplam: {adminMenuItems.Length}");
                #endregion

                #region Instructor Menüleri
                var instructorMenuItems = new[]
                {
                    new MenuItem { Name = "Dashboard", Icon = "bi bi-speedometer2", AreaName = "Instructor", ControllerName = "Home", ActionName = "Index", Role = "instructor", SortNumber = 43, SubMenuItems = new List<MenuItem>() },

                    new MenuItem {  Name = "Profil Yönetimi", Icon = "bi bi-person-circle", Role = "instructor", SortNumber = 44, ControllerName = "#", ActionName = "#",},
                    new MenuItem {  Name = "Profil Bilgileri", Icon = "bi bi-person-lines-fill", AreaName = "Member", ControllerName = "UserEdit", ActionName = "Index", ParentId = 44, Role = "instructor", SortNumber = 45 },
                    new MenuItem { Name = "Şifre Değiştir", Icon = "bi bi-key", AreaName = "Instructor", ControllerName = "Profile", ActionName = "ChangePassword", ParentId = 44, Role = "instructor", SortNumber = 46 },

                    new MenuItem {  Name = "Eğitim Yönetimi", Icon = "bi bi-mortarboard", Role = "instructor", SortNumber = 47,ControllerName = "#", ActionName = "#", },
                    new MenuItem { Name = "Eğitimlerim", Icon = "bi bi-collection-play", AreaName = "Instructor", ControllerName = "Edu", ActionName = "EduList", ParentId = 47, Role = "instructor", SortNumber = 48 },
                    new MenuItem { Name = "Yeni Eğitim", Icon = "bi bi-plus-circle", AreaName = "Instructor", ControllerName = "Edu", ActionName = "EduCreate", ParentId = 47, Role = "instructor", SortNumber = 49 },
                    new MenuItem { Name = "İçerik Yönetimi", Icon = "bi bi-file-earmark-text", AreaName = "Instructor", ControllerName = "Edu", ActionName = "Content", ParentId = 47, Role = "instructor", SortNumber = 50 },
                    new MenuItem {  Name = "Öğrenci Etkileşimleri", Icon = "bi bi-people", AreaName = "Instructor", ControllerName = "Edu", ActionName = "StudentInteractions", ParentId = 47, Role = "instructor", SortNumber = 51 },

                    new MenuItem {  Name = "Sınav Yönetimi", Icon = "bi bi-journal-check", Role = "instructor", SortNumber = 52, ControllerName = "#", ActionName = "#",},
                    new MenuItem { Name = "Sınavlarım", Icon = "bi bi-list-check", AreaName = "Instructor", ControllerName = "Exam", ActionName = "List", ParentId = 52, Role = "instructor", SortNumber = 53 },
                    new MenuItem {  Name = "Sınav Oluştur", Icon = "bi bi-plus-circle", AreaName = "Instructor", ControllerName = "Exam", ActionName = "Create", ParentId = 52, Role = "instructor", SortNumber = 54 },
                    new MenuItem {  Name = "Soru Bankası", Icon = "bi bi-question-circle", AreaName = "Instructor", ControllerName = "Exam", ActionName = "Questions", ParentId = 52, Role = "instructor", SortNumber = 55 },
                    new MenuItem {  Name = "Sınav Ayarları", Icon = "bi bi-gear", AreaName = "Instructor", ControllerName = "Exam", ActionName = "Settings", ParentId = 52, Role = "instructor", SortNumber = 56 },
                    new MenuItem {  Name = "Öğrenci Sınav Sonuçları", Icon = "bi bi-clipboard-data", AreaName = "Instructor", ControllerName = "Exam", ActionName = "Results", ParentId = 52, Role = "instructor", SortNumber = 57 },

                    new MenuItem {  Name = "Yorum Yönetimi", Icon = "bi bi-chat-dots", Role = "instructor", SortNumber = 58, ControllerName = "#", ActionName = "#",},
                    new MenuItem {  Name = "Tüm Yorumlar", Icon = "bi bi-chat-text", AreaName = "Instructor", ControllerName = "Comment", ActionName = "List", ParentId = 58, Role = "instructor", SortNumber = 59 },
                    new MenuItem {  Name = "Yanıtlanmamış Yorumlar", Icon = "bi bi-reply", AreaName = "Instructor", ControllerName = "Comment", ActionName = "Pending", ParentId = 58, Role = "instructor", SortNumber = 60 },
                    new MenuItem {  Name = "Yorum Ayarları", Icon = "bi bi-gear", AreaName = "Instructor", ControllerName = "Comment", ActionName = "Settings", ParentId = 58, Role = "instructor", SortNumber = 61 },

                    new MenuItem {  Name = "Analiz ve Raporlar", Icon = "bi bi-graph-up", Role = "instructor", SortNumber = 62, ControllerName = "#", ActionName = "#",},
                    new MenuItem {  Name = "Eğitim Satış Analizi", Icon = "bi bi-cart-check", AreaName = "Instructor", ControllerName = "Analytics", ActionName = "Sales", ParentId = 62, Role = "instructor", SortNumber = 63 },
                    new MenuItem {  Name = "Öğrenci Başarı Durumu", Icon = "bi bi-graph-up-arrow", AreaName = "Instructor", ControllerName = "Analytics", ActionName = "StudentProgress", ParentId = 62, Role = "instructor", SortNumber = 64 },
                    new MenuItem {  Name = "Geri Bildirimler", Icon = "bi bi-star", AreaName = "Instructor", ControllerName = "Analytics", ActionName = "Feedback", ParentId = 62, Role = "instructor", SortNumber = 65 }
                };
                allMenuItems.AddRange(instructorMenuItems);
                Console.WriteLine($"Instructor menüleri eklendi. Toplam: {instructorMenuItems.Length}");
                #endregion

                #region Student Menüleri
                var studentMenuItems = new[]
                {
                    new MenuItem {  Name = "Dashboard", Icon = "bi bi-speedometer2", AreaName = "Student", ControllerName = "Home", ActionName = "Index", Role = "student", SortNumber = 66, SubMenuItems = new List<MenuItem>() },

                    new MenuItem {  Name = "Profil Yönetimi", Icon = "bi bi-person-circle", Role = "student", SortNumber = 67, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Profil Bilgileri", Icon = "bi bi-person-lines-fill", AreaName = "Member", ControllerName = "UserEdit", ActionName = "Index", ParentId = 67, Role = "student", SortNumber = 68 },
                    new MenuItem {  Name = "Şifre Değiştir", Icon = "bi bi-key", AreaName = "Student", ControllerName = "Profile", ActionName = "ChangePassword", ParentId = 67, Role = "student", SortNumber = 69 },

                    new MenuItem {  Name = "Eğitim İçeriği", Icon = "bi bi-collection-play", Role = "student", SortNumber = 70, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Kurslarım", Icon = "bi bi-play-circle", AreaName = "Student", ControllerName = "Course", ActionName = "MyCourses", ParentId = 70, Role = "student", SortNumber = 71 },
                    new MenuItem {  Name = "Ders İzleme", Icon = "bi bi-camera-video", AreaName = "Student", ControllerName = "Course", ActionName = "WatchLessons", ParentId = 70, Role = "student", SortNumber = 72 },
                    new MenuItem {  Name = "Eğitim Dokümanları", Icon = "bi bi-file-earmark-text", AreaName = "Student", ControllerName = "Course", ActionName = "Documents", ParentId = 70, Role = "student", SortNumber = 73 },
                    new MenuItem { Name = "Sınav ve Ödevler", Icon = "bi bi-journal-check", AreaName = "Student", ControllerName = "Course", ActionName = "Assignments", ParentId = 70, Role = "student", SortNumber = 74 },

                    new MenuItem { Name = "Sertifikalarım", Icon = "bi bi-award", Role = "student", SortNumber = 75, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "Tüm Sertifikalar", Icon = "bi bi-collection", AreaName = "Student", ControllerName = "Certificate", ActionName = "List", ParentId = 75, Role = "student", SortNumber = 76 },
                    new MenuItem {  Name = "Sertifika İndir", Icon = "bi bi-download", AreaName = "Student", ControllerName = "Certificate", ActionName = "Download", ParentId = 75, Role = "student", SortNumber = 77 },
                    new MenuItem {  Name = "E-Devlet Erişimi", Icon = "bi bi-box-arrow-up-right", AreaName = "Student", ControllerName = "Certificate", ActionName = "EDevlet", ParentId = 75, Role = "student", SortNumber = 78 },

                    new MenuItem {  Name = "Yorumlarım", Icon = "bi bi-chat-dots", Role = "student", SortNumber = 79, ControllerName = "#", ActionName = "#", },
                    new MenuItem { Name = "Yorum Yap", Icon = "bi bi-pencil", AreaName = "Student", ControllerName = "Comment", ActionName = "Create", ParentId = 79, Role = "student", SortNumber = 80 },
                    new MenuItem {  Name = "Yorumlarımı Görüntüle", Icon = "bi bi-chat-text", AreaName = "Student", ControllerName = "Comment", ActionName = "MyComments", ParentId = 79, Role = "student", SortNumber = 81 },

                    new MenuItem { Name = "Destek & Yardım", Icon = "bi bi-question-circle", Role = "student", SortNumber = 82, ControllerName = "#", ActionName = "#", },
                    new MenuItem {  Name = "SSS", Icon = "bi bi-info-circle", AreaName = "Student", ControllerName = "Help", ActionName = "Faq", ParentId = 82, Role = "student", SortNumber = 83 },
                    new MenuItem { Name = "Canlı Destek", Icon = "bi bi-headset", AreaName = "Student", ControllerName = "Help", ActionName = "LiveSupport", ParentId = 82, Role = "student", SortNumber = 84 }
                };
                allMenuItems.AddRange(studentMenuItems);
                Console.WriteLine($"Student menüleri eklendi. Toplam: {studentMenuItems.Length}");
                #endregion

                // Önce parent menüleri ekle
                var parentMenuItems = allMenuItems.Where(m => !m.ParentId.HasValue).ToList();
                await dbContext.MenuItems.AddRangeAsync(parentMenuItems);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Ana menüler eklendi. Toplam: {parentMenuItems.Count}");

                // Parent menülerin ID'lerini güncelle
                var menuDict = parentMenuItems.ToDictionary(m => m.SortNumber);

                // Şimdi child menüleri ekle ve ParentId'leri güncelle
                var childMenuItems = allMenuItems.Where(m => m.ParentId.HasValue).ToList();
                foreach (var child in childMenuItems)
                {
                    // Parent menünün yeni ID'sini bul ve ata
                    if (menuDict.TryGetValue(child.ParentId.Value, out var parent))
                    {
                        child.ParentId = parent.Id;
                    }
                }

                await dbContext.MenuItems.AddRangeAsync(childMenuItems);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Alt menüler eklendi. Toplam: {childMenuItems.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Menü seed işlemi sırasında hata oluştu: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}

