using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using identity_signup.Areas.Instructor.Models;
using identity_signup.Areas.Instructor.ViewModels;
using identity_singup.Models;


namespace identity_signup.Areas.Instructor.Services
{
    public class EducationService : IEducationServices
    {
        private readonly AppDbContext _context;

        public EducationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddEducation(EduCreateViewModel model, string createdBy)
        {
            try
            {
                var education = new Education
                {
                    EduName = model.EduName,
                    Description = model.Description,
                    EduType = model.EduType,
                    EduDuration = model.EduDuration,
                    EduPrice = model.EduPrice,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now
                };

                await _context.Education.AddAsync(education);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<EduListViewModel>> GetAllEducations()
        {
            return await _context.Education
                .Select(x => new EduListViewModel
                {
                    Id = x.Id,
                    EduName = x.EduName,
                    Description = x.Description,
                    EduType = x.EduType,
                    EduDuration = x.EduDuration,
                    EduPrice = x.EduPrice,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Education>> GetInstructorEducations(string createdBy)
        {
            return await _context.Education
                .Where(x => x.CreatedBy == createdBy)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Education> GetEducationById(int id)
        {
            return await _context.Education
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateEducation(EduUpdateViewModel model)
        {
            try
            {
                var education = await _context.Education.FindAsync(model.Id);
                if (education == null) return false;

                education.EduName = model.EduName;
                education.Description = model.Description;
                education.EduType = model.EduType;
                education.EduDuration = model.EduDuration;
                education.EduPrice = model.EduPrice;

                _context.Education.Update(education);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEducation(int id)
        {
            try
            {
                var education = await _context.Education.FindAsync(id);
                if (education == null) return false;

                _context.Education.Remove(education);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
