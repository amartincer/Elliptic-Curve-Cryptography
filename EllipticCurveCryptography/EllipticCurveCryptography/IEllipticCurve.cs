using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace EllipticCurveCryptography
{
    /// <summary>
    /// Define the methods and properties that all implementations of elliptic curves at must be satisfied
    /// </summary>
    public interface IEllipticCurve
    {
        /// <summary>
        /// Gets the parameter a of the elliptic curve
        /// </summary>
        BigInteger A { get; }

        /// <summary>
        /// Gets the parameter b of the elliptic curve
        /// </summary>
        BigInteger B { get; }

        /// <summary>
        /// Gets the order of the group which the elliptic curve belongs
        /// </summary>
        BigInteger GroupOrder { get; }

        /// <summary>
        /// Gets a value indicating if this elliptic curve is or not singular, i.e. 4*a^3 + 27*b^2 == 0 .
        /// </summary>
        /// <returns></returns>
        bool IsSingular { get; }

        /// <summary>
        /// Gets a value indicating if the specified point belongs or not to the elliptic curve
        /// </summary>
        /// <param name="point">The query point</param>
        /// <returns></returns>
        bool Belong(Big2DPoint point);

        /// <summary>
        /// Performs the inverse of a given point
        /// </summary>
        /// <param name="point">The point to inverse</param>
        /// <returns></returns>
        Big2DPoint Inverse(Big2DPoint point);

        /// <summary>
        /// Gets the result value of the addition operation for the givens two points
        /// </summary>
        /// <param name="p1">The first point to add</param>
        /// <param name="p2">The second point to add</param>
        /// <returns></returns>
        Big2DPoint Add(Big2DPoint p1, Big2DPoint p2);

        /// <summary>
        /// Gets the duplicate point's value of a given point, i.e. 2*p
        /// </summary>
        /// <param name="p">The point to duplicate</param>
        /// <returns></returns>
        Big2DPoint Duplicate(Big2DPoint p);

        /// <summary>
        /// Gets a Big2DPoint resulting of the multiplication of a given Big2DPoint whith an BigInteger scalar
        /// </summary>
        /// <param name="point">The Big2DPoint to multiplicate</param>
        /// <param name="scalar">The BigInteger scalar for multiplication</param>
        /// <returns></returns>
        Big2DPoint ScalarMultiplicate(Big2DPoint point, BigInteger scalar);

        /// <summary>
        /// Gets a random Big2DPoint value that belong to the elliptic curve
        /// </summary>
        /// <returns></returns>
        Big2DPoint GetRandomBig2DPoint();
    }
}
