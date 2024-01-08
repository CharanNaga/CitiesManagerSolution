using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.DTO
{
    // Class that contains Properties to include as a response after successful login
    public class AuthenticationResponse 
    {
        public string? PersonName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}
