namespace csharp;
// https://www.hackerrank.com/contests/booking-hackathon/challenges/budget-friendly

public static class Backtracking
{
    public static void Example()
    {
        const bool fromFile = true;
        var inputStream = fromFile ? File.OpenText("input.txt") : Console.In;

        var citiesAndBudget = inputStream.ReadLine()!.Split().Select(int.Parse).ToList();
        var nCities = citiesAndBudget[0];
        var budget = citiesAndBudget[1];
        var cities = new List<City>();
        while (nCities-- > 0)
        {
            var nHotels = int.Parse(inputStream.ReadLine()!);
            var hotels = new List<Hotel>();
            while (nHotels-- > 0)
            {
                var priceAndScore = inputStream.ReadLine()!.Split().ToList();
                var price = int.Parse(priceAndScore[0]);
                var score = decimal.Parse(priceAndScore[1]);
                hotels.Add(new Hotel(price, score));
            }

            cities.Add(new City(hotels.OrderByDescending(hotel => hotel.Score).ToList()));
        }

        var chosenHotels = new List<int>();
        for (var (c, backtrack) = (0, false); c < cities.Count && c >= 0; c += backtrack ? -1 : 1)
        {
            var city = cities[c];
            var hotels = city.Hotels;
            var startHotel = chosenHotels.ElementAtOrDefault(c);
            if (backtrack)
            {
                chosenHotels.RemoveAt(c);
                startHotel++;
            }

            backtrack = true;
            for (var h = startHotel; h < hotels.Count; h++)
            {
                var hotel = hotels[h];
                if (chosenHotels.Select((ih, ic) => cities[ic].Hotels[ih].Price).Sum() + hotel.Price > budget) continue;
                chosenHotels.Add(h);
                backtrack = false;
                break;
            }
        }

        Console.WriteLine(chosenHotels.Count == cities.Count
            ? chosenHotels.Select((ih, ic) => cities[ic].Hotels[ih].Score).Sum().ToString("F2")
            : -1);
    }
}

file record Hotel(int Price, decimal Score);

file record City(List<Hotel> Hotels);
