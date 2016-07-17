using System;
using System.Text;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>A generic TLS 1.0 block cipher suite. This can be used for AES or 3DES for example.</remarks>
	internal class TlsBlockCipherCipherSuite
		: TlsCipherSuite
	{
		private IBlockCipher encryptCipher;

		private IBlockCipher decryptCipher;

		private IDigest writeDigest;

		private IDigest readDigest;

		private int cipherKeySize;

		private short keyExchange;

		private TlsMac writeMac;

		private TlsMac readMac;

		internal TlsBlockCipherCipherSuite(
			IBlockCipher	encrypt,
			IBlockCipher	decrypt,
			IDigest			writeDigest,
			IDigest			readDigest,
			int				cipherKeySize,
			short			keyExchange)
		{
			this.encryptCipher = encrypt;
			this.decryptCipher = decrypt;
			this.writeDigest = writeDigest;
			this.readDigest = readDigest;
			this.cipherKeySize = cipherKeySize;
			this.keyExchange = keyExchange;
		}

		internal override void Init(byte[] ms, byte[] cr, byte[] sr)
		{
			int prfSize = (2 * cipherKeySize) + (2 * writeDigest.GetDigestSize())
				+ (2 * encryptCipher.GetBlockSize());
			byte[] key_block = new byte[prfSize];
			byte[] random = new byte[cr.Length + sr.Length];
			Array.Copy(cr, 0, random, sr.Length, cr.Length);
			Array.Copy(sr, 0, random, 0, sr.Length);
			TlsUtilities.PRF(ms, "key expansion", random, key_block);

			int offset = 0;

			// Init MACs
			writeMac = new TlsMac(writeDigest, key_block, offset, writeDigest
				.GetDigestSize());
			offset += writeDigest.GetDigestSize();
			readMac = new TlsMac(readDigest, key_block, offset, readDigest
				.GetDigestSize());
			offset += readDigest.GetDigestSize();

			// Init Ciphers
			this.initCipher(true, encryptCipher, key_block, cipherKeySize, offset,
				offset + (cipherKeySize * 2));
			offset += cipherKeySize;
			this.initCipher(false, decryptCipher, key_block, cipherKeySize, offset,
				offset + cipherKeySize + decryptCipher.GetBlockSize());
		}

		private void initCipher(bool forEncryption, IBlockCipher cipher,
								byte[] key_block, int key_size, int key_offset, int iv_offset)
		{
			KeyParameter key_parameter = new KeyParameter(key_block, key_offset,
				key_size);
			ParametersWithIV parameters_with_iv = new ParametersWithIV(
				key_parameter, key_block, iv_offset, cipher.GetBlockSize());
			cipher.Init(forEncryption, parameters_with_iv);
		}

		internal override byte[] EncodePlaintext(
			short				type,
			byte[]				plaintext,
			int					offset,
			int					len,
			TlsProtocolHandler	handler)
		{
			int blocksize = encryptCipher.GetBlockSize();

			// Add a random number of extra blocks worth of padding
			int minPaddingSize = blocksize - ((len + writeMac.Size + 1) % blocksize);
			int maxExtraPadBlocks = (255 - minPaddingSize) / blocksize;
			int actualExtraPadBlocks = chooseExtraPadBlocks(handler.Random, maxExtraPadBlocks);
			int paddingsize = minPaddingSize + (actualExtraPadBlocks * blocksize);

			int totalsize = len + writeMac.Size + paddingsize + 1;
			byte[] outbuf = new byte[totalsize];
			Array.Copy(plaintext, offset, outbuf, 0, len);
			byte[] mac = writeMac.CalculateMac(type, plaintext, offset, len);
			Array.Copy(mac, 0, outbuf, len, mac.Length);
			int paddoffset = len + mac.Length;
			for (int i = 0; i <= paddingsize; i++)
			{
				outbuf[i + paddoffset] = (byte)paddingsize;
			}
			for (int i = 0; i < totalsize; i += blocksize)
			{
				encryptCipher.ProcessBlock(outbuf, i, outbuf, i);
			}
			return outbuf;
		}

		private int chooseExtraPadBlocks(SecureRandom r, int max)
		{
//			return r.NextInt(max + 1);

			int x = r.NextInt();
			int n = lowestBitSet(x);
			return System.Math.Min(n, max);
		}

		private int lowestBitSet(int x)
		{
			if (x == 0)
				return 32;

			int n = 0;
			while ((x & 1) == 0)
			{
				++n;
				x >>= 1;
			}
			return n;
		}

		internal override byte[] DecodeCiphertext(
			short				type,
			byte[]				ciphertext,
			int					offset,
			int					len,
			TlsProtocolHandler	handler)
		{
			// TODO TLS 1.1 (RFC 4346) introduces an explicit IV

			int minLength = readMac.Size + 1;
			int blocksize = decryptCipher.GetBlockSize();
			bool decrypterror = false;

			/*
			* ciphertext must be at least (macsize + 1) bytes long
			*/
			if (len < minLength)
			{
				handler.FailWithError(TlsProtocolHandler.AL_fatal, TlsProtocolHandler.AP_decode_error);
			}

			/*
			* ciphertext must be a multiple of blocksize
			*/
			if (len % blocksize != 0)
			{
				handler.FailWithError(TlsProtocolHandler.AL_fatal, TlsProtocolHandler.AP_decryption_failed);
			}

			/*
			* Decrypt all the ciphertext using the blockcipher
			*/
			for (int i = 0; i < len; i += blocksize)
			{
				decryptCipher.ProcessBlock(ciphertext, i + offset, ciphertext, i
					+ offset);
			}

			/*
			* Check if padding is correct
			*/
			int lastByteOffset = offset + len - 1;

			byte paddingsizebyte = ciphertext[lastByteOffset];

			// Note: interpret as unsigned byte
			int paddingsize = paddingsizebyte;

			int maxPaddingSize = len - minLength;
			if (paddingsize > maxPaddingSize)
			{
				decrypterror = true;
				paddingsize = 0;
			}
			else
			{
				/*
				* Now, check all the padding-bytes (constant-time comparison).
				*/
				int diff = 0;
				for (int i = lastByteOffset - paddingsize; i < lastByteOffset; ++i)
				{
					diff |= (ciphertext[i] ^ paddingsizebyte);
				}
				if (diff != 0)
				{
					/* Wrong padding */
					decrypterror = true;
					paddingsize = 0;
				}
			}

			/*
			* We now don't care if padding verification has failed or not,
			* we will calculate the mac to give an attacker no kind of timing
			* profile he can use to find out if mac verification failed or
			* padding verification failed.
			*/
			int plaintextlength = len - minLength - paddingsize;
			byte[] calculatedMac = readMac.CalculateMac(type, ciphertext, offset,
				plaintextlength);

			/*
			* Check all bytes in the mac (constant-time comparison).
			*/
			byte[] decryptedMac = new byte[calculatedMac.Length];
			Array.Copy(ciphertext, offset + plaintextlength, decryptedMac, 0, calculatedMac.Length);

			if (!Arrays.ConstantTimeAreEqual(calculatedMac, decryptedMac))
			{
				decrypterror = true;
			}

			/*
			* Now, it is safe to fail.
			*/
			if (decrypterror)
			{
				handler.FailWithError(TlsProtocolHandler.AL_fatal,
					TlsProtocolHandler.AP_bad_record_mac);
			}
			byte[] plaintext = new byte[plaintextlength];
			Array.Copy(ciphertext, offset, plaintext, 0, plaintextlength);
			return plaintext;

		}

		internal override short KeyExchangeAlgorithm
		{
			get { return this.keyExchange; }
		}
	}
}
