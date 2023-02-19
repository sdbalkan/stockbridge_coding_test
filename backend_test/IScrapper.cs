using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend_test
{
    public interface IScrapper
    {
        Task LoginAsync(string username, string password);
        Task SearchAsync(string stockType, string make, string model, string maxPrice, string maxDistance, string zipCode);
        Task<List<Vehicle>> ScrapAllByPagesAsync(int numberOfPages);
        Task<Vehicle> ScrapOneAsync(int index);
        Task SelectModelFromResultPage(string model);
    }
}
