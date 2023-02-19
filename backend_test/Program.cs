using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace backend_test
{
    internal class Program
    {
        private static VehicleService _vehicleService = new VehicleService();
        static async Task Main(string[] args)
        {
            try
            {
                var json = await _vehicleService.ScrapAsJson();
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
