namespace csharp;
// https://www.hackerrank.com/contests/booking-womenintech/challenges/minimum-hotels

public static class MinimumHotels
{
    public static void Example()
    {
        var tests = int.Parse(Console.ReadLine()!.Trim());
        while (tests-- > 0)
        {
            _ = Console.ReadLine();
            var customer = Console.ReadLine()!.TrimEnd().Split(' ').Select(int.Parse).ToList();
            var k = int.Parse(Console.ReadLine()!.Trim());
            Console.WriteLine(Solve(customer, k));
        }
    }

    private static int Solve(List<int> customers, int k)
    {
        customers.Sort();
        var hotels = 1;
        var pos = customers.First();
        foreach (var customer in customers.Skip(1))
        {
            if (pos + 2 * k >= customer) continue;
            hotels++;
            pos = customer;
        }

        return hotels;
    }
}
