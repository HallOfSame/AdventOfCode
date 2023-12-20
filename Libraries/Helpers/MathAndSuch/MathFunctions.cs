using System.Collections.Generic;
using System.Linq;

// To not conflict with the normal Math namespace
namespace Helpers.MathAndSuch;

public class MathFunctions
{
    // Tweaked from stack overflow
    public static decimal LeastCommonMultiple(IEnumerable<decimal> values)
    {
        // GDC using Euclids formula (I think)
        decimal GCD(decimal a, decimal b)
        {
            while (true)
            {
                if (b == 0)
                {
                    return a;
                }

                var a1 = a;
                a = b;
                b = a1 % b;
            }
        }

        // LCM of two numbers can be sped up by using GCD
        decimal LCM(decimal a, decimal b)
        {
            return a * b / GCD(a, b);
        }

        var orderedValues = values.OrderBy(x => x);

        var multiple = orderedValues.First();
        
        // Get the LCM of all input values
        foreach (var value in orderedValues)
        {
            multiple = LCM(multiple, value);
        }

        return multiple;
    }
}