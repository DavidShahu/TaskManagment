using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.DTOs
{
    public class AdminResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
