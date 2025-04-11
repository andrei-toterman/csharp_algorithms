namespace csharp;

public static class BinarySearchInterval
{
    public static void Example()
    {
        var inputStream = true ? File.OpenText("input.txt") : Console.In;
        var nm = inputStream.ReadLine()!.TrimEnd().Split(' ').Select(int.Parse).ToList();
        var n = nm[0];
        var m = nm[1];
        var deskInputs = new List<List<int>>();
        while (n-- > 0)
        {
            deskInputs.Add(inputStream.ReadLine()!.TrimEnd().Split(' ').Select(int.Parse).ToList());
        }

        Console.WriteLine(Solve(m, deskInputs));
    }

    private static int Solve(int unassigned, List<List<int>> desksData)
    {
        var maxTimeForAll = 0;
        var desks = desksData.Select(d =>
        {
            var perMinute = d[0];
            var inLine = d[1];
            var timeForLine = (int)Math.Ceiling((double)inLine / perMinute);
            if (timeForLine > maxTimeForAll) maxTimeForAll = timeForLine;
            return new { perMinute, inLine };
        }).ToList();


        var addedMinutes = 0;
        var step = unassigned;
        for (var i = 0; i < 500; i++)
        {
            var totalTime = maxTimeForAll + addedMinutes;
            if (desks.Sum(d => (long)totalTime * d.perMinute - d.inLine) >= unassigned)
            {
                if (step == 0 || addedMinutes == 0) return totalTime;
                addedMinutes -= step;
            }
            else if (step == 0)
            {
                maxTimeForAll++;
            }
            else
            {
                addedMinutes += step;
                step /= 2;
            }
        }

        throw new Exception("number too big");
    }
}
