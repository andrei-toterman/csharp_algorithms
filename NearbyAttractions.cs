namespace csharp;
// https://www.hackerrank.com/contests/booking-hackathon/challenges/nearby-attractions

public static class NearbyAttractions
{
    public static void Example()
    {
        const bool fromFile = true;
        var inputStream = fromFile ? File.OpenText("input.txt") : Console.In;
        var nAttractions = int.Parse(inputStream.ReadLine()!);
        var attractions = new List<Attraction>();
        while (nAttractions-- > 0)
        {
            var data = inputStream.ReadLine()!.Split();
            var id = int.Parse(data[0]);
            var latitude = double.Parse(data[1]);
            var longitude = double.Parse(data[2]);
            attractions.Add(new Attraction(id, new Point(latitude, longitude)));
        }

        var nClients = int.Parse(inputStream.ReadLine()!);
        var clients = new List<Client>();
        while (nClients-- != 0)
        {
            var data = inputStream.ReadLine()!.Split();
            var latitude = double.Parse(data[0]);
            var longitude = double.Parse(data[1]);
            var transport = Enum.Parse<Transport>(data[2], true);
            var minutes = int.Parse(data[3]);
            clients.Add(new Client(new Point(latitude, longitude), transport, minutes));
        }

        var results = from client in clients
            let canVisit = from attraction in attractions
                let distance = client.Position.Distance(attraction.Position)
                let canTravel = (int)client.Transport * client.Minutes / 60.0
                where canTravel >= distance
                orderby distance, attraction.Id
                select attraction.Id
            select canVisit.ToList();

        foreach (var result in results) Console.WriteLine(string.Join(' ', result));
    }
}

file record Point(double Latitude, double Longitude)
{
    public double Distance(Point other)
    {
        const int earthRadius = 6371;
        var point1Lat = double.DegreesToRadians(Latitude);
        var point1Long = double.DegreesToRadians(Longitude);
        var point2Lat = double.DegreesToRadians(other.Latitude);
        var point2Long = double.DegreesToRadians(other.Longitude);
        var distance = Math.Acos(Math.Sin(point1Lat) * Math.Sin(point2Lat) +
                                 Math.Cos(point1Lat) * Math.Cos(point2Lat) *
                                 Math.Cos(point2Long - point1Long)) * earthRadius;

        return Math.Round(distance, 2);
    }
}

file record Attraction(int Id, Point Position);

file enum Transport
{
    Bike = 15,
    Foot = 5,
    Metro = 20,
}

file record Client(Point Position, Transport Transport, int Minutes);
