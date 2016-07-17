using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace EllipticCurveCryptography
{
    public static class RandomExtension
    {
        /// <summary>
        /// Returns a random BigInteger within the specified range.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="minValue">The inclusive lower bound value</param>
        /// <param name="maxValue">The exlusive greater bound value</param>
        /// <returns></returns>
        public static BigInteger Next(this Random r, BigInteger minValue, BigInteger maxValue) 
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException("The minValue must be lower than maxValue");
            var bytesMinValue = minValue.ToByteArray().ToList();
            var bytesMaxValue = maxValue.ToByteArray().ToList();
            if (bytesMinValue.Count < bytesMaxValue.Count)
            {
                while (bytesMinValue.Count != bytesMaxValue.Count)
                    bytesMinValue.Add(0);
            }
            else if (bytesMinValue.Count > bytesMaxValue.Count)
            {
                while (bytesMaxValue.Count != bytesMinValue.Count)
                    bytesMaxValue.Add(0);
            }
            var listBytes=Gen(r, bytesMinValue, bytesMaxValue, bytesMinValue.Count-1);
            listBytes.Reverse();
            return new BigInteger(listBytes.ToArray());
        }

        private static List<byte> Gen(Random r, List<byte> bytesMinValue, List<byte> bytesMaxValue, int index)
        {
            var maxByte = Math.Max(bytesMinValue[index], bytesMaxValue[index]);
            var minByte = Math.Min(bytesMinValue[index], bytesMaxValue[index]);
            List<byte> res = new List<byte>();
            if(index!=0)
                res.Add((byte)r.Next(minByte, maxByte + 1));
            else res.Add((byte)r.Next(minByte, maxByte));
            if (res[res.Count - 1] == maxByte && index!=0)
            {
                res.AddRange(Gen(r, bytesMinValue, bytesMaxValue, index - 1));
            }
            else
            {
                byte[] buffer = new byte[index];
                r.NextBytes(buffer);
                res.AddRange(buffer);
            }
            return res;
        }
    }
}
