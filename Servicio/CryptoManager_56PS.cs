using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services_625NS
{
    public static class CryptoManager_56PS
    {
        public static string Encriptar(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
                byte[] hash = sha256.ComputeHash(bytes);

                // Convertir a string hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private static readonly string _clave = "ClaveDeSeguridadParaEncriptacionNoModificar_56PS"; // Cambiar por una clave fuerte

        public static string EncriptarReversible(string texto)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = DerivarClave(_clave);
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                {
                    // Guardamos el IV al inicio
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs =
                        new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(texto);
                        cs.Write(bytes, 0, bytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private static byte[] DerivarClave(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }


        public static string DesencriptarReversible(string textoEncriptado)
        {
            byte[] data = Convert.FromBase64String(textoEncriptado);

            using (Aes aes = Aes.Create())
            {
                aes.Key = DerivarClave(_clave);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Extraer IV (primeros 16 bytes)
                byte[] iv = new byte[16];
                Array.Copy(data, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs =
                        new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, iv.Length, data.Length - iv.Length);
                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
