using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.Pkcs;
using ECCGenerator;
using System.Text.RegularExpressions;

namespace ConsoleAppTest
{
    class Program
    {
        public static void Main(
            string[] args)
        {
            //SecP384r1, Prime256v1
            
            ECCGenerator.Generator.CreateKeys("Prime256v1", @"C:\Users\ale\Desktop\keys.pem");
            var keys = ECCGenerator.Generator.ReadKeys(@"C:\Users\ale\Desktop\keys.pem");

            var certCAEntry = ECCGenerator.Generator.CreateCACert(keys.Public, keys.Private, 4, "Cert CA", "Cert CA", "OU", "O", "L", "ST", "ca@ca.com");
            ECCGenerator.Generator.GenerateCACertPkcs12(@"C:\Users\ale\Desktop\pkcsCA.p12", "123", @"C:\Users\ale\Desktop\keys.pem", certCAEntry, "Cert CA");

            ECCGenerator.Generator.CreateKeys("Prime256v1", @"C:\Users\ale\Desktop\keys2.pem");
            ECCGenerator.Generator.GenerateUserCertPkcs12(@"C:\Users\ale\Desktop\UserPkcs.p12", "123", @"C:\Users\ale\Desktop\User.cer", @"C:\Users\ale\Desktop\pkcsCA.p12", "123", @"C:\Users\ale\Desktop\keys2.pem", 7, "User Cert", "CNNNNN", "CI", "OU", "O", "L", "ST", "a@a.es");
        }
    }
}
