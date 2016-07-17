using System;
using System.IO;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Examples
{
    /**
    * A simple utility class that encrypts/decrypts password based
    * encryption files.
    * <p>
    * To encrypt a file: PBEFileProcessor -e [-ai] fileName passPhrase.<br/>
    * If -a is specified the output file will be "ascii-armored".<br/>
    * If -i is specified the output file will be "integrity protected".</p>
    * <p>
    * To decrypt: PBEFileProcessor -d fileName passPhrase.</p>
    * <p>
    * Note: this example will silently overwrite files, nor does it pay any attention to
    * the specification of "_CONSOLE" in the filename. It also expects that a single pass phrase
    * will have been used.</p>
    */
    public sealed class PbeFileProcessor
    {
        private PbeFileProcessor() {}

        /**
        * decrypt the passed in message stream
        */
        private static void DecryptFile(
            Stream	inputStream,
            char[]	passPhrase)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

			PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
            PgpObject o = pgpF.NextPgpObject();

            //
            // the first object might be a PGP marker packet.
            //
			PgpEncryptedDataList enc = o as PgpEncryptedDataList;
            if (enc == null)
            {
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
            }

            PgpPbeEncryptedData pbe = (PgpPbeEncryptedData)enc[0];

            Stream clear = pbe.GetDataStream(passPhrase);

            PgpObjectFactory pgpFact = new PgpObjectFactory(clear);

			//
			// if we're trying to read a file generated by someone other than us
			// the data might not be compressed, so we check the return type from
			// the factory and behave accordingly.
			//
			o = pgpFact.NextPgpObject();
			if (o is PgpCompressedData)
			{
				PgpCompressedData cData = (PgpCompressedData) o;
				pgpFact = new PgpObjectFactory(cData.GetDataStream());
				o = pgpFact.NextPgpObject();
			}

			PgpLiteralData ld = (PgpLiteralData) o;
			Stream unc = ld.GetInputStream();
            Stream fOut = File.Create(ld.FileName);
			Streams.PipeAll(unc, fOut);
			fOut.Close();

			if (pbe.IsIntegrityProtected())
            {
                if (!pbe.Verify())
                {
                    Console.Error.WriteLine("message failed integrity check");
                }
                else
                {
                    Console.Error.WriteLine("message integrity check passed");
                }
            }
            else
            {
                Console.Error.WriteLine("no message integrity check");
            }
        }

        private static void EncryptFile(
            Stream	outputStream,
            string	fileName,
            char[]	passPhrase,
            bool	armor,
            bool	withIntegrityCheck)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

			MemoryStream bOut = new MemoryStream();

			PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(
				CompressionAlgorithmTag.Zip);

			PgpUtilities.WriteFileToLiteralData(
				comData.Open(bOut),
				PgpLiteralData.Binary,
				new FileInfo(fileName));

			comData.Close();

			byte[] bytes = bOut.ToArray();

			PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(
				SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());

			cPk.AddMethod(passPhrase);

			Stream cOut = cPk.Open(outputStream, bytes.Length);

            cOut.Write(bytes, 0, bytes.Length);

			cOut.Close();

			if (armor)
			{
				outputStream.Close();
			}
        }

		public static void Main(
			string[] args)
        {
            if (args[0].Equals("-e"))
            {
				Stream fos;
                if (args[1].Equals("-a") || args[1].Equals("-ai") || args[1].Equals("-ia"))
                {
                    fos = File.Create(args[2] + ".asc");
                    EncryptFile(fos, args[2], args[3].ToCharArray(), true, (args[1].IndexOf('i') > 0));
                }
                else if (args[1].Equals("-i"))
                {
                    fos = File.Create(args[2] + ".bpg");
                    EncryptFile(fos, args[2], args[3].ToCharArray(), false, true);
                }
                else
                {
                    fos = File.Create(args[1] + ".bpg");
                    EncryptFile(fos, args[1], args[2].ToCharArray(), false, false);
                }
				fos.Close();
            }
            else if (args[0].Equals("-d"))
            {
                Stream fis = File.OpenRead(args[1]);
                DecryptFile(fis, args[2].ToCharArray());
				fis.Close();
            }
            else
            {
                Console.Error.WriteLine("usage: PbeFileProcessor -e [-ai]|-d file passPhrase");
            }
        }
    }
}