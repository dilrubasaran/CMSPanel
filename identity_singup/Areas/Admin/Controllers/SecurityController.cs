using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace identity_singup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class SecurityController : Controller
    {
        private readonly AppDbContext _context;

        public SecurityController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> LoginStats(string timeRange = "daily")
        {
            var now = DateTime.Now;
            var query = _context.LoginAudits.AsQueryable();

            // Zaman aralığına göre filtreleme
            query = timeRange switch
            {
                "12hours" => query.Where(x => x.LoginTime >= now.AddHours(-12)),
                "daily" => query.Where(x => x.LoginTime >= now.Date),
                "weekly" => query.Where(x => x.LoginTime >= now.AddDays(-7)),
                _ => query.Where(x => x.LoginTime >= now.Date)
            };

            //Kullanıcı rolü ve başarı durumuna göre gruplama
            var stats = await query
                .GroupBy(x => new { x.UserRole, x.IsSuccess })
                .Select(g => new LoginStatViewModel
                {
                    UserRole = g.Key.UserRole ?? "Bilinmeyen",
                    IsSuccess = g.Key.IsSuccess,
                    Count = g.Count()
                })
                .ToListAsync();

            var viewModel = new LoginStatsViewModel
            {
                TimeRange = timeRange,
                Stats = stats,
                TotalLogins = stats.Sum(x => x.Count),
                SuccessfulLogins = stats.Where(x => x.IsSuccess).Sum(x => x.Count),
                FailedLogins = stats.Where(x => !x.IsSuccess).Sum(x => x.Count)
            };

            return View(viewModel);
        }


        // Giriş denemelerini listeler
        public async Task<IActionResult> LoginAttempts(string searchTerm, int page = 1)
        {
            const int pageSize = 20;
            var query = _context.LoginAudits.AsQueryable();


            //arama filtresi
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => 
                    x.UserName.Contains(searchTerm) || 
                    x.IpAddress.Contains(searchTerm) ||
                    x.UserRole.Contains(searchTerm));
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var loginAttempts = await query
                .OrderByDescending(x => x.LoginTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new LoginAttemptsViewModel
            {
                LoginAttempts = loginAttempts,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }
    }
} 