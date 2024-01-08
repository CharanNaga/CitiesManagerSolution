using CitiesManager.Core.DTO;
using CitiesManager.Core.Identities;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            // Creating a DateTime object representing the token expiration time by adding the number of minutes specified in the configuration to the current UTC time.
            DateTime expiration  = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"])
                ); 

            //Adding claims to be added in the payload or value or details representing particular user
            //Creating an array of Claim objects representing the user's claims, such as their ID, name, email, etc.
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject with an user Id value i.e., Subject (user id)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Unique Id (JWT Id) for the token i.e., JWT unique ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), //Issued at Date and Time of token generation i.e., Issued at (date and time of token generation)
                new Claim(ClaimTypes.NameIdentifier, user.Email), //Unique value of the particular user (considering email) i.e., Unique name identifier of the user (Email)
                new Claim(ClaimTypes.Name, user.PersonName) //Name of the User
            };

            //secret key ( Creating a SymmetricSecurityKey object using the key specified in the configuration.)
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                );

            //Employing Hashing Algorithm using security key
            // Creating a SigningCredentials object with the security key and the HMACSHA256 algorithm.
            SigningCredentials signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            //Token Generation Process
            // Creating a JwtSecurityToken object with the given issuer, audience, claims, expiration, and signing credentials.
            JwtSecurityToken securityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            //Generates final token
            // Creating a JwtSecurityTokenHandler object and use it to write the token as a string.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken( securityToken);

            //// Creating and return an AuthenticationResponse object containing the token, user email, user name, and token expiration time.
            return new AuthenticationResponse()
            {
                Token = token,
                PersonName = user.PersonName,
                Email = user.Email,
                Expiration = expiration
            };
        }
    }
}
