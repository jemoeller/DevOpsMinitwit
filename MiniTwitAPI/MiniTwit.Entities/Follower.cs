using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MiniTwit.Entities
{
    [Keyless]
    [Table("follower")]
    public partial class Follower
    {
        [Column("who_id", TypeName = "integer")]
        public int WhoId { get; set; }
        [Column("whom_id", TypeName = "integer")]
        public int WhomId { get; set; }
    }
}
