using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.IO;
using EllipticCurveCryptography.Properties;
using EllipticCurveCryptography.Exceptions;

namespace EllipticCurveCryptography
{
    /// <summary>
    /// Specify the approximately count of bits for a prime number
    /// </summary>
    public enum CountBitsPrimeNumber { n256=256,n384=384,n512=512};

    /// <summary>
    /// Represent the domain parameters for a elliptic curve over a prime finite field
    /// </summary>
    [Serializable]
    public class EllipticCurveDomainParameters
    {
        BigInteger primeOrder;
        BigInteger a, b;
        Big2DPoint basePoint;
        BigInteger orderBasePoint;
        double cofactor;

        EllipticCurve curve;
        Random random = new Random(Environment.TickCount);

        /// <summary>
        /// Initialize a new domain parameters given the elliptic curve, the base point, his order on that group and the cofactor
        /// </summary>
        /// <param name="primeOrder">A prime number that will be the order of the group of the finite field</param>
        /// <param name="a">The parameter a in the Weierstraß's ecuation</param>
        /// <param name="b">The parameter b in the Weierstraß's ecuation</param>
        /// <param name="basePoint">The base point selected from the prime finite field</param>
        /// <param name="orderBasePoint">The order of the selected base point</param>
        /// <param name="cofactor">The cofactor value of the selected base point</param>
        public EllipticCurveDomainParameters(BigInteger primeOrder, BigInteger a, BigInteger b, Big2DPoint basePoint, BigInteger orderBasePoint, double cofactor)
        {
            curve = new EllipticCurve(a, b, primeOrder);
            if (!curve.Belong(basePoint))
                throw new InvalidPointInEllipticCurveException();
            if (curve.ScalarMultiplicate(basePoint, orderBasePoint) != Big2DPoint.InfinitePoint)
                throw new InvalidOrderPointException();
            this.primeOrder = primeOrder;
            this.a = a;
            this.b = b;
            this.basePoint = basePoint;
            this.orderBasePoint = orderBasePoint;
            this.cofactor = cofactor;
            if (!CheckedParameters())
                throw new InvalidDomainParametersException();
        }

        /// <summary>
        /// Initialize a new domain parameters given the approximately count of bits of the prime order of the group
        /// </summary>
        /// <param name="countBits">The approximately count of bits of the prime order of the group</param>
        public EllipticCurveDomainParameters(CountBitsPrimeNumber countBits)
        {
            primeOrder = GetRandomPrimeNumber(countBits);
            do
            {
                a = random.Next(0, primeOrder);
                b = random.Next(0, primeOrder);
                try { curve = new EllipticCurveCryptography.EllipticCurve(a, b, primeOrder); }
                catch (SingularCurveEllipticException) { continue; }
                basePoint = GetRandomBasePoint();
                orderBasePoint = CalculateOrderBasePoint();
                cofactor = CalculateCofactor();
            } while (!CheckedParameters());    
        }

        private bool CheckedParameters()
        {
            if (a < 0 || a >= primeOrder || b < 0 || b >= primeOrder || basePoint.X < 0 || basePoint.X >= primeOrder || basePoint.Y < 0 || basePoint.Y >= primeOrder)
                return false;
            return true;
        }

        private double CalculateCofactor()
        {
            return 0;
        }

        private BigInteger CalculateOrderBasePoint()
        {
            return primeOrder;
        }

        private Big2DPoint GetRandomBasePoint()
        {
            return curve.GetRandomBig2DPoint();
        }

        private BigInteger GetRandomPrimeNumber(CountBitsPrimeNumber countBits)
        {
            string text="";
            switch(countBits)
            { 
                case CountBitsPrimeNumber.n256:
                    text = Resources.Primos_256;
                    break;
                case CountBitsPrimeNumber.n384:
                    text = Resources.Primos_384;
                    break;
                case CountBitsPrimeNumber.n512:
                    text = Resources.Primos_512;
                    break;
            }
            var lines = text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var index = random.Next(lines.Length);
            if (lines[index].Contains('x'))
                return BigInteger.Parse(lines[index].Replace("x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            return BigInteger.Parse(lines[index]);
        }

        /// <summary>
        /// Gets the prime number that is the order of the group of the finite field
        /// </summary>
        public BigInteger Order
        {
            get { return primeOrder; }
        }

        /// <summary>
        /// Gets the parameter a in the Weierstraß's ecuation
        /// </summary>
        public BigInteger A
        {
            get { return a; }
        }

        /// <summary>
        /// Gets the parameter b in the Weierstraß's ecuation
        /// </summary>
        public BigInteger B
        {
            get { return b; }
        }

        /// <summary>
        /// Gets the selected base point of the elliptic curve
        /// </summary>
        public Big2DPoint BasePoint
        {
            get { return basePoint; }
        }

        /// <summary>
        /// Gets the order of the selected base point of the elliptic curve
        /// </summary>
        public BigInteger OrderBasePoint
        {
            get { return orderBasePoint; }
        }

        /// <summary>
        /// Gets the cofactor value of the selected base point
        /// </summary>
        public double Cofactor
        {
            get { return cofactor; }
        }

        /// <summary>
        /// Gets the elliptic curve over the finite prime field defined in this domain parameters
        /// </summary>
        public EllipticCurve EllipticCurve
        {
            get { return curve; } 
        }
    }
}
