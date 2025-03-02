using System.Collections.Generic;
using System.Threading.Tasks;
using identity_signup.Areas.Instructor.Models;
using identity_signup.Areas.Instructor.ViewModels;

namespace identity_signup.Areas.Instructor.Services
{
    public interface IEducationServices
    {
        Task<bool> AddEducation(EduCreateViewModel model, string createdBy);
         Task<List<EduListViewModel>> GetAllEducations();
        Task<List<EduListViewModel>> GetInstructorEducations(string createdBy);
        Task<Education> GetEducationById(int id);
        Task<bool> UpdateEducation(EduUpdateViewModel model);
        Task<bool> DeleteEducation(int id);
    }
}
