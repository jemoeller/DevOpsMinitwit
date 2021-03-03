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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_id", TypeName = "integer")]
        public int UserId { get; set; }
        [Required]
        [Column("username", TypeName = "string")]
        public string Username { get; set; }
        [Required]
        [Column("email", TypeName = "string")]
        public string Email { get; set; }
        [Required]
        [Column("pw_hash", TypeName = "string")]
        public string PwHash { get; set; }
    }
}
