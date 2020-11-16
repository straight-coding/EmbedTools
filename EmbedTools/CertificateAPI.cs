﻿//Originated from 
//  https://github.com/rlipscombe/bouncy-castle-csharp/blob/master/CreateCertificate/Program.cs
//    with minor modifications
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using X509KeyStorageFlags = System.Security.Cryptography.X509Certificates.X509KeyStorageFlags;
using X509ContentType = System.Security.Cryptography.X509Certificates.X509ContentType;
using X509Store = System.Security.Cryptography.X509Certificates.X509Store;

using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using System.Text;
using System.Diagnostics;

namespace CreateCertificate
{
    public class CertificateAPI
    {
        private static int ShowUsage()
        {
            Console.WriteLine("CreateCertificate self subject-name subject.pfx");
            Console.WriteLine("CreateCertificate ca subject-name CA.pfx");
            Console.WriteLine("CreateCertificate issue CA.pfx subject-name subject.pfx");

            return -1;
        }

        private static int MainTest(string[] args)
        {
            if (args.Length < 1)
            {
                return ShowUsage();
            }

            var mode = args[0];
            switch (mode.ToLower())
            {
                case "self":
                    {
                        if (args.Length != 3)
                            return ShowUsage();

                        var subjectName = args[1];
                        var outputFileName = args[2];

                        var certificate = CreateSelfSignedCertificate(512, subjectName, new[] {"server", "server.mydomain.com"}, new[] {KeyPurposeID.IdKPServerAuth});
                        ExportPfx(certificate, outputFileName, "password");
                        return 0;
                    }

                case "ca":
                    {
                        if (args.Length != 3)
                            return ShowUsage();

                        var subjectName = args[1];
                        var outputFileName = args[2];

                        var certificate = CreateCertificateAuthorityCertificate(2048, subjectName, null, null);
                        ExportPfx(certificate, outputFileName, "password");
                        return 0;
                    }

                case "issue":
                    {
                        if (args.Length != 4)
                            return ShowUsage();

                        var issuerFileName = args[1];
                        var subjectName = args[2];
                        var outputFileName = args[3];

                        var issuerCertificate = LoadCertificate(issuerFileName, "password");
                        var certificate = IssueCertificate(512, subjectName, issuerCertificate, new[] { "server", "server.mydomain.com" }, new[] { KeyPurposeID.IdKPServerAuth });
                        ExportPfx(certificate, outputFileName, "password");

                        return 0;
                    }

                default:
                    return ShowUsage();
            }
        }

        public static X509Certificate2 LoadCertificate(string issuerFileName, string password)
        {
            // We need to pass 'Exportable', otherwise we can't get the private key.
            var issuerCertificate = new X509Certificate2(issuerFileName, password, X509KeyStorageFlags.Exportable);
            return issuerCertificate;
        }

        public static X509Certificate2 IssueCertificate(int nKeySize, string subjectName, X509Certificate2 issuerCertificate, string[] subjectAlternativeNames, KeyPurposeID[] usages)
        {
            //int nKeySize = 2048;

            // It's self-signed, so these are the same.
            var issuerName = issuerCertificate.Subject;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, nKeySize);

            var issuerKeyPair = DotNetUtilities.GetKeyPair(issuerCertificate.PrivateKey);

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = new BigInteger(issuerCertificate.GetSerialNumber());

            const bool isCertificateAuthority = false;
            var certificate = GenerateCertificate(random, subjectName, subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages);
            return ConvertCertificate(certificate, subjectKeyPair, random);
        }

        public static X509Certificate2 CreateCertificateAuthorityCertificate(int nKeySize, string subjectName, string[] subjectAlternativeNames, KeyPurposeID[] usages)
        {
            //int nKeySize = 2048;

            // It's self-signed, so these are the same.
            var issuerName = subjectName;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, nKeySize);

            // It's self-signed, so these are the same.
            var issuerKeyPair = subjectKeyPair;

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = serialNumber; // Self-signed, so it's the same serial number.

            const bool isCertificateAuthority = true;
            var certificate = GenerateCertificate(random, subjectName, subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages);
            return ConvertCertificate(certificate, subjectKeyPair, random);
        }

        public static X509Certificate2 CreateSelfSignedCertificate(int nKeySize, string subjectName, string[] subjectAlternativeNames, KeyPurposeID[] usages)
        {
            //int nKeySize = 2048;

            // It's self-signed, so these are the same.
            var issuerName = subjectName;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, nKeySize);

            // It's self-signed, so these are the same.
            var issuerKeyPair = subjectKeyPair;

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = serialNumber; // Self-signed, so it's the same serial number.

            const bool isCertificateAuthority = false;
            var certificate = GenerateCertificate(random, subjectName, subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages);
            return ConvertCertificate(certificate, subjectKeyPair, random);
        }

        private static SecureRandom GetSecureRandom()
        {
            // Since we're on Windows, we'll use the CryptoAPI one (on the assumption
            // that it might have access to better sources of entropy than the built-in
            // Bouncy Castle ones):
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            return random;
        }

        public static X509Certificate GenerateCertificate(SecureRandom random,
                                                           string subjectName,
                                                           AsymmetricCipherKeyPair subjectKeyPair,
                                                           BigInteger subjectSerialNumber,
                                                           string[] subjectAlternativeNames,
                                                           string issuerName,
                                                           AsymmetricCipherKeyPair issuerKeyPair,
                                                           BigInteger issuerSerialNumber,
                                                           bool isCertificateAuthority,
                                                           KeyPurposeID[] usages)
        {
            var certificateGenerator = new X509V3CertificateGenerator();

            certificateGenerator.SetSerialNumber(subjectSerialNumber);

            // Set the signature algorithm. This is used to generate the thumbprint which is then signed
            // with the issuer's private key. We'll use SHA-256, which is (currently) considered fairly strong.
            //const string signatureAlgorithm = "SHA256WithRSA";
            //certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);
            // set the hash algorithm
            ISignatureFactory signatureFactory = null;
            if (isCertificateAuthority)
                signatureFactory = new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha1WithRsaEncryption.ToString(), subjectKeyPair.Private, random);
            else
                signatureFactory = new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(), subjectKeyPair.Private, random);

            var issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);

            // Note: The subject can be omitted if you specify a subject alternative name (SAN).
            var subjectDN = new X509Name(subjectName);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Our certificate needs valid from/to values.
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(20);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // The subject's public key goes in the certificate.
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            AddAuthorityKeyIdentifier(certificateGenerator, issuerDN, issuerKeyPair, issuerSerialNumber);
            AddSubjectKeyIdentifier(certificateGenerator, subjectKeyPair);
            AddBasicConstraints(certificateGenerator, isCertificateAuthority);
            
            if (usages != null && usages.Any())
                AddExtendedKeyUsage(certificateGenerator, usages);

            if (subjectAlternativeNames != null && subjectAlternativeNames.Any())
                AddSubjectAlternativeNames(certificateGenerator, subjectAlternativeNames);

            // The certificate is signed with the issuer's private key.
            var certificate = certificateGenerator.Generate(signatureFactory);//issuerKeyPair.Private, random);
            return certificate;
        }

        /// <summary>
        /// The certificate needs a serial number. This is used for revocation,
        /// and usually should be an incrementing index (which makes it easier to revoke a range of certificates).
        /// Since we don't have anywhere to store the incrementing index, we can just use a random number.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        private static BigInteger GenerateSerialNumber(SecureRandom random)
        {
            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            return serialNumber;
        }

        /// <summary>
        /// Generate a key pair.
        /// </summary>
        /// <param name="random">The random number generator.</param>
        /// <param name="strength">The key length in bits. For RSA, 2048 bits should be considered the minimum acceptable these days.</param>
        /// <returns></returns>
        private static AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, int strength)
        {
            var keyGenerationParameters = new KeyGenerationParameters(random, strength);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            return subjectKeyPair;
        }

        /// <summary>
        /// Add the Authority Key Identifier. According to http://www.alvestrand.no/objectid/2.5.29.35.html, this
        /// identifies the public key to be used to verify the signature on this certificate.
        /// In a certificate chain, this corresponds to the "Subject Key Identifier" on the *issuer* certificate.
        /// The Bouncy Castle documentation, at http://www.bouncycastle.org/wiki/display/JA1/X.509+Public+Key+Certificate+and+Certification+Request+Generation,
        /// shows how to create this from the issuing certificate. Since we're creating a self-signed certificate, we have to do this slightly differently.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="issuerDN"></param>
        /// <param name="issuerKeyPair"></param>
        /// <param name="issuerSerialNumber"></param>
        private static void AddAuthorityKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                      X509Name issuerDN,
                                                      AsymmetricCipherKeyPair issuerKeyPair,
                                                      BigInteger issuerSerialNumber)
        {
            var authorityKeyIdentifierExtension =
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKeyPair.Public),
                    new GeneralNames(new GeneralName(issuerDN)),
                    issuerSerialNumber);
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifierExtension);
        }

        /// <summary>
        /// Add the "Subject Alternative Names" extension. Note that you have to repeat
        /// the value from the "Subject Name" property.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="subjectAlternativeNames"></param>
        private static void AddSubjectAlternativeNames(X509V3CertificateGenerator certificateGenerator,
                                                       IEnumerable<string> subjectAlternativeNames)
        {
            var subjectAlternativeNamesExtension =
                new DerSequence(
                    subjectAlternativeNames.Select(name => new GeneralName(GeneralName.DnsName, name))
                                           .ToArray<Asn1Encodable>());

            certificateGenerator.AddExtension(
                X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension);
        }

        /// <summary>
        /// Add the "Extended Key Usage" extension, specifying (for example) "server authentication".
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="usages"></param>
        private static void AddExtendedKeyUsage(X509V3CertificateGenerator certificateGenerator, KeyPurposeID[] usages)
        {
            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(usages));
        }

        /// <summary>
        /// Add the "Basic Constraints" extension.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="isCertificateAuthority"></param>
        private static void AddBasicConstraints(X509V3CertificateGenerator certificateGenerator,
                                                bool isCertificateAuthority)
        {
            certificateGenerator.AddExtension(
                X509Extensions.BasicConstraints.Id, true, new BasicConstraints(isCertificateAuthority));
        }

        /// <summary>
        /// Add the Subject Key Identifier.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="subjectKeyPair"></param>
        private static void AddSubjectKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                    AsymmetricCipherKeyPair subjectKeyPair)
        {
            var subjectKeyIdentifierExtension =
                new SubjectKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public));
            certificateGenerator.AddExtension(
                X509Extensions.SubjectKeyIdentifier.Id, false, subjectKeyIdentifierExtension);
        }

        public static X509Certificate2 ConvertCertificate(X509Certificate certificate,
                                                           AsymmetricCipherKeyPair subjectKeyPair,
                                                           SecureRandom random)
        {
            // Now to convert the Bouncy Castle certificate to a .NET certificate.
            // See http://web.archive.org/web/20100504192226/http://www.fkollmann.de/v2/post/Creating-certificates-using-BouncyCastle.aspx
            // ...but, basically, we create a PKCS12 store (a .PFX file) in memory, and add the public and private key to that.
            var store = new Pkcs12Store();

            // What Bouncy Castle calls "alias" is the same as what Windows terms the "friendly name".
            string friendlyName = certificate.SubjectDN.ToString();

            // Add the certificate.
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);

            // Add the private key.
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] {certificateEntry});

            // Convert it to an X509Certificate2 object by saving/loading it from a MemoryStream.
            // It needs a password. Since we'll remove this later, it doesn't particularly matter what we use.
            const string password = "password";
            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), random);

            var convertedCertificate =
                new X509Certificate2(stream.ToArray(),
                                     password,
                                     X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return convertedCertificate;
        }

        public static void ExportPfx(X509Certificate2 certificate, string outputPfxFile, string password)
        {
            // This password is the one attached to the PFX file. Use 'null' for no password.
            var bytes = certificate.Export(X509ContentType.Pfx, password);
            File.WriteAllBytes(outputPfxFile, bytes);
        }

        public static Boolean ExportSourceCode(X509Certificate2 certificate, String outCFile)
        {
            try
            {
                var certKeyPair = DotNetUtilities.GetKeyPair(certificate.PrivateKey);

                var textPubWriter = new StringWriter();
                var pemPubWriter = new PemWriter(textPubWriter);
                pemPubWriter.WriteObject(DotNetUtilities.FromX509Certificate(certificate));//cerKp.Public);//, "DESEDE", userPassword.ToCharArray(), new SecureRandom());
                pemPubWriter.Writer.Flush();

                string publicKeyPem = textPubWriter.ToString();

                //encrypted form of PKCS#8 file  
                string privateKeyPem = "";
                if (certificate.HasPrivateKey)
                {
                    Pkcs8Generator pkcs8Gen = new Pkcs8Generator(certKeyPair.Private);//, Pkcs8Generator.PbeSha1_RC4_128);
                    pkcs8Gen.SecureRandom = new SecureRandom();

                    var textKeyWriter = new StringWriter();
                    var pemKeyWriter = new PemWriter(textKeyWriter);
                    pemKeyWriter.WriteObject(pkcs8Gen);//cerKp.Private, "DESEDE", userPassword.ToCharArray(), new SecureRandom());
                    pemKeyWriter.Writer.Flush();

                    privateKeyPem = textKeyWriter.ToString();
                }

                StringBuilder exportCode = new StringBuilder();

                exportCode.AppendLine("#include <stdlib.h>");
                exportCode.AppendLine("#include <stdio.h>");
                exportCode.AppendLine("#include <string.h>");

                exportCode.AppendLine("");

                exportCode.AppendLine(String.Format("//Date: {0}, created by https://github.com/straight-coding/EmbedTools", DateTime.Now.ToString("yyyy-MM-dd")));

                exportCode.AppendLine("");

                exportCode.AppendLine(String.Format("//Issuer: {0}", certificate.Issuer));
                exportCode.AppendLine(String.Format("//Subject: {0}", certificate.Subject));

                exportCode.AppendLine("");

                String[] keyLines = privateKeyPem.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in keyLines)
                {
                    if (line.IndexOf("BEGIN") >= 0)
                        exportCode.AppendLine(String.Format("const char *privkey = \"{0}\\n\"\\", line));
                    else if (line.IndexOf("-END") >= 0)
                        exportCode.AppendLine(String.Format("    \"{0}\\n\";", line));
                    else
                        exportCode.AppendLine(String.Format("    \"{0}\\n\"\\", line));
                }
                exportCode.AppendLine("");

                String[] pubLines = publicKeyPem.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in pubLines)
                {
                    if (line.IndexOf("-BEGIN") >= 0)
                        exportCode.AppendLine(String.Format("const char *cert = \"{0}\\n\"\\", line));
                    else if (line.IndexOf("-END") >= 0)
                        exportCode.AppendLine(String.Format("    \"{0}\\n\";", line));
                    else
                        exportCode.AppendLine(String.Format("    \"{0}\\n\"\\", line));
                }

                File.WriteAllText(outCFile, exportCode.ToString());
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        //addCertToStore(MyRootCAcert, StoreName.Root, StoreLocation.LocalMachine);
        //addCertToStore(MyCert, StoreName.My, StoreLocation.LocalMachine);
        public static bool addCertToStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            bool bRet = false;

            try
            {
                X509Store store = new X509Store(st, sl);
                store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadWrite);
                store.Add(cert);

                store.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return bRet;
        }
    }
}
