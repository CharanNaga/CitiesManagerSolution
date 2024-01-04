using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.WebAPI.DatabaseContext;
using CitiesManager.WebAPI.Models;

namespace CitiesManager.WebAPI.Controllers.v2
{
    //[Route("api/[controller]")] //If commented, throws exception as every web api controller should have route
    //[ApiController] //If this attribute is commented, then wherever we want to read JsonData from Req.Body we need to write [FromBody] explicitly before. If any model state errors appears, then it automatically redirects to bad request page
    [ApiVersion("2.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CitiesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/Cities
        /// <summary>
        /// Returns List of City Names retrieved from 'Cities' Table
        /// </summary>
        /// <returns>List of City Names</returns>
        [HttpGet]
        //[Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<string?>>> GetCities() //Modifying requirement such that in v2, we need only GetCities() and that should return only CityNames
        {
            if (_db.Cities == null)
            {
                return NotFound();
            }
            return await _db.Cities.
                OrderBy(c=>c.CityName)
                .Select(c=>c.CityName)
                .ToListAsync();
        }
    }
}
