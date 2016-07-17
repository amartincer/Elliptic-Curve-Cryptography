using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;
using crypto.Properties;
using System.Runtime.InteropServices;
using System.IO;

namespace Org.BouncyCastle.Crypto.Parameters
{

    public class ECDomainParameters
    {
        internal ECCurve     curve;
        internal byte[]      seed;
        internal ECPoint     g;
        internal BigInteger  n;
        internal BigInteger  h;
        private CountBitsPrimeNumber countBits;

		public ECDomainParameters(ECCurve curve, ECPoint g, BigInteger n): this(curve, g, n, BigInteger.One){}

        public ECDomainParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h):this(curve, g, n, h, null){}

		public ECDomainParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h, byte[] seed)
        {
			if (curve == null)
				throw new ArgumentNullException("curve");
			if (g == null)
				throw new ArgumentNullException("g");
			if (n == null)
				throw new ArgumentNullException("n");
			if (h == null)
				throw new ArgumentNullException("h");

			this.curve = curve;
            this.g = g;
            this.n = n;
            this.h = h;
            this.seed = Arrays.Clone(seed);
        }

        public ECDomainParameters(CountBitsPrimeNumber countBits,bool randomized)
        {
            this.countBits = countBits;
            if (randomized)
            {
                curve = FpCurve.RandomFpCurve(countBits);
                g = ((FpCurve)curve).GetRandomBig2DPoint();
                n = ((FpCurve)curve).Q;
                h = 1;
            }
            else 
            {
                string[] lines = null;
                BigInteger q, a, b, gX, gY;
                switch (countBits)
                {
                    case CountBitsPrimeNumber.n256:
                        lines = Resources.curva256.Split(new char[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case CountBitsPrimeNumber.n384:
                        lines = Resources.curva384.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case CountBitsPrimeNumber.n512:
                        lines = Resources.curva512.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                }
                q = new BigInteger(lines[0]);
                a =new BigInteger(lines[1]);
                b = new BigInteger(lines[2]);
                curve = new FpCurve(q, a, b);
                gX = new BigInteger(lines[4]);
                gY = new BigInteger(lines[5]);
                g = new FpPoint(curve, new FpFieldElement(q, gX), new FpFieldElement(q, gY), false);
                n = new BigInteger(lines[7]);
                h = new BigInteger(lines[8]);
            }
        }

        public CountBitsPrimeNumber CountBits
        {
            get { return countBits; }
        }

		public ECCurve Curve
        {
            get { return curve; }
        }

        public ECPoint G
        {
            get { return g; }
        }

        public BigInteger N
        {
            get { return n; }
        }

        public BigInteger H
        {
            get { return h; }
        }

        public byte[] GetSeed()
        {
			return Arrays.Clone(seed);
        }

		public override bool Equals(
			object obj)
        {
			if (obj == this)
				return true;

			ECDomainParameters other = obj as ECDomainParameters;

			if (other == null)
				return false;

			return Equals(other);
        }

		protected bool Equals(
			ECDomainParameters other)
		{
			return curve.Equals(other.curve)
				&&	g.Equals(other.g)
				&&	n.Equals(other.n)
				&&	h.Equals(other.h)
				&&	Arrays.AreEqual(seed, other.seed);
		}

		public override int GetHashCode()
        {
            return curve.GetHashCode()
				^	g.GetHashCode()
				^	n.GetHashCode()
				^	h.GetHashCode()
				^	Arrays.GetHashCode(seed);
        }
    }
}
