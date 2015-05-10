using System;

namespace NeonShooter.Utils
{
    public static class MathHelper
    {
        public static int Max(params int[] values)
        {
            if (values.Length == 0) throw new ArgumentException("Must provide at least one argument.");

            int currentMax = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                currentMax = Math.Max(currentMax, values[i]);
            }

            return currentMax;
        }

        public static int IntPow(int x, int exponent)
        {
            if (exponent < 0)
                throw new ArgumentException("Argument exponent must not be lower than 0.");

            int result = 1;
            for (int i = 0; i < exponent; i++)
                result *= x;
            return result;
        }
    }
}
