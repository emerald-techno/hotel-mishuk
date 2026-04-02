namespace WebMVC.Models.UserRole;

public class RolePermmissionCheck
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int? ParentId { get; set; }
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public IList<RolePermmissionCheck> Children { get; set; }

    public RolePermmissionCheck()
    {
        this.Children = new List<RolePermmissionCheck>();
    }
}
