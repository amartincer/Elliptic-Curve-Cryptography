using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;

namespace EllipticCurveCryptography
{
    public static class BigIntegerExtension
    {
        /// <summary>
        /// Gets the value of the ecuation x^2 = y % p
        /// </summary>
        /// <param name="value">the big integer to evaluate</param>
        /// <param name="modulus">the modulus value</param>
        /// <returns></returns>        
        public static BigInteger ModSqrt(this BigInteger value, BigInteger modulus)
        {
            if (value.Legendre(modulus) != 1)
                return BigInteger.Zero;
            else if (value == 0)
                return 0;
            else if (modulus % 4 == 3)
                return BigInteger.ModPow(value, (modulus + 1) / 4, modulus);
            BigInteger s = modulus - 1;
            int e=0;
            while(s%2==0)
            {
                s /= 2;
                e++;
            }
            BigInteger n=2;
            while (n.Legendre(modulus) != -1)
                n++;
            BigInteger x = BigInteger.ModPow(value, (s + 1) / 2, modulus);
            BigInteger b = BigInteger.ModPow(value, s, modulus);
            BigInteger g = BigInteger.ModPow(n, s, modulus);
            int r = e;

            while (true)
            {
                BigInteger t = b;
                int m = 0;
                for (; m < r; m++)
                {
                    if (t == 1)
                        break;
                    t = BigInteger.ModPow(t, 2, modulus);
                }
                if (m == 0)
                    return x;
                BigInteger gs = BigInteger.ModPow(g, BigInteger.Pow(2, r - m - 1), modulus);
                g = (gs * gs) % modulus;
                x = (x * gs) % modulus;
                b = (b * g) % modulus;
                r = m;
            }
        }

        /// <summary>
        /// Returns true if the BigInteger value is a prime number; false otherwise.
        /// </summary>
        /// <param name="value">The BigInteger value</param>
        /// <returns></returns>
        public static bool IsPrime(this BigInteger value)
        {
            var roof=value.Sqrt();
            for (BigInteger i = 2; i <= roof; i++)
            {
                if (value % i == 0)
                    return false;
            }
            return true;
        }

        public static int Legendre(this BigInteger value, BigInteger modulus)
        {
            int res=1;
            if (value == 0)
                res = ((modulus == 1) ? 1 : 0);
            else if (value == 2)
            {
                switch ((int)(modulus % 8))
                {
                    case 1:
                    case 7:
                        res = 1;
                        break;
                    case 3:
                    case 5:
                        res = -1;
                        break;
                }
            }
            else if (value >= modulus)
                res = Legendre(value % modulus, modulus);
            else if (value % 2 == 0)
                res = Legendre(2, modulus) * Legendre(value / 2, modulus);
            else
                res = (value % 4 == 3 && modulus % 4 == 3) ? -Legendre(modulus, value) : Legendre(modulus, value);
            return res;
        }

        /// <summary>
        /// Gets the square root of a BigInteger value
        /// </summary>
        /// <param name="bigInt">The BigInteger value</param>
        /// <returns></returns>
        public static BigInteger Sqrt(this BigInteger bigInt)
        {
            return Sqrt(bigInt,0.00001f);           
        }

        /// <summary>
        /// Gets the square root of a BigInteger value, specifing the desired precision of the operations
        /// </summary>
        /// <param name="bigInt">The BigInteger value</param>
        /// <param name="precision">The float value specifing the desired precision of the operations. Must be greater or equal to zero</param>
        /// <returns></returns>
        public static BigInteger Sqrt(this BigInteger bigInt,float precision)
        {
            if (bigInt.Sign == -1)
                throw new ArgumentOutOfRangeException("The bigInt parameter must be greater or equal to zero");
            BigInteger a, b, p;
            a = bigInt;
            p = a * a;
            BigInteger two = new BigInteger(2);
            float diff=0;
            do
            {
                try { diff = (float)(p - bigInt); }
                catch { diff = float.MaxValue; }
                if (diff < precision)
                    break;
                b = (a + (bigInt / a)) / two;
                a = b;
                p = a * a;
            }
            while (true);
            return a;
        }

        /// <summary>
        /// Gets the value of the multiplicative inverse modular operation. i.e. value*x mod n == 1
        /// </summary>
        /// <param name="value">The BigInteger param</param>
        /// <param name="modulus">The BigInteger modulus for the operation</param>
        /// <returns></returns>
        public static BigInteger MultiplicativeInverseMod(this BigInteger value,BigInteger modulus)
        {
            BigInteger cociente;
            BigInteger m1 = value;
            BigInteger m2 = modulus;
            BigInteger u1 = new BigInteger(1);
            BigInteger u2 = new BigInteger(0);
            BigInteger v1 = new BigInteger(0);
            BigInteger v2 = new BigInteger(1);
            BigInteger temp;
            
            while (m2!=0)
            {
                cociente = m1 / m2;
                temp = m1;
                m1 = m2;
                m2 = temp % m2;
                
                temp = u1;
                u1 = u2;
                u2 = temp - cociente * u2;

                temp = v1;
                v1 = v2;
                v2 = temp - cociente * v2;
            }
            return u1.Module(modulus);
        }

        /// <summary>
        /// Performs the module operation of a given BigInteger. If the BigInteger is negative, the result would be a positive value between 0 and modulus - 1
        /// </summary>
        /// <param name="value">The BigInteger param</param>
        /// <param name="modulus">The BigInteger modulus for the operation</param>
        /// <returns></returns>
        public static BigInteger Module(this BigInteger value,BigInteger modulus)
        {
            BigInteger res = value % modulus;
            if (res.Sign == -1)
                res = modulus + res;
            return res;
        }

        /// <summary>
        /// Returns the binary representation of the BigInteger value
        /// </summary>
        /// <param name="value">A BigIntger value</param>
        /// <returns></returns>
        public static BitArray BinaryRepresentation(this BigInteger value)
        {
            List<bool> list = new List<bool>();
            BigInteger div = value;
            BigInteger rem;
            while(div!=0)
            {
                div=BigInteger.DivRem(div, 2, out rem);
                list.Add(rem==0?false:true);
            }
            list.Reverse();
            return new BitArray(list.ToArray());
        }
    }
}
