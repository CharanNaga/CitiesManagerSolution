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
    [Route("api/[controller]")]
    [ApiController]
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
            return await _db.Cities.ToListAsync();
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(Guid id)
        {
          if (_db.Cities == null)
          {
              return NotFound();
          }
            var city = await _db.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(Guid id, City city)
        {
            if (id != city.CityID)
            {
                return BadRequest();
            }

            _db.Entry(city).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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
        public async Task<ActionResult<City>> PostCity(City city)
        {
          if (_db.Cities == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Cities'  is null.");
          }
            _db.Cities.Add(city);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.CityID }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            if (_db.Cities == null)
            {
                return NotFound();
            }
            var city = await _db.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid id)
        {
            return (_db.Cities?.Any(e => e.CityID == id)).GetValueOrDefault();
        }
    }
}
