using System;
using System.Numerics;
using System.Collections.Generic;
using EllipticCurveCryptography.Exceptions;
using System.Collections;


namespace EllipticCurveCryptography
{
    /// <summary>
    /// Represent an elliptic curve in Weierstraß equation (y^2 = x^3 + a*x + b) over a prime finite field
    /// </summary>
    [Serializable]
    public class EllipticCurve:IEllipticCurve
    {
        BigInteger a, b;
        BigInteger order;
        Random random = new Random(Environment.TickCount);

        /// <summary>
        /// Create an elliptic curve in Weierstraß equation given the modulus' value
        /// </summary>
        /// <param name="a">The a parameter of the elliptic curve</param>
        /// <param name="b">The b parameter of the elliptic curve</param>
        /// <param name="modulus">The modulus value for internal operations of the curve</param>
        public EllipticCurve(BigInteger a, BigInteger b, BigInteger modulus)
        {
            this.a = a;
            this.b = b;
            this.order = modulus;
            
            if (modulus == 0)
                throw new ModulusEqualZeroException();

            if (IsSingular)
                throw new SingularCurveEllipticException();
        }

        /// <summary>
        /// Create an elliptic curve represented in Weierstraß equation
        /// </summary>
        /// <param name="a">The a parameter of the elliptic curve</param>
        /// <param name="b">The b parameter of the elliptic curve</param>
        public EllipticCurve(BigInteger a,BigInteger b):this(a,b,1){}

        /// <summary>
        /// Gets the parameter a in the Weierstraß's ecuation
        /// </summary>
        public BigInteger A { get { return a; } }

        /// <summary>
        /// Gets the parameter b in the Weierstraß's ecuation
        /// </summary>
        public BigInteger B { get { return b; } }

        /// <summary>
        /// Gets the order of the group which the elliptic curve belongs
        /// </summary>
        public BigInteger GroupOrder
        {
            get { return order; }
        }

        /// <summary>
        /// Gets a value indicating if this elliptic curve is or not singular, i.e. 4*a^3 + 27*b^2 == 0 .
        /// </summary>
        /// <returns></returns>
        public bool IsSingular
        {
            get { return 4 * BigInteger.Pow(a, 3) + 27 * BigInteger.Pow(b, 2) == 0; }
        }

        /// <summary>
        /// Gets a value indicating if the specified point belongs or not to the elliptic curve
        /// </summary>
        /// <param name="point">The query point</param>
        /// <returns></returns>
        public bool Belong(Big2DPoint point)
        {
            if (point == Big2DPoint.InfinitePoint)
                return true;
            return BigInteger.Pow(point.Y, 2).Module(order) == (BigInteger.Pow(point.X, 3) + a * point.X + b).Module(order);
        }

        /// <summary>
        /// Performs the inverse of a given point
        /// </summary>
        /// <param name="point">The point to inverse</param>
        /// <returns></returns>
        public Big2DPoint Inverse(Big2DPoint point)
        {
            if (!Belong(point))
                throw new InvalidPointInEllipticCurveException();
            return new Big2DPoint(point.X, (order - point.Y).Module(order));
        }

        /// <summary>
        /// Gets the result value of the addition operation for the givens two points
        /// </summary>
        /// <param name="p1">The first point to add</param>
        /// <param name="p2">The second point to add</param>
        /// <returns></returns>
        public Big2DPoint Add(Big2DPoint p1, Big2DPoint p2) 
        {
            if (!Belong(p1) || !Belong(p2))
                throw new InvalidPointInEllipticCurveException();
            if (Inverse(p1) == p2)
                return Big2DPoint.InfinitePoint;
            if (p1 == Big2DPoint.InfinitePoint)
                return p2;
            if (p2 == Big2DPoint.InfinitePoint)
                return p1;
            if (p1 == p2)
                return Duplicate(p1);
            
            BigInteger d = (p2.Y - p1.Y) * (p2.X-p1.X).Module(order).MultiplicativeInverseMod(order);//(p2.Y - p1.Y) / (p2.X - p1.X) % order;
            var x = (BigInteger.Pow(d, 2) - p1.X - p2.X).Module(order);
            var y = (-p1.Y + d * (p1.X - x)).Module(order);
            return new Big2DPoint(x, y);
        }

        /// <summary>
        /// Gets the duplicate point's value of a given point, i.e. 2*p
        /// </summary>
        /// <param name="p">The point to duplicate</param>
        /// <returns></returns>
        public Big2DPoint Duplicate(Big2DPoint p)
        {
            if (!Belong(p))
                throw new InvalidPointInEllipticCurveException();
            if (p.Y.Sign == 0)
                return Big2DPoint.InfinitePoint;
            BigInteger d = (3 * BigInteger.Pow(p.X, 2) + a) * (2 * p.Y).Module(order).MultiplicativeInverseMod(order);
            var tempX=BigInteger.Pow(d,2)-2*p.X;
            return new Big2DPoint(tempX.Module(order), (-p.Y + d * (p.X - tempX)).Module(order));
        }

        /// <summary>
        /// Gets a Big2DPoint resulting of the multiplication of a given Big2DPoint whith an BigInteger scalar
        /// </summary>
        /// <param name="point">The Big2DPoint to multiplicate</param>
        /// <param name="scalar">The BigInteger scalar for multiplication</param>
        /// <returns></returns>
        public Big2DPoint ScalarMultiplicate(Big2DPoint point,BigInteger scalar)
        {
            if (scalar.IsZero)
                return new Big2DPoint(BigInteger.Zero, BigInteger.Zero);
            if (scalar == BigInteger.One)
                return point;
            if (scalar == 2)
                return Duplicate(point);
            BitArray bits = scalar.BinaryRepresentation();
            Big2DPoint res=point;
            for (int i = bits.Count-2; i >=0 ; i--)
            {
                res = Duplicate(res);
                if (bits[i])
                    res = Add(res,point);
            }
            return res;
        }

        /// <summary>
        /// Gets a random Big2DPoint value that belong to the elliptic curve
        /// </summary>
        /// <returns></returns>
        public Big2DPoint GetRandomBig2DPoint()
        {
            BigInteger x,y;
            do
            {
                x = random.Next(BigInteger.Zero, order);
                y=  (BigInteger.Pow(x,3) + a * x + b)%order;

            } while (y.Legendre(order) == -1);
            Big2DPoint res = new Big2DPoint(x, y.ModSqrt(order));
            if (random.Next(2) == 1)
                res = Inverse(res);            
            if (!Belong(res))
                throw new InvalidPointInEllipticCurveException();
            return res;
        }
    }

    /// <summary>
    /// Represent a big 2D point given the x,y coordinates
    /// </summary>
    [Serializable]
    public struct Big2DPoint:ICloneable
    {
        BigInteger x, y;

        /// <summary>
        /// Create an Big2DPoint given the coordinates x,y
        /// </summary>
        /// <param name="x">the x coordinate of the big point</param>
        /// <param name="y">the y coordinate of the big point</param>
        public Big2DPoint(BigInteger x, BigInteger y)
        {
            this.x=x;
            this.y=y;
        }

        /// <summary>
        /// Gets the x coordinate of the point
        /// </summary>
        public BigInteger X { get { return x; } }

        /// <summary>
        /// Gets the y coordinate of the point
        /// </summary>
        public BigInteger Y { get { return y; } }

        public object Clone()
        {
            return new Big2DPoint(x, y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Big2DPoint))
                return false;
            Big2DPoint b = (Big2DPoint)obj;
            return x.Equals(b.X) && y.Equals(b.Y);
        }

        public static bool operator==(Big2DPoint p1,Big2DPoint p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Big2DPoint p1,Big2DPoint p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Big2DPoint InfinitePoint { get { return new Big2DPoint(BigInteger.Zero, BigInteger.One); } }

        public override string ToString()
        {
            return x.ToString() + " , " + y.ToString();
        }
    }
}
