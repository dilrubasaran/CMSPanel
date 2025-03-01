using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using identity_singup.Models;

namespace identity_singup.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public SidebarViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (user == null) return View(new List<MenuItem>());

            var roles = await _userManager.GetRolesAsync(user);
            var menuItems = GetMenuItemsByRole(roles);

            // Aktif menü öğesini belirle
            var currentUrl = HttpContext.Request.Path;
            SetActiveMenuItem(menuItems, currentUrl);

            return View(menuItems);
        }

        private void SetActiveMenuItem(List<MenuItem> MenuItems, string currentUrl)
        {
            foreach (var item in MenuItems)
            {
                item.IsActive = item.Url?.Equals(currentUrl.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
                if (item.SubItems?.Any() == true)
                {
                    SetActiveMenuItem(item.SubItems, currentUrl);
                }
            }
        }

        private List<MenuItem> GetMenuItemsByRole(IList<string> roles)
        {
            var menuItems = new List<MenuItem>();

            if (roles.Contains("admin"))
            {
                menuItems.AddRange(new[]
                {
                    new MenuItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "/Admin/Home/Index" },
                    
                    // 1. Kullanıcı Yönetimi
                    new MenuItem 
                    { 
                        Title = "Kullanıcı Yönetimi", 
                        Icon = "bi bi-people",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Kullanıcı Listesi", Icon = "bi bi-person-lines-fill", Url = "/Admin/Home/UserList" },
                            new MenuItem { Title = "Kullanıcı Ekleme", Icon = "bi bi-person-plus", Url = "/Admin/User/Create" },
                            new MenuItem { Title = "Onay Bekleyenler", Icon = "bi bi-person-check", Url = "/Admin/User/PendingApprovals" },
                            new MenuItem { Title = "Aktif/Pasif Kullanıcılar", Icon = "bi bi-toggle-on", Url = "/Admin/User/Status" },
                            
                        }
                    },

                    new MenuItem { Title = "Rol Yönetimi", Icon = "bi bi-shield-lock", Url = "/Admin/Role/Index" },

                    // 2. Eğitim Yönetimi
                    new MenuItem 
                    { 
                        Title = "Eğitim Yönetimi", 
                        Icon = "bi bi-mortarboard",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Eğitim Listesi", Icon = "bi bi-list-ul", Url = "/Instructor/Edu/EduList" },
                            new MenuItem { Title = "Eğitim Ekle", Icon = "bi bi-plus-circle", Url = "/Instructor/Edu/EduCreate" },
                            new MenuItem { Title = "Eğitim Güncelle", Icon = "bi bi-pencil-square", Url = "/Instructor/Edu/EduUpdate" }
                        }
                    },

                    // 3. Kategori Yönetimi
                    new MenuItem 
                    { 
                        Title = "Kategori Yönetimi", 
                        Icon = "bi bi-diagram-2",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Kategori Listesi", Icon = "bi bi-list-check", Url = "/Admin/Category/List" },
                            new MenuItem { Title = "Kategori Ekle", Icon = "bi bi-plus-circle", Url = "/Admin/Category/Create" },
                            new MenuItem { Title = "Kategori Güncelle", Icon = "bi bi-pencil", Url = "/Admin/Category/Update" },
                            new MenuItem { Title = "Eğitim Gruplandırma", Icon = "bi bi-grid", Url = "/Admin/Category/GroupCourses" }
                        }
                    },

                    // 4. Güvenlik Yönetimi
                    new MenuItem 
                    { 
                        Title = "Güvenlik Yönetimi", 
                        Icon = "bi bi-shield-check",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Giriş İstatistikleri", Icon = "bi bi-graph-up", Url = "/Admin/Security/LoginStats" },
                            new MenuItem { Title = "Tehdit İçeren Giriş İşlemler", Icon = "bi bi-exclamation-triangle", Url = "/Admin/Security/Threats" },
                            new MenuItem { Title = "İşlem Logları", Icon = "bi bi-activity", Url = "/Admin/Security/ActivityLogs" }
                        }
                    },

                    // 5. Ödeme ve Satış Yönetimi
                    new MenuItem 
                    { 
                        Title = "Ödeme ve Satış", 
                        Icon = "bi bi-cash-stack",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Satış Raporları", Icon = "bi bi-graph-up", Url = "/Admin/Payment/SalesReport" },
                            new MenuItem { Title = "Ödeme Durumu", Icon = "bi bi-credit-card", Url = "/Admin/Payment/Status" },
                            new MenuItem { Title = "İade Yönetimi", Icon = "bi bi-arrow-return-left", Url = "/Admin/Payment/Refunds" },
                            new MenuItem { Title = "Ödeme Onaylama", Icon = "bi bi-check-circle", Url = "/Admin/Payment/Approve" }
                        }
                    },

                    // 6. Bildirim ve Duyuru
                    new MenuItem 
                    { 
                        Title = "Bildirim Yönetimi", 
                        Icon = "bi bi-bell",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Genel Duyurular", Icon = "bi bi-megaphone", Url = "/Admin/Notification/Announcements" },
                            new MenuItem { Title = "Özel Bildirimler", Icon = "bi bi-envelope", Url = "/Admin/Notification/Custom" },
                            new MenuItem { Title = "Sistem Bildirimleri", Icon = "bi bi-gear", Url = "/Admin/Notification/System" }
                        }
                    },

                    // 7. Form Yönetimi
                    new MenuItem 
                    { 
                        Title = "Form Yönetimi", 
                        Icon = "bi bi-file-text",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "İstek Formları", Icon = "bi bi-chat-right-text", Url = "/Admin/Form/Requests" },
                            new MenuItem { Title = "Şikayet Formları", Icon = "bi bi-exclamation-circle", Url = "/Admin/Form/Complaints" },
                            new MenuItem { Title = "Öneri Formları", Icon = "bi bi-lightbulb", Url = "/Admin/Form/Suggestions" },
                            new MenuItem { Title = "Form Yönetimi", Icon = "bi bi-gear", Url = "/Admin/Form/Management" }
                        }
                    },

                    // 8. Yorum Yönetimi
                    new MenuItem 
                    { 
                        Title = "Yorum Yönetimi", 
                        Icon = "bi bi-chat-dots",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Yorum Listesi", Icon = "bi bi-list-stars", Url = "/Admin/Comment/List" },
                            new MenuItem { Title = "Onay Bekleyenler", Icon = "bi bi-clock", Url = "/Admin/Comment/Pending" },
                            new MenuItem { Title = "Kullanıcı Yorumları", Icon = "bi bi-person-lines-fill", Url = "/Admin/Comment/UserComments" }
                        }
                    },

                    // 9. Raporlama ve Analiz
                    new MenuItem 
                    { 
                        Title = "Raporlar ve Analiz", 
                        Icon = "bi bi-file-earmark-bar-graph",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Eğitim Analizi", Icon = "bi bi-graph-up", Url = "/Admin/Report/CourseAnalytics" },
                            new MenuItem { Title = "Kullanıcı Raporları", Icon = "bi bi-people", Url = "/Admin/Report/UserProgress" },
                            new MenuItem { Title = "Gelir Raporları", Icon = "bi bi-cash", Url = "/Admin/Report/Revenue" },
                          
                        }
                    }
                });
            }

            if (roles.Contains("instructor"))
            {
                menuItems.AddRange(new[]
                {
                    new MenuItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "/Instructor/Home/Index" },
                    
                    // 1. Profil Yönetimi
                    new MenuItem 
                    { 
                        Title = "Profil Yönetimi", 
                        Icon = "bi bi-person-circle",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Profil Bilgileri", Icon = "bi bi-person-lines-fill", Url = "/Member/UserEdit" },
                            new MenuItem { Title = "Şifre Değiştir", Icon = "bi bi-key", Url = "/Instructor/Profile/ChangePassword" }
                        }
                    },

                    // 2. Eğitim Yönetimi
                    new MenuItem 
                    { 
                        Title = "Eğitim Yönetimi", 
                        Icon = "bi bi-mortarboard",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Eğitimlerim", Icon = "bi bi-collection-play", Url = "/Instructor/Edu/EduList" },
                            new MenuItem { Title = "Yeni Eğitim", Icon = "bi bi-plus-circle", Url = "/Instructor/Edu/EduCreate" },
                            new MenuItem { Title = "İçerik Yönetimi", Icon = "bi bi-file-earmark-text", Url = "/Instructor/Edu/Content" },
                            new MenuItem { Title = "Öğrenci Etkileşimleri", Icon = "bi bi-people", Url = "/Instructor/Edu/StudentInteractions" }
                        }
                    },

                    // 3. Sınav Yönetimi
                    new MenuItem 
                    { 
                        Title = "Sınav Yönetimi", 
                        Icon = "bi bi-journal-check",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Sınavlarım", Icon = "bi bi-list-check", Url = "/Instructor/Exam/List" },
                            new MenuItem { Title = "Sınav Oluştur", Icon = "bi bi-plus-circle", Url = "/Instructor/Exam/Create" },
                            new MenuItem { Title = "Soru Bankası", Icon = "bi bi-question-circle", Url = "/Instructor/Exam/Questions" },
                            new MenuItem { Title = "Sınav Ayarları", Icon = "bi bi-gear", Url = "/Instructor/Exam/Settings" },
                            new MenuItem { Title = "Öğrenci Sınav Sonuçları", Icon = "bi bi-clipboard-data", Url = "/Instructor/Exam/Results" }
                        }
                    },

                    // 4. Yorum ve Geri Bildirim
                    new MenuItem 
                    { 
                        Title = "Yorum Yönetimi", 
                        Icon = "bi bi-chat-dots",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Tüm Yorumlar", Icon = "bi bi-chat-text", Url = "/Instructor/Comment/List" },
                            new MenuItem { Title = "Yanıtlanmamış Yorumlar", Icon = "bi bi-reply", Url = "/Instructor/Comment/Pending" },
                            new MenuItem { Title = "Yorum Ayarları", Icon = "bi bi-gear", Url = "/Instructor/Comment/Settings" }
                        }
                    },

                    // 5. Analiz ve Raporlama
                    new MenuItem 
                    { 
                        Title = "Analiz ve Raporlar", 
                        Icon = "bi bi-graph-up",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Eğitim Satış Analizi", Icon = "bi bi-cart-check", Url = "/Instructor/Analytics/Sales" },
                            new MenuItem { Title = "Öğrenci Başarı Durumu", Icon = "bi bi-graph-up-arrow", Url = "/Instructor/Analytics/StudentProgress" },
                            new MenuItem { Title = "Geri Bildirimler", Icon = "bi bi-star", Url = "/Instructor/Analytics/Feedback" }
                        }
                    }
                });
            }

            if (roles.Contains("student"))
            {
                menuItems.AddRange(new[]
                {
                    new MenuItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "/Student/Home/Index" },
                    
                    // 1. Profil Yönetimi
                    new MenuItem 
                    { 
                        Title = "Profil Yönetimi", 
                        Icon = "bi bi-person-circle",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Profil Bilgileri", Icon = "bi bi-person-lines-fill", Url = "/Member/UserEdit" },
                            new MenuItem { Title = "Şifre Değiştir", Icon = "bi bi-key", Url = "/Student/Profile/ChangePassword" }
                        }
                    },

                    // 2. Eğitim İçeriği
                    new MenuItem 
                    { 
                        Title = "Eğitim İçeriği", 
                        Icon = "bi bi-collection-play",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Kurslarım", Icon = "bi bi-play-circle", Url = "/Student/Course/MyCourses" },
                            new MenuItem { Title = "Ders İzleme", Icon = "bi bi-camera-video", Url = "/Student/Course/WatchLessons" },
                            new MenuItem { Title = "Eğitim Dokümanları", Icon = "bi bi-file-earmark-text", Url = "/Student/Course/Documents" },
                            new MenuItem { Title = "Sınav ve Ödevler", Icon = "bi bi-journal-check", Url = "/Student/Course/Assignments" }
                        }
                    },

                    // 3. Sertifikalar
                    new MenuItem 
                    { 
                        Title = "Sertifikalarım", 
                        Icon = "bi bi-award",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Tüm Sertifikalar", Icon = "bi bi-collection", Url = "/Student/Certificate/List" },
                            new MenuItem { Title = "Sertifika İndir", Icon = "bi bi-download", Url = "/Student/Certificate/Download" },
                            new MenuItem { Title = "E-Devlet Erişimi", Icon = "bi bi-box-arrow-up-right", Url = "/Student/Certificate/EDevlet" }
                        }
                    },

                    // 4. İstatistik ve Analizler
                    new MenuItem 
                    { 
                        Title = "İstatistiklerim", 
                        Icon = "bi bi-graph-up",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Sınav Sonuçları", Icon = "bi bi-clipboard-data", Url = "/Student/Statistics/ExamResults" },
                            new MenuItem { Title = "Kurs Başarı Durumu", Icon = "bi bi-bar-chart-line", Url = "/Student/Statistics/CourseProgress" },
                            new MenuItem { Title = "Genel Performans", Icon = "bi bi-graph-up-arrow", Url = "/Student/Statistics/Performance" }
                        }
                    },

                    // 5. Destek ve İletişim
                    new MenuItem 
                    { 
                        Title = "Destek", 
                        Icon = "bi bi-headset",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Soru & Cevap", Icon = "bi bi-chat-dots", Url = "/Student/Support/QA" },
                            new MenuItem { Title = "Geri Bildirim", Icon = "bi bi-chat-quote", Url = "/Student/Support/Feedback" },
                            new MenuItem { Title = "Yardım Merkezi", Icon = "bi bi-info-circle", Url = "/Student/Support/Help" }
                        }
                    }
                });
            }

            return menuItems;
        }
    }
} 