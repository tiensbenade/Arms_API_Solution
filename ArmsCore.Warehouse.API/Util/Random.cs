using System.Security.Cryptography;

namespace ArmsCore.Warehouse.API.Util
{
    public static class Random
    {
        public static string CreateNewInternalRefNumber()
        {
            string refNumber = GenerateRandom(1024 * 1024, 1024 * 1024 * 1024).ToString();

            return string.Format("R-{0}", refNumber);
        }

        public static int GenerateRandom(int min, int max)
        {
            RandomNumberGenerator randomNumber = RandomNumberGenerator.Create();
            // Generate four random bytes
            byte[] input = new byte[8];
            randomNumber.GetBytes(input);

            // Convert the bytes to a UInt32
            UInt32 scale = BitConverter.ToUInt32(input, 0);

            // And use that to pick a random number >= min and < max
            return (int)(min + (max - min) * (scale / (uint.MaxValue + 1.0)));
        }
    }
}
