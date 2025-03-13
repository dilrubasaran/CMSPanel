using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using identity_singup.Models;

public class MenuAddViewModel
{
    [Required(ErrorMessage = "Menü adı zorunludur")]
    public string Name { get; set; }

    [Required(ErrorMessage = "İcon seçimi zorunludur")]
    public string Icon { get; set; }

    [Required(ErrorMessage = "Controller name seçimi zorunludur")]
    public string ControllerName { get; set; }

    [Required(ErrorMessage = "Action name seçimi zorunludur")]
    public string ActionName { get; set; }
    public string AreaName { get; set; }
    public int? ParentId { get; set; }
    
    [Required(ErrorMessage = "Rol seçimi zorunludur")]
    public string Role { get; set; }
    
    //Todo: sort numberi nasıl otomatik atarım ?
    public int SortNumber { get; set; }

    // Dropdown listeler için
    public IEnumerable<MenuItem> ParentMenus { get; set; }
    public IEnumerable<string> Roles { get; set; }
} 