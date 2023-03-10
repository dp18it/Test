using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipsController : ControllerBase
    {
        private static List<Port> ports = new List<Port>
    {
        new Port { Name = "Port A", Latitude = 10.0, Longitude = 20.0 },
        new Port { Name = "Port B", Latitude = 15.0, Longitude = 25.0 },
        new Port { Name = "Port C", Latitude = 20.0, Longitude = 30.0 }
    };

        private static List<Ship> ships = new List<Ship>();

        [HttpPost]
        public IActionResult AddShip([FromBody] Ship ship)
        {
            // Assign unique ID to ship
            ship.Id = ships.Count + 1;

            ships.Add(ship);

            return Ok(ship);
        }

        [HttpGet]
        public IActionResult GetAllShips()
        {
            return Ok(ships);
        }

        [HttpPut("{id}/velocity")]
        public IActionResult UpdateShipVelocity(int id, [FromBody] double velocity)
        {
            var ship = ships.FirstOrDefault(s => s.Id == id);

            if (ship == null)
            {
                return NotFound();
            }

            ship.Velocity = velocity;

            return Ok(ship);
        }

        [HttpGet]
        [Route("api/ships/{shipId}/closestPort")]
        public IActionResult GetClosestPort(int shipId)
        {
            // Find the ship with the specified ID
            var ship = ships.FirstOrDefault(s => s.Id == shipId);
            if (ship == null)
            {
                return NotFound();
            }

            // Calculate the distance between the ship and each port
            var distances = ports.Select(p => new
            {
                Port = p,
                Distance = CalculateDistance(ship.Latitude, ship.Longitude, p.Latitude, p.Longitude)
            });

            // Find the closest port
            var closestPort = distances.OrderBy(d => d.Distance).First().Port;

            // Calculate the estimated arrival time based on the velocity of the ship and the distance to the closest port
            var estimatedArrivalTime = DateTime.Now.AddHours(closestPort.Distance / ship.Velocity);

            // Return the details of the closest port and estimated arrival time
            return Ok(new
            {
                PortName = closestPort.Name,
                PortLatitude = closestPort.Latitude,
                PortLongitude = closestPort.Longitude,
                EstimatedArrivalTime = estimatedArrivalTime
            });
        }

        // Helper method to calculate the distance between two sets of latitude and longitude coordinates
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        // Helper method to convert degrees to radians
        private double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

    }
}
