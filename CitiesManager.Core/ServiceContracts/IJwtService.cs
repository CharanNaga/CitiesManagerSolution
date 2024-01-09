using CitiesManager.Core.DTO;
using CitiesManager.Core.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        //Based on the existing user details, generate the JWT Token & return as a response to the client along with the username, email & expiration
        AuthenticationResponse CreateJwtToken(ApplicationUser user);

        //Represents User details
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
