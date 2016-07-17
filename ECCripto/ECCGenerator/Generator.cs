using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto.Parameters;
using System.Collections;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using System.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using ECCGenerator.Properties;
using Org.BouncyCastle.OpenSsl;

namespace ECCGenerator
{
    public enum KeySize { Size256=256,Size384=384,Size521=521};

    public class Generator
    {
        #region CA
        
        /// <summary>
        /// Genera un fichero PKCS#12 para una CA pasando como argumento un objeto de tipo X509CertificateEntry
        /// </summary>
        /// <param name="outputPath">Ruta de salida del fichero PKCS#12</param>
        /// <param name="passwordPkcs">Password que tendrá el fichero PKCS#12</param>
        /// <param name="keysPath">Ruta hacia el fichero que contiene las llaves a usar para generar el PKCS#12</param>
        /// <param name="certificateEntry">Objeto que contiene el certificado auto-firmado de la CA</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el PKCS#12</param>
        public static void GenerateCACertPkcs12(string outputPath, string passwordPkcs, string keysPath, X509CertificateEntry certificateEntry,string friendlyName)
        {
            var keys = ReadKeys(keysPath);
            if (!((ECPublicKeyParameters)certificateEntry.Certificate.GetPublicKey()).Q.Equals(((ECPublicKeyParameters)keys.Public).Q))
                throw new Exception("Llaves públicas diferentes");

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(keys.Private), new X509CertificateEntry[] { certificateEntry });
            
            SaveCertEntry(store, outputPath, passwordPkcs);
        }

        /// <summary>
        /// Genera un fichero PKCS#12 para una CA pasando como argumento la ruta hacia el fichero que contiene el certificado auto-firmado
        /// </summary>
        /// <param name="outputPath">Ruta de salida del fichero PKCS#12</param>
        /// <param name="passwordPkcs">Password que tendrá el fichero PKCS#12</param>
        /// <param name="keysPath">Ruta hacia el fichero que contiene las llaves a usar para generar el PKCS#12</param>
        /// <param name="cerFilePath">Ruta hacia el fichero que contiene el certificado auto-firmado de la CA</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el PKCS#12</param>
        public static void GenerateCACertPkcs12(string outputPath, string passwordPkcs, string keysPath, string cerFilePath, string friendlyName)
        {
            var keys = ReadKeys(keysPath);
            var cert = ReadCert(cerFilePath);
            var certificateEntry = new X509CertificateEntry(cert);

            if (!((ECPublicKeyParameters)certificateEntry.Certificate.GetPublicKey()).Q.Equals(((ECPublicKeyParameters)keys.Public).Q))
                throw new Exception("Llaves públicas diferentes");
            
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(keys.Private), new X509CertificateEntry[] { certificateEntry });

            SaveCertEntry(store, outputPath, passwordPkcs);
        }

        /// <summary>
        /// Genera un objeto X509CertificateEntry auto-firmado
        /// </summary>
        /// <param name="pubKey">Clave pública del certificado</param>
        /// <param name="privKey">Clave privada con que se firmará el certificado</param>
        /// <param name="countValidMonths">Cantidad de meses en que será válido el certificado a partir de la fecha actual</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el certificado</param>
        /// <param name="cn">CN</param>
        /// <param name="ou">OU</param>
        /// <param name="o">O</param>
        /// <param name="l">L</param>
        /// <param name="st">ST</param>
        /// <param name="emailAddress">Email Address</param>
        /// <returns></returns>
        public static X509CertificateEntry CreateCACert(AsymmetricKeyParameter pubKey, AsymmetricKeyParameter privKey, int countValidMonths, string friendlyName, string cn, string ou, string o, string l, string st, string emailAddress)
        {
            var oids = new ArrayList();
            oids.Add(X509Name.CN);
            oids.Add(X509Name.OU);
            oids.Add(X509Name.O);
            oids.Add(X509Name.L);
            oids.Add(X509Name.ST);
            oids.Add(X509Name.EmailAddress);

            var values = new ArrayList();
            values.Add(cn);
            values.Add(ou);
            values.Add(o);
            values.Add(l);
            values.Add(st);
            values.Add(emailAddress);

            //
            // create the certificate - version 1
            //
            Org.BouncyCastle.X509.X509V1CertificateGenerator v1CertGen = new Org.BouncyCastle.X509.X509V1CertificateGenerator();
            Settings.Default.SerialNumberCA++;
            v1CertGen.SetSerialNumber(new BigInteger(Settings.Default.SerialNumberCA.ToString()));
            v1CertGen.SetIssuerDN(new X509Name(oids, values));
            v1CertGen.SetNotBefore(DateTime.UtcNow);
            v1CertGen.SetNotAfter(DateTime.UtcNow.AddMonths(countValidMonths));
            v1CertGen.SetSubjectDN(new X509Name(oids, values));
            v1CertGen.SetPublicKey(pubKey);

            switch (((ECPublicKeyParameters)pubKey).Parameters.Curve.FieldSize)
            {
                case 256:
                    v1CertGen.SetSignatureAlgorithm("SHA256WITHECDSA");
                    break;
                case 384:
                    v1CertGen.SetSignatureAlgorithm("SHA384WITHECDSA");
                    break;
                case 521:
                    v1CertGen.SetSignatureAlgorithm("SHA512WITHECDSA");
                    break;
            }

            Org.BouncyCastle.X509.X509Certificate cert = v1CertGen.Generate(privKey);

            cert.CheckValidity(DateTime.UtcNow);
            cert.Verify(pubKey);

            Hashtable bagAttr = new Hashtable();
            bagAttr.Add(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id, new DerBmpString(friendlyName));

            return new X509CertificateEntry(cert, bagAttr);
        }

        /// <summary>
        /// Genera un fichero que contiene un certificado digital auto-firmado
        /// </summary>
        /// <param name="pubKey">Clave pública del certificado</param>
        /// <param name="privKey">Clave privada con que se firmará el certificado</param>
        /// <param name="outputPath">Ruta de salida del fichero con el certificado auto-firmado</param>
        /// <param name="countValidMonths">Cantidad de meses en que será válido el certificado a partir de la fecha actual</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el certificado</param>
        /// <param name="cn">CN</param>
        /// <param name="ou">OU</param>
        /// <param name="o">O</param>
        /// <param name="l">L</param>
        /// <param name="st">ST</param>
        /// <param name="emailAddress">Email Address</param>
        public static void CreateCACert(AsymmetricKeyParameter pubKey, AsymmetricKeyParameter privKey, string outputPath, int countValidMonths, string friendlyName, string cn, string ou, string o, string l, string st, string emailAddress)
        {
            var certEntry = CreateCACert(pubKey, privKey, countValidMonths, friendlyName, cn, ou, o, l, st, emailAddress);
            SaveCert(certEntry.Certificate, outputPath);
        }

        #endregion

        #region User

        /// <summary>
        ///  Genera un fichero PKCS#12 de usuario pasando como argumento la ruta hacia el fichero que contiene el certificado
        /// </summary>
        /// <param name="outputPath">Ruta de salida del fichero PKCS#12</param>
        /// <param name="passwordPkcs">Password que tendrá el fichero PKCS#12</param>
        /// <param name="keysPath">Ruta hacia el fichero que contiene las llaves a usar para generar el PKCS#12</param>
        /// <param name="cerFilePath">Ruta hacia el fichero que contiene el certificado de usuario</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el PKCS#12</param>
        public static void GenerateUserCertPkcs12(string outputPath, string passwordPkcs, string keysPath, string cerFilePath, string friendlyName)
        {
            var keys = ReadKeys(keysPath);
            var cert = ReadCert(cerFilePath);
            var certificateEntry = new X509CertificateEntry(cert);

            if (!((ECPublicKeyParameters)certificateEntry.Certificate.GetPublicKey()).Q.Equals(((ECPublicKeyParameters)keys.Public).Q))
                throw new Exception("Llaves públicas diferentes");

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(keys.Private), new X509CertificateEntry[] { certificateEntry });

            SaveCertEntry(store, outputPath, passwordPkcs);
        }

        /// <summary>
        ///  Genera un fichero con un certificado digital de usuario y su correspondiente fichero PKCS#12
        /// </summary>
        /// <param name="outputPathP12">Ruta de salida del fichero PKCS#12</param>
        /// <param name="passwordPkcs">Password que tendrá el fichero PKCS#12</param>
        /// <param name="outputPathCert">Ruta de salida del fichero con el certificado</param>
        /// <param name="CAPathPkcs12">Ruta hacia el fichero PKCS#12 que contiene el certificado digital de la CA</param>
        /// <param name="passwordCAPkcs12">Password del fichero PKCS#12 de la CA</param>
        /// <param name="keysPath">Ruta hacia el fichero que contiene las llaves a usar para generar el PKCS#12</param>
        /// <param name="countValidMonths">Cantidad de meses en que será válido el certificado a partir de la fecha actual</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el PKCS#12</param>
        /// <param name="cn">CN</param>
        /// <param name="ci">CI</param>
        /// <param name="ou">OU</param>
        /// <param name="o">O</param>
        /// <param name="l">L</param>
        /// <param name="st">ST</param>
        /// <param name="emailAddress">Email Address</param>
        public static void GenerateUserCertPkcs12(string outputPathP12, string passwordPkcs, string outputPathCert, string CAPathPkcs12, string passwordCAPkcs12, string keysPath, int countValidMonths, string friendlyName, string cn, string ci, string ou, string o, string l, string st, string emailAddress)
        {
            var keys = ReadKeys(keysPath);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            var caStore = new Pkcs12Store(new FileStream(CAPathPkcs12, FileMode.Open), passwordCAPkcs12.ToCharArray());
            foreach (string item in caStore.Aliases)
            {
                var cert = caStore.GetCertificate(item).Certificate;
                if (cert.SubjectDN.ToString() == cert.IssuerDN.ToString())//is self-signed
                {
                    var caKeyPublic = cert.GetPublicKey();
                    var caKeyPrivate = caStore.GetKey(item).Key;
                    var pubEntry = CreateUserCert(cert.IssuerDN, keys.Public, caKeyPrivate, caKeyPublic, countValidMonths, friendlyName, null, cn, ci, ou, o, l, st, emailAddress);

                    store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(keys.Private), new X509CertificateEntry[] { pubEntry });

                    SaveCert(pubEntry.Certificate, outputPathCert);
                    SaveCertEntry(store, outputPathP12, passwordPkcs);
                    break;
                }
            }
        }

        
        private static X509CertificateEntry CreateUserCert(X509Name issuer, AsymmetricKeyParameter pubKey, AsymmetricKeyParameter caPrivKey, AsymmetricKeyParameter caPubKey, int countValidMonths, string friendlyName, string signatureAlgorithm, string cn, string ci, string ou, string o, string l, string st, string emailAddress)
        {
            var oids = new ArrayList();
            oids.Add(X509Name.CN);
            oids.Add(X509Name.Initials);
            oids.Add(X509Name.OU);
            oids.Add(X509Name.O);
            oids.Add(X509Name.L);
            oids.Add(X509Name.ST);
            oids.Add(X509Name.EmailAddress);

            var values = new ArrayList();
            values.Add(cn);
            values.Add(ci);
            values.Add(ou);
            values.Add(o);
            values.Add(l);
            values.Add(st);
            values.Add(emailAddress);

            return CreateUserCert(issuer, pubKey, caPrivKey, caPubKey, countValidMonths, friendlyName, new X509Name(oids, values), signatureAlgorithm);
        }

        private static void CreateUserCert(string outputPath, X509Name issuer, AsymmetricKeyParameter pubKey, AsymmetricKeyParameter caPrivKey, AsymmetricKeyParameter caPubKey, int countValidMonths, string friendlyName, string signatureAlgorithm, string cn, string ci, string ou, string o, string l, string st, string emailAddress)
        {
            var oids = new ArrayList();
            oids.Add(X509Name.CN);
            oids.Add(X509Name.Initials);
            oids.Add(X509Name.OU);
            oids.Add(X509Name.O);
            oids.Add(X509Name.L);
            oids.Add(X509Name.ST);
            oids.Add(X509Name.EmailAddress);

            var values = new ArrayList();
            values.Add(cn);
            values.Add(ci);
            values.Add(ou);
            values.Add(o);
            values.Add(l);
            values.Add(st);
            values.Add(emailAddress);

            var certEntry= CreateUserCert(issuer, pubKey, caPrivKey, caPubKey, countValidMonths, friendlyName, new X509Name(oids, values), signatureAlgorithm);
            SaveCert(certEntry.Certificate, outputPath);
        }

        
        /// <summary>
        /// Genera un objeto X509CertificateEntry de usuario a partir de una solicitud CSR
        /// </summary>
        /// <param name="CAPathPkcs12">Ruta hacia el fichero PKCS#12 que contiene el certificado digital de la CA</param>
        /// <param name="passwordCAPkcs12">Password del fichero PKCS#12 de la CA</param>
        /// <param name="countValidMonths">Cantidad de meses en que será válido el certificado a partir de la fecha actual</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el certificado</param>
        /// <param name="csrFilePath">Ruta hacia el fichero de solicitud CSR</param>
        /// <returns></returns>
        public static X509CertificateEntry CreateUserCert(string CAPathPkcs12, string passwordCAPkcs12, int countValidMonths, string friendlyName, string csrFilePath)
        {
            AsymmetricKeyParameter caPublic = null, caPrivate = null;
            X509Name issuer = null;

            #region getting information about CA
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            var caStore = new Pkcs12Store(new FileStream(CAPathPkcs12, FileMode.Open), passwordCAPkcs12.ToCharArray());
            foreach (string item in caStore.Aliases)
            {
                var cert = caStore.GetCertificate(item).Certificate;
                if (cert.SubjectDN.ToString() == cert.IssuerDN.ToString())//is self-signed
                {
                    caPublic = cert.GetPublicKey();
                    caPrivate = caStore.GetKey(item).Key;
                    issuer = cert.IssuerDN;
                    break;
                }
            }
            #endregion

            var csr=ReadCSRFile(csrFilePath);
            var subject = csr.GetCertificationRequestInfo().Subject;
            var pubKey = csr.GetPublicKey();

            return CreateUserCert(issuer, pubKey, caPrivate, caPublic, countValidMonths, friendlyName, subject, csr.SignatureAlgorithm.ObjectID.ToString());
        }

        /// <summary>
        /// Genera un fichero con un certificado digital de usuario a partir de una solicitud CSR
        /// </summary>
        /// <param name="outputPath">Ruta de salida del fichero con el certificado</param>
        /// <param name="CAPathPkcs12">Ruta hacia el fichero PKCS#12 que contiene el certificado digital de la CA</param>
        /// <param name="passwordCAPkcs12">Password del fichero PKCS#12 de la CA</param>
        /// <param name="countValidMonths">Cantidad de meses en que será válido el certificado a partir de la fecha actual</param>
        /// <param name="friendlyName">Nombre descriptivo que tendrá el certificado</param>
        /// <param name="csrFilePath">Ruta hacia el fichero de solicitud CSR</param>
        public static void CreateUserCert(string outputPath,string CAPathPkcs12, string passwordCAPkcs12, int countValidMonths, string friendlyName, string csrFilePath)
        {
            AsymmetricKeyParameter caPublic = null, caPrivate = null;
            X509Name issuer = null;

            #region getting information about CA
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            var caStore = new Pkcs12Store(new FileStream(CAPathPkcs12, FileMode.Open), passwordCAPkcs12.ToCharArray());
            foreach (string item in caStore.Aliases)
            {
                var cert = caStore.GetCertificate(item).Certificate;
                if (cert.SubjectDN.ToString() == cert.IssuerDN.ToString())//is self-signed
                {
                    caPublic = cert.GetPublicKey();
                    caPrivate = caStore.GetKey(item).Key;
                    issuer = cert.IssuerDN;
                    break;
                }
            }
            #endregion

            var csr = ReadCSRFile(csrFilePath);
            var subject = csr.GetCertificationRequestInfo().Subject;
            var pubKey = csr.GetPublicKey();

            var certEntry= CreateUserCert(issuer, pubKey, caPrivate, caPublic, countValidMonths, friendlyName, subject, csr.SignatureAlgorithm.ObjectID.ToString());
            SaveCert(certEntry.Certificate, outputPath);
        }

        
        private static X509CertificateEntry CreateUserCert(X509Name issuer, AsymmetricKeyParameter pubKey, AsymmetricKeyParameter caPrivKey, AsymmetricKeyParameter caPubKey, int countValidMonths, string friendlyName, X509Name subject, string signatureAlgorithm)
        {
            //
            // create the certificate - version 3
            //
            X509V3CertificateGenerator v3CertGen = new X509V3CertificateGenerator();
            Settings.Default.SerialNumberUser++;
            v3CertGen.SetSerialNumber(new BigInteger(Settings.Default.SerialNumberUser.ToString()));
            v3CertGen.SetIssuerDN(issuer);
            v3CertGen.SetNotBefore(DateTime.UtcNow);
            v3CertGen.SetNotAfter(DateTime.UtcNow.AddMonths(countValidMonths));
            v3CertGen.SetSubjectDN(subject);
            v3CertGen.SetPublicKey(pubKey);

            switch (((ECPublicKeyParameters)pubKey).Parameters.Curve.FieldSize)
            {
                case 256:
                    v3CertGen.SetSignatureAlgorithm("SHA256WITHECDSA");
                    break;
                case 384:
                    v3CertGen.SetSignatureAlgorithm("SHA384WITHECDSA");
                    break;
                case 521:
                    v3CertGen.SetSignatureAlgorithm("SHA512WITHECDSA");
                    break;
            }

            //
            // add the extensions
            //
            v3CertGen.AddExtension(
                X509Extensions.SubjectKeyIdentifier,
                false,
                new SubjectKeyIdentifierStructure(pubKey));

            v3CertGen.AddExtension(
                X509Extensions.AuthorityKeyIdentifier,
                false,
                new AuthorityKeyIdentifierStructure(caPubKey));

            Org.BouncyCastle.X509.X509Certificate cert = v3CertGen.Generate(caPrivKey);

            cert.CheckValidity(DateTime.UtcNow);

            cert.Verify(caPubKey);

            Hashtable bagAttr = new Hashtable();
            bagAttr.Add(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id, new DerBmpString(friendlyName));
            bagAttr.Add(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id, new SubjectKeyIdentifierStructure(pubKey));

            return new X509CertificateEntry(cert, bagAttr);
        }

        #endregion

        #region Common

        /// <summary>
        /// Lee el contenido de un fichero que contiene un par de llaves y lo retorna como un objeto AsymmetricCipherKeyPair
        /// </summary>
        /// <param name="keysPath">Ruta hacia el fichero con las llaves</param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair ReadKeys(string keysPath)
        {
            var streamReader = new StreamReader(keysPath);
            var PemReader = new PemReader(streamReader);
            var keys = (AsymmetricCipherKeyPair)PemReader.ReadObject();
            streamReader.Close();
            return keys;
        }

        /// <summary>
        /// Genera un fichero de solicitud CSR y un par de llaves asociadas
        /// </summary>
        /// <param name="outputCSRPath">Ruta de salida del fichero CSR</param>
        /// <param name="outputKeysPath">Ruta de salida de las llaves</param>
        /// <param name="curveName">Nombre de la curva elíptica estándar a utilizar</param>
        /// <param name="cn">CN</param>
        /// <param name="ci">CI</param>
        /// <param name="ou">OU</param>
        /// <param name="o">O</param>
        /// <param name="l">L</param>
        /// <param name="st">ST</param>
        /// <param name="emailAddress">Email Address</param>
        public static void GenerateCSRKeysFile(string outputCSRPath,string outputKeysPath,string curveName, string cn, string ci, string ou, string o, string l, string st, string emailAddress)
        {
            var keys = CreateKeys(curveName);

            var oids = new ArrayList();
            oids.Add(X509Name.CN);
            oids.Add(X509Name.Initials);
            oids.Add(X509Name.OU);
            oids.Add(X509Name.O);
            oids.Add(X509Name.L);
            oids.Add(X509Name.ST);
            oids.Add(X509Name.EmailAddress);

            var values = new ArrayList();
            values.Add(cn);
            values.Add(ci);
            values.Add(ou);
            values.Add(o);
            values.Add(l);
            values.Add(st);
            values.Add(emailAddress);

            var signature = "";

            switch (((ECPublicKeyParameters)keys.Public).Parameters.Curve.FieldSize)
            {
                case 256:
                    signature = "SHA256WITHECDSA";
                    break;
                case 384:
                    signature = "SHA384WITHECDSA";
                    break;
                case 521:
                    signature = "SHA512WITHECDSA";
                    break;
            }

            var csr = new Pkcs10CertificationRequest(signature, new X509Name(oids, values), keys.Public, null, keys.Private);
            
            var streamWriter = new StreamWriter(outputCSRPath);
            var PEMWriter = new PemWriter(streamWriter);
            PEMWriter.WriteObject(csr);
            streamWriter.Close();

            var privateStreamWriter = new StreamWriter(outputKeysPath);
            var privatePEMWriter = new PemWriter(privateStreamWriter);
            privatePEMWriter.WriteObject(keys.Private);
            privateStreamWriter.Close();
        }

        /// <summary>
        /// Genera un par de llaves a partir del nombre de una curva elíptica estándar
        /// </summary>
        /// <param name="curveName">Nombre de la curva elíptica estándar</param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair CreateKeys(string curveName)
        {
            var gen = new ECKeyPairGenerator();
            var strength = ECKeyPairGenerator.FindECCurveByName(curveName).Curve.FieldSize;
            gen.Init(new KeyGenerationParameters(new SecureRandom(), strength), curveName);
            var keys = gen.GenerateKeyPairExternAlgorithm();
            return keys;
        }

        /// <summary>
        /// Genera un fichero con un par de llaves a partir del nombre de una curva elíptica estándar
        /// </summary>
        /// <param name="curveName">Nombre de la curva elíptica estándar</param>
        /// <param name="outputPath">Ruta de salida del fichero con las llaves</param>
        public static void CreateKeys(string curveName, string outputPath)
        {
            var keys=CreateKeys(curveName);

            var streamWriter = new StreamWriter(outputPath);
            var PEMWriter = new PemWriter(streamWriter);
            PEMWriter.WriteObject(keys.Private);
            streamWriter.Close();
        }

        /// <summary>
        /// Lee el contenido de un fichero que contiene una solicitud CSR y lo retorna como un objeto Pkcs10CertificationRequest
        /// </summary>
        /// <param name="path">Ruta hacia el fichero de solicitud CSR</param>
        /// <returns></returns>
        public static Pkcs10CertificationRequest ReadCSRFile(string path)
        {
            var streamReader = new StreamReader(path);
            var PemReader = new PemReader(streamReader);
            var csr = (Pkcs10CertificationRequest)PemReader.ReadObject();
            streamReader.Close();
            return csr;
        }


        private static void SaveCertEntry(Pkcs12Store store, string outputPath, string passwordPkcs)
        {
            Stream outputStream = File.Create(outputPath);
            store.Save(outputStream, passwordPkcs.ToCharArray(), new SecureRandom());
            outputStream.Close();
            Settings.Default.Save();
        }

        
        private static void SaveCert(Org.BouncyCastle.X509.X509Certificate cert, string outputPath)
        {
            //Stream outputStream = File.Create(outputPath);
            File.WriteAllBytes(outputPath, cert.GetEncoded());
            //outputStream.Close();
        }

        private static Org.BouncyCastle.X509.X509Certificate ReadCert(string path)
        {
            var stream=File.OpenRead(path);
            var bytes=new byte[stream.Length];
            stream.Read(bytes,0,bytes.Length);

            var parser = new Org.BouncyCastle.X509.X509CertificateParser();
            return parser.ReadCertificate(bytes);
        }

        #endregion
    }
}
