using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Data.Db;

[Table("live")]
public partial class Live
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("guid")]
    public Guid Guid { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("slug", TypeName = "character varying")]
    public string Slug { get; set; } = null!;

    [Column("image", TypeName = "character varying")]
    public string? Image { get; set; }

    [Column("embed", TypeName = "character varying")]
    public string? Embed { get; set; }

    [Column("winner")]
    public int Winner { get; set; }

    [Column("num")]
    public int Num { get; set; }

    [Column("ratio", TypeName = "character varying")]
    public string? Ratio { get; set; }

    [Column("isend")]
    public bool Isend { get; set; }

    [Column("isdelete")]
    public bool Isdelete { get; set; }

    [Column("enddate", TypeName = "timestamp without time zone")]
    public DateTime? Enddate { get; set; }

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime Createdate { get; set; }

    [Column("updatedate", TypeName = "timestamp without time zone")]
    public DateTime? Updatedate { get; set; }

    [Column("createby")]
    public Guid Createby { get; set; }

    [Column("updateby")]
    public Guid? Updateby { get; set; }
}
