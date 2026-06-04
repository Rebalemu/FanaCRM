using Microsoft.AspNetCore.Mvc.Rendering;

public class EditRoleVM
{
    public string? UserId { get; set; }
    public string? Email { get; set; }

    public List<string> SelectedRoles { get; set; } = new List<string>();

    public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>(); // ✅ FIX
}