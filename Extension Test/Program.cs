using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

string file = @"E:\Downloads\bla\ext.pem";
string file2 = @"E:\Downloads\bla\pubKey.pem";
string content = File.ReadAllText(file);
content = content.Replace("-----BEGIN PRIVATE KEY-----", "");
content = content.Replace("-----END PRIVATE KEY-----", "");

var rsa = RSA.Create();
rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(content), out _);

var publicKey = rsa.ExportSubjectPublicKeyInfo();
string pk = "";
foreach (var b in publicKey)
{
    pk += b.ToString();
}
File.WriteAllBytes(file2, publicKey);