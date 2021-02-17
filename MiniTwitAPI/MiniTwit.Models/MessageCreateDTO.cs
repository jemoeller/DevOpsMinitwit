using System;
using System.ComponentModel.DataAnnotations;

namespace MiniTwit.Models
{
    public class MessageCreateDTO
    {
        [Required]
        public long AuthorId { get; set; }
        public string Text { get; set; }
    }
}
