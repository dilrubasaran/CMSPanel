# CMSPanel – Çok Rollü .NET MVC Eğitim Yönetim Paneli


**CMSPanel**, çok rollü bir kullanıcı yönetim sistemi sunan ve eğitim içeriklerinin yönetimini sağlayan bir web tabanlı CMS panelidir. Proje, `ASP.NET MVC`, `Entity Framework`, `Role-Based Authorization`, `Custom Filters` ve servis katmanları gibi ileri düzey yazılım mimarisi bileşenleri kullanılarak geliştirilmiştir.

## 🎯 Proje Amacı

Bu proje, özellikle **eğitim yönetimi** için geliştirilmiş olup; **Root Admin**, ** Admin**, **Instructor (Eğitmen)** ve **Student (Öğrenci)** rollerine özel dinamik bir yönetim paneli sunar. Her kullanıcı yalnızca yetkili olduğu sayfalara erişebilir. Rol bazlı **Sidebar menü**, **yetki kontrolleri**, **gelişmiş loglama**, **form onay süreçleri** gibi özellikleri içerir.

---

## 🧩 Roller ve Yetkiler

| Rol         | Açıklama                                                                 |
|-------------|--------------------------------------------------------------------------|
| `Root Admin`| Sistemin en yetkili kullanıcısı. Tüm verilere ve ayarlara erişebilir.    |
| `Admin`     | Eğitim yönetimi, giriş loglarını görüntüleme, instructor formlarını onaylama gibi işlemleri yapabilir. |
| `Instructor`| Eğitim ekleyebilir, düzenleme isteği gönderebilir. Gün sayısı 7’yi geçtiyse tekrar düzenleme için form ile başvuru yapar. |
| `Student`   | Profil düzenleme ve dashboard görüntüleme işlemleri yapabilir. İleride daha fazla yetki ve özellik eklenecektir. |

---

## 🧠 Temel Özellikler

- ✅ Rol bazlı yetkilendirme (Authorization)
- 🧩 Dinamik Sidebar (kullanıcının rolüne göre menü)
- 🗃️ Eğitim ekleme, görüntüleme, düzenleme, silme işlemleri
- 📄 Instructor güncelleme talepleri ve form onay süreci
- 📱 Telefon numarası olmayan kullanıcıların belirli sayfalara erişimini engelleyen filtre yapısı
- 🔐 Giriş loglarının kaydı
- 🗂 Entity Framework ile veritabanı yönetimi
- 📊 Ortak Dashboard yapısı (CoreUI temasıyla geliştirildi)
- 🧷 Unobtrusive Validation ile kullanıcı dostu formlar ve anlık uyarılar

---

## 🔐 Yetkilendirme Yönetimi

- Admin, sadece kendisinden **daha düşük yetkili** rolleri (Instructor, Student) atayabilir.
- Admin, başka bir kullanıcıdan **Admin yetkisini kaldıramaz** — bu işlemi yalnızca **Root Admin** gerçekleştirebilir.
- Yeni bir rol tanımlama işlemi yalnızca **Root Admin** tarafından yapılabilir.
- Yetkisiz bir kullanıcı bu tür işlemleri denediğinde, sistem **“Access Denied”** yanıtı verir ve işlem **loglanır**.

---
## 🧾 Identity & Claims Tabanlı Yapı

- ASP.NET Identity altyapısı kullanıldı.
- Kullanıcılara rol ve ek bilgiler (telefon onayı, aktiflik vb.) claim olarak atandı.
- Policy-based authorization ile erişim kontrolleri yapıldı (örneğin, sadece telefonu doğrulanmış kullanıcılar bazı sayfalara erişebilir).
- Gelişmiş kontroller için custom filter’lar ve middleware yapıları kullanıldı.
---

## 📊 Loglama ve Güvenlik

- **Başarılı / Başarısız giriş** denemeleri günlük olarak ve 12 saatlik periyotlarla raporlanır.
- **İşlem logları**, kullanıcıların hangi cihazdan ne zaman giriş yaptığı gibi detayları içerir.
- Kullanıcılara **aktif/pasif** durumu atanabilir:
  - Pasif durumdaki kullanıcılar sisteme giriş yapamaz.
  - Giriş yapabilmesi için tekrar aktif hale getirilmesi gerekir.
- Kullanıcılar ayrıca “Kullanıcıyı Görüntüle” ekranından detaylı olarak incelenebilir.

---

## 💻 Dashboard Yapısı

- Tüm roller için ortak bir **Dashboard** yapısı mevcuttur.
- Dashboard tasarımı **CoreUI** teması entegre edilerek geliştirilmiştir.
- Kullanıcı deneyimi ve arayüz kalitesi ön planda tutulmuştur.

---

## ✅ Form Doğrulama (Validation)

- `Unobtrusive Validation` desteklidir.
- `ViewModel` üzerindeki **Data Annotation** kontrolleri sayesinde kullanıcılar anlık olarak form hatalarını görebilir.
- Hatalar otomatik olarak client tarafında gösterilir, kullanıcı deneyimi iyileştirilmiştir.

---
## 🎯 UserService ile Merkezi Hata Yönetimi

Proje, `TS.Result` kütüphanesi ile geliştirilmiş özel validasyon mesajları sağlayan `UserService` katmanını içeriyor. Bu sayede, DataAnnotations ile ifade edilemeyen hata durumları servis katmanına taşınarak, hataların merkezi bir şekilde yönetilmesi sağlandı. 

- **Özellikler**:
  - `TS.Result` yapısı kullanılarak hata mesajları daha özelleştirilmiş hale getirildi.
  - Validation işlemleri Controller seviyesine taşındı, bu sayede sadece `ValidationResult.IsSuccessful` kontrolü ile sade ve merkezi hata yönetimi sağlandı.
  - Bu özellik, yazılımın bakımını ve hata takibini daha verimli hale getiriyor.

---
## 🔧 Kullanılan Teknolojiler

- `.NET MVC`
- `Entity Framework`
- `C#`
- `Razor`
- `Bootstrap`
- `CoreUI`
- `JavaScript`
- `SQL Server`

## 🖼️ Proje Ekran Görüntüleri

![Ekran görüntüsü 2025-05-08 121440](https://github.com/user-attachments/assets/291b877d-c092-4b32-95bd-632660aba6c0)

## 📊 Dashboard Görünümü (Admin)
![admin-dasboard png](https://github.com/user-attachments/assets/1114b38a-c17a-4c3e-8b2c-488516276427)

## 📊 Dashboard Görünümü (Instructor)
![image](https://github.com/user-attachments/assets/3bbdcdee-7cb6-4aff-909e-a32a5fb22814)

### 🔐 Giriş İstatistikleri
![image](https://github.com/user-attachments/assets/48c07766-1c6b-4d9f-93a6-9f6b3dfe3aa1)


### 📑 Eğitim Kart Yapısı
![Ekran görüntüsü 2025-05-08 123017](https://github.com/user-attachments/assets/f908572d-f347-4b2f-a2ca-3a310ed65516)

