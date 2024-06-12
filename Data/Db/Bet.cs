using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Data.Db;

[Table("bet")]
public partial class Bet
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("guid")]
    public Guid Guid { get; set; }

    [Column("guidlive")]
    public Guid Guidlive { get; set; }

    [Column("guidusercreated")]
    public Guid Guidusercreated { get; set; }

    [Column("guiduserjoined")]
    public Guid? Guiduserjoined { get; set; }

    [Column("player")]
    public int Player { get; set; }

    [Column("points")]
    public long Points { get; set; }

    [Column("ratioin")]
    public int Ratioin { get; set; }

    [Column("ratioout")]
    public int Ratioout { get; set; }

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime Createdate { get; set; }

    [Column("ispaid")]
    public bool Ispaid { get; set; }
}
