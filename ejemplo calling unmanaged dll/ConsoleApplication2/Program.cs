using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            string N = BigInteger.Parse("4142421").ToString();
            string A = BigInteger.Parse("2466576").ToString();
            string B = BigInteger.Parse("668950252").ToString();
            string xG = BigInteger.Parse("668954320252").ToString();
            string yG = BigInteger.Parse("6689504252").ToString();
            string nG = BigInteger.Parse("6689544250252").ToString();
            string h = BigInteger.Parse("6689552730252").ToString();

            byte[] publicX = new byte[10000];
            byte[] publicY = new byte[10000];
            byte[] prvKey = new byte[10000];
            

            //getKeys(N, A, B, xG, yG, nG, h, publicX, publicY, prvKey);
            var s=getPublicX(N, A, B, xG, yG, nG, "1");
        }

        [DllImport("genCertificados.dll", EntryPoint = "@getCurva$qpcpuct2t2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getCurva(string aOrigen, [MarshalAs(UnmanagedType.LPArray)] byte[] aN, [MarshalAs(UnmanagedType.LPArray)] byte[] aA, [MarshalAs(UnmanagedType.LPArray)] byte[] aB);

        [DllImport(@"D:\Visual Studio 2010 projects\csharp\crypto\bin\Debug\genCertificados.dll", EntryPoint = "getKeys", CallingConvention = CallingConvention.StdCall)]
        public static extern void getKeys(string N, string A, string B, string xG, string yG, string nG, string h, [MarshalAs(UnmanagedType.LPArray)] byte[] publicX, [MarshalAs(UnmanagedType.LPArray)] byte[] publicY, [MarshalAs(UnmanagedType.LPArray)] byte[] privateKey);

        [DllImport("genCertificados", EntryPoint = "@getError$qv", CallingConvention = CallingConvention.Cdecl)]
        public static extern string getError();

        [DllImport(@"genCertificados.dll", EntryPoint = "getPublicX", CallingConvention = CallingConvention.Cdecl)]
        public static extern string getPublicX(string N, string A, string B, string xG, string yG, string nG, string h);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern int GetCurrentThreadId();
    }
}
