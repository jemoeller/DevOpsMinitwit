using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MiniTwit.Entities
{
    [Table("user")]
    public partial class User
    {
        [Key]
        [Column("user_id", TypeName = "integer")]
        public long UserId { get; set; }
        [Required]
        [Column("username", TypeName = "string")]
        public byte[] Username { get; set; }
        [Required]
        [Column("email", TypeName = "string")]
        public byte[] Email { get; set; }
        [Required]
        [Column("pw_hash", TypeName = "string")]
        public byte[] PwHash { get; set; }
    }
}
