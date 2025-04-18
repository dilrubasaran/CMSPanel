using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using identity_singup.Models;

public class MenuAddViewModel
{
    [Required(ErrorMessage = "Menü adı zorunludur")]
    [Display(Name = "Menu Adı")]
    public string Name { get; set; }

    [Required(ErrorMessage = "İcon seçimi zorunludur")]
    [Display(Name = "Icon")]
    public string Icon { get; set; }

    [Required(ErrorMessage = "Controller name seçimi zorunludur")]
    [Display(Name = "Controller Adı")]
    public string ControllerName { get; set; }

    [Required(ErrorMessage = "Action name seçimi zorunludur")]
    [Display(Name = "Action Adı")]
    public string ActionName { get; set; }

    [Display(Name = "Area Adı")]
    public string AreaName { get; set; }

    public int? ParentId { get; set; }
    
    [Required(ErrorMessage = "Rol seçimi zorunludur")]
    [Display(Name = "Rol")]
    public string Role { get; set; }

    //Todo: sort numberi nasıl otomatik atarım ?
    [Display(Name = "Sıra No")]
    public int SortNumber { get; set; }

    // Dropdown listeler için
    public IEnumerable<MenuItem> ParentMenus { get; set; }
    public IEnumerable<string> Roles { get; set; }
} 