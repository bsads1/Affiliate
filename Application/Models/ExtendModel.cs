using System.ComponentModel.DataAnnotations;
using Spark.Library.Database;

namespace Affiliate.Application.Models;

public class ExtendModel : BaseModel
{
    [Required] public Guid Guid { get; set; } = Guid.NewGuid();

    public Guid CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }
}