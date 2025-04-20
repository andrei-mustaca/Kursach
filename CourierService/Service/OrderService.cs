using System.Text;
using DataBase.DataBase;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json.Linq;

namespace Service;

public class OrderService
{
    private readonly CourierServiceContext _context;
    
    public OrderService(CourierServiceContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
    }

    public async Task<bool> CreateOrder(OrderViewModel model)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c=>c.FullName==model.FullName&&c.TelephoneNumber==model.TelephoneNumber);
        if(client==null)
        {
            return false;
        }

        string[] massDeparture = model.DepartureRouteName.Split(' ');
        var departureRoute = await _context.Routes.FirstOrDefaultAsync(r=>r.Name==massDeparture[0]);
        departureRoute = await _context.Routes.FirstOrDefaultAsync(r=>r.Name==massDeparture[1]&&r.ParentId==departureRoute.Id);
        departureRoute = await _context.Routes.FirstOrDefaultAsync(r=>r.Name==massDeparture[2]&&r.ParentId==departureRoute.Id);
        string[] massDestination = model.DestinationRouteName.Split(' ');
        var destinationRoute = await _context.Routes.FirstOrDefaultAsync(r=>r.Name==massDestination[0]);
        destinationRoute = await _context.Routes.FirstOrDefaultAsync(r=>r.Name==massDestination[1]&&r.ParentId==destinationRoute.Id);
        destinationRoute = await _context.Routes.FirstOrDefaultAsync(r=>r.Name==massDestination[2]&&r.ParentId==destinationRoute.Id);

        var order = new Order()
        {
            Id = new Guid(),
            ClientId = client.Id,
            DeparturePoint = departureRoute.Id,
            DestinationPoint = destinationRoute.Id
        };
        _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<ClientOrderViewModel>> GetClientOrdersAsync(Guid clientId)
    {
        var orders = await _context.Orders
            .Where(o => o.ClientId == clientId)
            .ToListAsync();

        var result = new List<ClientOrderViewModel>();

        foreach (var order in orders)
        {
            var departure = await BuildFullAddressAsync(order.DeparturePoint);
            var destination = await BuildFullAddressAsync(order.DestinationPoint);
            departure = departure.Replace("_", " ");
            destination = destination.Replace("_", " ");
            var status = await GetOrderStatusAsync(order.Id);
            var distance = await CalculateDistanceAsync(departure, destination);
            var cost = (decimal)(distance * 10); // 10 руб/км

            result.Add(new ClientOrderViewModel
            {
                DepartureAddress = departure,
                DestinationAddress = destination,
                Status = status,
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
        var acceptance = await _context.OrderAcceptances.FirstOrDefaultAsync(x => x.OrderId == orderId);
        var history = await _context.OrderHistories
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.Date)
            .FirstOrDefaultAsync();

        if (history != null && history.Status == 0)
            return "Выполнено";
        if (history != null && history.Status == 1)
            return "Отказано";
        if (acceptance != null)
            return "В исполнении";

        return "В ожидании";
    }
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "5b3ce3597851110001cf6248850c2afff3ed41649b58a1d6c3396d56"; // <-- замени на свой ключ
    

    public async Task<double> CalculateDistanceAsync(string from, string to)
    {
        // 1. Геокодинг — получаем координаты
        var fromCoords = await GeocodeAsync(from);
        var toCoords = await GeocodeAsync(to);
        
        if (fromCoords == null || toCoords == null)
            return 0;

        // 2. Запрос к маршрутизатору
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