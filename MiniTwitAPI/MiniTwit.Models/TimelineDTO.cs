using System;
using MiniTwit.Entities;
namespace MiniTwit.Models
{
    public class TimelineDTO
    {
        public Message message { get; set; }
        
        public User user { get; set; }
    }
}