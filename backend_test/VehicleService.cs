using CefSharp.Internals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend_test
{
    public class VehicleService
    {
        private const string UserName = "johngerson808@gmail.com";
        private const string Password = "test8008";
        private const int NumberOfPages = 2;
        private const int SpecificIndex = 3;

        private readonly IScrapper _scrapper;

        public VehicleService()
        {
            _scrapper = new CefScrapper();
        }

        public VehicleService(IScrapper scrapper)
        {
            _scrapper = scrapper;
        }

        public async Task<String> ScrapAsJson()
        {
            var scraps = new List<Scrap>();

            await _scrapper.LoginAsync(UserName, Password); //Login

            await _scrapper.SearchAsync("used", "tesla", "tesla-model_s", "100000", "all", "94596"); //Search
            scraps.Add(await Scrap());

            await _scrapper.SelectModelFromResultPage("tesla-model_x"); //Select another model
            scraps.Add(await Scrap());

            return JsonConvert.SerializeObject(scraps, Formatting.Indented);
        }

        private async Task<Scrap> Scrap()
        {
            var vehicles = await _scrapper.ScrapAllByPagesAsync(NumberOfPages);
            var vehicle = await _scrapper.ScrapOneAsync(SpecificIndex);

            var scrap = new Scrap()
            {
                VehiclesFromPages = vehicles,
                SpecificVehicle = vehicle
            };

            return scrap;
        }
    }
}
