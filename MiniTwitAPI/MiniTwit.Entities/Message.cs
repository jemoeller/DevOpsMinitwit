using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MiniTwit.Entities
{
    [Table("message")]
    public partial class Message
    {
        [Key]
        [Column("message_id", TypeName = "integer")]
        public long MessageId { get; set; }
        [Column("author_id", TypeName = "integer")]
        public long AuthorId { get; set; }
        [Required]
        [Column("text", TypeName = "string")]
        public byte[] Text { get; set; }
        [Column("pub_date", TypeName = "integer")]
        public long? PubDate { get; set; }
        [Column("flagged", TypeName = "integer")]
        public long? Flagged { get; set; }
    }
}
