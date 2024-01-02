using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.WebAPI.DatabaseContext;
using CitiesManager.WebAPI.Models;

namespace CitiesManager.WebAPI.Controllers
{
    [Route("api/[controller]")] //If commented, throws exception as every web api controller should have route
    [ApiController] //If this attribute is commented, then wherever we want to read JsonData from Req.Body we need to write [FromBody] explicitly before. If any model state errors appears, then it automatically redirects to bad request page
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CitiesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
          if (_db.Cities == null)
          {
              return NotFound();
          }
            return await _db.Cities.OrderByDescending(temp => temp.CityName).ToListAsync();
        }

        // GET: api/Cities/5
        [HttpGet("{cityID}")] //HttpGet along with the Route Parameter
        public async Task<ActionResult<City>> GetCity(Guid cityID)
        {
          if (_db.Cities == null)
          {
              return NotFound();
          }
            //var city = await _db.Cities.FindAsync(cityID);
            var city = await _db.Cities.FirstOrDefaultAsync(temp => temp.CityID == cityID);

            if (city == null)
            {
                return NotFound(); //Response.StatusCode = 404
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{cityID}")]
        public async Task<IActionResult> PutCity(Guid cityID, [Bind(nameof(City.CityID),nameof(City.CityName))] City city) //To avoid OverPosting of properties, better to use [Bind] for required Properties we want to include in Model Binding
        {
            if (cityID != city.CityID)
            {
                return BadRequest();
            }

            //_db.Entry(city).State = EntityState.Modified;
            var existingCity = await _db.Cities.FindAsync(cityID);
            if(existingCity == null)
            {
                return NotFound();
            }

            existingCity.CityName = city.CityName;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) //this exception occurs, if the same obj is already updated by another
            {
                if (!CityExists(cityID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity([Bind(nameof(City.CityID),nameof(City.CityName))] City city)
        {
          if (_db.Cities == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Cities'  is null.");
          }
          _db.Cities.Add(city);
          await _db.SaveChangesAsync();

          return CreatedAtAction("GetCity", new { cityID = city.CityID }, city); //it returns 201 status code and add a response header 'location' with the url api/cities/newlygeneratedid & gives response. third parameter city object represents response.
        }

        // DELETE: api/Cities/5
        [HttpDelete("{cityID}")]
        public async Task<IActionResult> DeleteCity(Guid cityID)
        {
            if (_db.Cities == null)
            {
                return NotFound();
            }
            var city = await _db.Cities.FindAsync(cityID);
            if (city == null)
            {
                return NotFound();
            }

            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid cityID)
        {
            return (_db.Cities?.Any(e => e.CityID == cityID)).GetValueOrDefault();
        }
    }
}
