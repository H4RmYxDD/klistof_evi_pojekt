using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;

namespace DeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, Package> Packages = new();
        private static readonly string[] Statuses = { "Fogadott", "Kiszállítás alatt", "Kiszállítva" };
        private static readonly Random Random = new();

        [HttpPost("receive")]
        public IActionResult ReceivePackage([FromBody] Package package)
        {
            if (Random.NextDouble() < 0.5)
            {
                return BadRequest(new { error = "Hiba történt" });
            }

            package.Id = Guid.NewGuid();
            Packages[package.Id] = package;
            return Ok(package);
        }

        [HttpGet("status/{id}")]
        public IActionResult GetStatus(Guid id)
        {
            if (!Packages.ContainsKey(id))
                return NotFound(new { error = "Csomag nem található" });

            var status = Statuses[Random.Next(Statuses.Length)];
            return Ok(status);
        }
        [HttpGet("packages")]
        public IActionResult GetPackages()
        {
            return Ok(Packages.Values.ToList());
        }
    }

    public class Package
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
    }
}