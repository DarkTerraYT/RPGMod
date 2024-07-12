namespace RPGMod
{
    public static class Ext
    {
        public static bool NoRemainder(this double num, double divisor)
        {
            return (num % divisor) == 0;
        }

        public static bool NoRemainder(this decimal num, decimal divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this float num, float divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this long num, long divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this int num, int divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this short num, short divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this ulong num, ulong divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this uint num, uint divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this ushort num, ushort divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this byte num, byte divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }

        public static bool NoRemainder(this sbyte num, sbyte divisor)
        {
            return ((double)num % (double)divisor) == 0;
        }
    }
}
