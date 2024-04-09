using Spark.Library.Database;

namespace Affiliate.Application.Models;

public class Role : ExtendModel
{
    public Role()
    {
        UserRoles = new HashSet<UserRole>();
    }

    public string Name { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
}