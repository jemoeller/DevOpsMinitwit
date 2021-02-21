using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MiniTwit.Models
{
    public class UserCreateDTO
    {
        [Required]
        public string Username;

        [Required]
        public string Password;

        [Required]
        public string Email;
    }
}
