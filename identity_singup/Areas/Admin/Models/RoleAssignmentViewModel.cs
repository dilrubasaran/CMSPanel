public class RoleAssignmentViewModel
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public List<RoleAssignmentItemViewModel> Roles { get; set; } = new();
}

public class RoleAssignmentItemViewModel
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public bool IsAssigned { get; set; }
    public int PermissionLevel { get; set; }
} 