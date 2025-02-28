public class MenuItem
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public List<MenuItem> SubItems { get; set; } = new();
    public bool IsActive { get; set; }
} 