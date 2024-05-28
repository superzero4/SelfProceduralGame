using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Extensions
{
    public static UnityEngine.Color ToConstantRGB(this string input)
    {
        // Use MD5 to create a hash of the input string
        using (SHA256 alg = SHA256.Create())
        {
            byte[] hashBytes = alg.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            // Aggregate bytes for each color component
            int r = 0, g = 0, b = 0;
            int i = 0;
            int s = hashBytes.Length / 3;
            for (; i < s; i++)
            {
                r += hashBytes[i];
                g += hashBytes[i+1];
                b += hashBytes[i+2];
            }

            int re = hashBytes.Length % 3;
            if (re > 0)
            {
                r += hashBytes[i];
                if (re > 1)
                {
                    b += hashBytes[i + 1];
                }
            }

            // Ensure values are within 0-255 range
            r = r % 256;
            g = g % 256;
            b = b % 256;

            return new Color(r/256f,g/256f,b/256f);
        }
    }
}
