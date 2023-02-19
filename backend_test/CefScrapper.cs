using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace backend_test
{
    public class CefScrapper: IScrapper, IDisposable
    {
        private ChromiumWebBrowser _browser;
        private const string _url = "https://www.cars.com/";
        private bool _isLoggedIn = false;
        private bool _isLoading = true;
        private bool _disposed;

        private const string openLoginModalScript = "(function() {" +
            "let signInButton = document.querySelector('cars-global-header').shadowRoot.querySelector('ep-button');" + //find sign in button
            "if(!signInButton) return true;" + //user already logged in
            "signInButton.click();" + //open login modal
            "return false;" +
            "})()";

        private const string loginScript = "(function(userName, password) {{" +
            "document.querySelector('[id=auth-modal-email]').value = userName;" + //fill email
            "document.querySelector('[id=auth-modal-current-password]').value = password;" + //fill password
            "document.querySelector('cars-auth-modal').shadowRoot.querySelector('ep-button[type=submit]').click();" + //click login button
            "return;" +
            "}})('{0}','{1}')";

        private const string searchScript = "(function(stockType, make, model, maxPrice, maxDistance, zipCode) {{" +
            "document.querySelector('select[id=make-model-search-stocktype]').value = stockType;" + //select used cars
            "document.querySelector('select[id=makes]').value = make;" + //select tesla
            "document.querySelector('select[id=models]').value = model;" + //select model s
            "document.querySelector('select[id=make-model-max-price]').value = maxPrice;" + //select max price as 100K
            "document.querySelector('select[id=make-model-maximum-distance]').value = maxDistance;" + //select max distance as all
            "document.querySelector('input[id=make-model-zip]').value = zipCode;" + //fill zip code
            "document.querySelector('button[type=submit]').click();" + //click search button
            "return;" +
            "}})('{0}','{1}','{2}','{3}','{4}','{5}')";

        private const string scrapVehiclesScript = "(function() {" +
            "let vehicleCards = document.querySelector('div[class=vehicle-cards]').querySelectorAll(\"div[class='vehicle-card   ']\");" +
            "let vehicles = [];" +
            "for(let vehicleCard of vehicleCards) {" +
            "let stockType = vehicleCard.querySelector(\"p[class=stock-type]\").innerText;" +
            "let title = vehicleCard.querySelector(\"h2[class=title]\").innerText;" +
            "let mileage = vehicleCard.querySelector(\"div[class=mileage]\").innerText;" +
            "let primaryPrice = vehicleCard.querySelector(\"span[class=primary-price]\").innerText;" +
            "let dealerName = vehicleCard.querySelector(\"div[class=dealer-name]\").innerText;" +
            "let vehicle = {" +
            "stockType: stockType," +
            "title: title," +
            "mileage: mileage," +
            "primaryPrice: primaryPrice," +
            "dealerName: dealerName," +
            "};" +
            "vehicles.push(vehicle);" +
            "}" +
            "return vehicles;" +
            "})()";

        private const string scrapOneVehicleScript = "(function(index) {{" +
            "let vehicleCards = document.querySelector('div[class=vehicle-cards]').querySelectorAll(\"div[class='vehicle-card   ']\");" +
            "let vehicleCard = vehicleCards[index];" +
            "let stockType = vehicleCard.querySelector(\"p[class=stock-type]\").innerText;" +
            "let title = vehicleCard.querySelector(\"h2[class=title]\").innerText;" +
            "let mileage = vehicleCard.querySelector(\"div[class=mileage]\").innerText;" +
            "let primaryPrice = vehicleCard.querySelector(\"span[class=primary-price]\").innerText;" +
            "let dealerName = vehicleCard.querySelector(\"div[class=dealer-name]\").innerText;" +
            "let homeDelivery = '';" +
            "let homeDeliveryButton = vehicleCard.querySelector('div[class=\"sds-badge sds-badge--virtual-appointments\"]');" +
            "if(homeDeliveryButton) {{" +
            "homeDeliveryButton.click();" +
            "homeDelivery = document.querySelector('p[class=badge-description]').innerText;" +
            "}}" +
            "let vehicle = {{" +
            "stockType: stockType," +
            "title: title," +
            "mileage: mileage," +
            "primaryPrice: primaryPrice," +
            "dealerName: dealerName," +
            "homeDelivery: homeDelivery," +
            "}};" +
            "return vehicle;" +
            "}})({0})";

        private const string nextPageScript = "(function() {" +
            "document.querySelector('a[id=next_paginate]').click();" +
            "})()";

        private const string selectModelScript = "(function(model) {{" +
            "document.querySelector('input[id=model_tesla-model_x]').click()" +
            "}})('{0}')";

        public CefScrapper()
        {
            Initialize();

            _browser = new ChromiumWebBrowser(_url);
            _browser.LoadingStateChanged += Browser_BrowserLoadingStateChanged;
            _browser.BrowserInitialized += Browser_BrowserInitialized;
        }

        private void Browser_BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            _isLoading = e.IsLoading;
        }

        private async Task WaitForInitialLoadAsync()
        {
            var initialLoadResponse = await _browser.WaitForInitialLoadAsync();
            if (!initialLoadResponse.Success)
            {
                throw new Exception(string.Format("Page load failed with ErrorCode:{0}, HttpStatusCode:{1}", initialLoadResponse.ErrorCode, initialLoadResponse.HttpStatusCode));
            }

            while (_isLoading)
            {
                Thread.Sleep(500);
            }
        }

        public async Task LoginAsync(string userName, string password)
        {
            var scriptResult = await ExecuteScriptAsync<object>(openLoginModalScript);

            var isAlreadyLogin = (bool)scriptResult;
            if (!isAlreadyLogin)
            {
                await ExecuteScriptAsync<object>(String.Format(loginScript, userName, password));
            }

            _isLoggedIn = true;
        }

        public async Task SearchAsync(string stockType, string make, string model, string maxPrice, string maxDistance, string zipCode)
        {
            await ExecuteScriptAsync<object>(String.Format(searchScript, stockType, make, model, maxPrice, maxDistance, zipCode));
        }

        public async Task<List<Vehicle>> ScrapAllByPagesAsync(int numberOfPages)
        {
            var vehicles = new List<Vehicle>();

            for (int i = 0; i < numberOfPages; i++)
            {
                if (i > 0) // skip for first page
                {
                    await ExecuteScriptAsync<object>(nextPageScript);
                }

                var pageVehicles = await GetVehiclesByPageAsync();

                vehicles.AddRange(pageVehicles);
            }

            return vehicles;
        }

        public async Task<Vehicle> ScrapOneAsync(int index)
        {
            var result = await ExecuteScriptAsync<object>(String.Format(scrapOneVehicleScript, index));

            return Map((IDictionary<string, object>)result);
        }

        public async Task SelectModelFromResultPage(string model)
        {
            await ExecuteScriptAsync<object>(String.Format(selectModelScript, model));
        }

        private async Task<List<Vehicle>> GetVehiclesByPageAsync()
        {
            var results = await ExecuteScriptAsync<List<object>>(scrapVehiclesScript);

            return results?
                .Cast<IDictionary<string, object>>()?
                .ToList()?
                .Select(r => Map(r))?
                .ToList();
        }

        private Vehicle Map(IDictionary<string, object> dictRow)
        {
            dictRow.TryGetValue("stockType", out object stockType);
            dictRow.TryGetValue("title", out object title);
            dictRow.TryGetValue("mileage", out object mileage);
            dictRow.TryGetValue("primaryPrice", out object primaryPrice);
            dictRow.TryGetValue("dealerName", out object dealerName);
            dictRow.TryGetValue("homeDelivery", out object homeDelivery);

            var vehicle = new Vehicle()
            {
                StockType = stockType?.ToString(),
                Title = title?.ToString(),
                Mileage = mileage?.ToString(),
                PrimaryPrice = primaryPrice?.ToString(),
                DealerName = dealerName?.ToString(),
                HomeDelivery = homeDelivery?.ToString(),
            };

            return vehicle;
        }

        private static void Initialize()
        {
            //It should initialize once.
            if (Cef.IsInitialized)
            {
                return;
            }

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            settings.CefCommandLineArgs.Add("enable-shadow-dom", "true");
            settings.CefCommandLineArgs.Add("enable-style-scoped", "true");
            settings.CefCommandLineArgs.Add("enable-pointer-lock", "true");
            settings.CefCommandLineArgs.Add("enable-source-media", "true");
            settings.CefCommandLineArgs.Add("enable-date-type_input", "true");
            settings.CefCommandLineArgs.Add("disable-web-security", "true");


            Cef.Initialize(settings);
        }

        private void Browser_BrowserInitialized(object sender, EventArgs e)
        {
            _browser.ShowDevTools();
        }

        private async Task<T> ExecuteScriptAsync<T>(string script)
        {
            await WaitForInitialLoadAsync();

            var scriptResult = await _browser.EvaluateScriptAsync(script);
            if (!scriptResult.Success)
            {
                throw new Exception("Executing the script failed!");
            }

            Thread.Sleep(500);

            return (T)scriptResult.Result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_browser != null)
                {
                    _browser.Dispose();
                    _browser = null;
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
