namespace csharp;
// https://www.hackerrank.com/contests/booking-hackathon/challenges/travel-profiles

public static class MatchFacilitiesAndBudget
{
    public static void Example()
    {
        const bool fromFile = true;
        var inputStream = fromFile ? File.OpenText("input.txt") : Console.In;

        var nHotels = int.Parse(inputStream.ReadLine()!);
        var hotels = new List<Hotel>();
        while (nHotels-- > 0)
        {
            var hotelData = inputStream.ReadLine()!.Split();
            if (hotelData is not [var idString, var priceString, .. var facilities]) continue;
            var id = int.Parse(idString);
            var price = int.Parse(priceString);
            hotels.Add(new Hotel(id, price, facilities.ToHashSet()));
        }

        var nClients = int.Parse(inputStream.ReadLine()!);
        var clients = new List<Client>();
        while (nClients-- > 0)
        {
            var clientData = inputStream.ReadLine()!.Split();
            if (clientData is not [var budgetString, .. var facilities]) continue;
            var budget = int.Parse(budgetString);
            clients.Add(new Client(budget, facilities.ToHashSet()));
        }

        var results =
            from client in clients
            let clientHotels = from hotel in hotels
                where hotel.Price <= client.Budget && hotel.Facilities.IsSupersetOf(client.Facilities)
                orderby hotel.Facilities.Count descending, hotel.Price, hotel.Id
                select hotel.Id
            select clientHotels.ToList();

        foreach (var result in results) Console.WriteLine(string.Join(' ', result));
    }
}

file record Client(int Budget, HashSet<string> Facilities);

file record Hotel(int Id, int Price, HashSet<string> Facilities);
