using System.Text;
using DataBase.DataBase;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json.Linq;

namespace Service;

public class CourierService
{
    private readonly CourierServiceContext _context;
    

    public CourierService(CourierServiceContext context, HttpClient client)
    {
        _context = context;
        _httpClient = client;
    }
    public async Task CourierRegister(Courier courier)
    {
        if (courier.FullName==_context.Couriers.FirstOrDefault(c=>c.FullName==courier.FullName)?.FullName)
        {
            
        }
        else
        {
            courier.Id = new Guid();
            courier.OrderPercentage = 25;
            await _context.Couriers.AddAsync(courier);
            await _context.SaveChangesAsync();
        }
    } 
     public async Task<List<CourierSearchViewModel>> GetClientOrdersAsync()
    {
        var orders = await _context.Orders.ToListAsync();

        var result = new List<CourierSearchViewModel>();

        foreach (var order in orders)
        {
            var departure = await BuildFullAddressAsync(order.DeparturePoint);
            var destination = await BuildFullAddressAsync(order.DestinationPoint);
            departure = departure.Replace("_", " ");
            destination = destination.Replace("_", " ");
            var status = await GetOrderStatusAsync(order.Id);
            if(status!="В ожидании")
                continue;
            var distance = await CalculateDistanceAsync(departure, destination);
            var cost = (decimal)(distance * 10*25/100); // 10 руб/км

            result.Add(new CourierSearchViewModel()
            {
                DepartureAddress = departure,
                DestinationAddress = destination,
                DistanceKm = distance,
                Cost = cost
            });
        }

        return result;
    }
    private async Task<string> BuildFullAddressAsync(Guid addressId)
    {
        var address = await _context.Routes.FindAsync(addressId);
        if (address == null) return "Неизвестно";

        var route = await _context.Routes.FindAsync(address.Id);

        var segments = new List<string>();
        while (route != null)
        {
            segments.Insert(0, route.Name);
            route = route.ParentId.HasValue ? await _context.Routes.FindAsync(route.ParentId) : null;
        }

        return string.Join(", ", segments);
    }
    private async Task<string> GetOrderStatusAsync(Guid orderId)
    {
        var history = await _context.OrderHistories
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.Date)
            .FirstOrDefaultAsync();

        if (history != null && history.Status == 1)
            return "В выполнении";
        if (history != null && history.Status == 2)
            return "Выполнен";
        if(history!=null&&history.Status==3)
            return "Отказано";

        return "В ожидании";
    }
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "5b3ce3597851110001cf6248850c2afff3ed41649b58a1d6c3396d56";
    

    public async Task<double> CalculateDistanceAsync(string from, string to)
    {
        var fromCoords = await GeocodeAsync(from);
        var toCoords = await GeocodeAsync(to);
        
        if (fromCoords == null || toCoords == null)
            return 0;
        var body = new
        {
            coordinates = new[] { fromCoords, toCoords }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openrouteservice.org/v2/directions/driving-car/json")
        {
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", _apiKey);

        var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(result);

        var distanceInMeters = json["routes"]?[0]?["summary"]?["distance"]?.ToObject<double>();

        return distanceInMeters.HasValue ? distanceInMeters.Value / 1000 : 0; // в км
    }

    private async Task<double[]> GeocodeAsync(string address)
    {
        var uri = $"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={Uri.EscapeDataString(address+", Moldova")}";

        var result = await _httpClient.GetStringAsync(uri);
        var json = JObject.Parse(result);
        var coords = json["features"]?[0]?["geometry"]?["coordinates"]?.ToObject<double[]>();
        return coords;
    } 
}