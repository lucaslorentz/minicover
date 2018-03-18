using System.Linq;

public class Lambdas
{
    public int Add2ToEachValueAndSumThem(params int[] values)
    {
        return values.Select(a => a + 2).Sum();
    }
}
