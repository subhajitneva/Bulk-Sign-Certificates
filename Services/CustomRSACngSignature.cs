using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Sign_Certificates.Services
{
    public class CustomRSACngSignature : IExternalSignature
    {
        private readonly RSA _privateKey;
        private readonly string _hashAlgorithm;
        private readonly string _encryptionAlgorithm;
        private readonly string _provider;

        public CustomRSACngSignature(RSA privateKey, X509Certificate2 certificate, string hashAlgorithm)
        {
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            _hashAlgorithm = DigestAlgorithms.GetDigest(hashAlgorithm);
            _encryptionAlgorithm = "RSA";
            _provider = certificate.SignatureAlgorithm.FriendlyName;
        }

        public string GetHashAlgorithm() => _hashAlgorithm;

        public string GetEncryptionAlgorithm() => _encryptionAlgorithm;

        public byte[] Sign(byte[] message)
        {
            return _privateKey.SignData(message, new HashAlgorithmName(_hashAlgorithm), RSASignaturePadding.Pkcs1);
        }
    }
}
