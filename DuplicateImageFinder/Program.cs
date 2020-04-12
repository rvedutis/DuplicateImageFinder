using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace DuplicateImageFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var hashedFiles = new List<FileWithHash>();
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                foreach (var file in GetAllFiles())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var byteArray = File.ReadAllBytes(file);
                        var hash = Convert.ToBase64String(sha1.ComputeHash(byteArray));

                        hashedFiles.Add(new FileWithHash
                        {
                            Path = file,
                            Hash = hash
                        });
                    }
                }
            }

            var duplicates = hashedFiles.GroupBy(f => f.Hash).Where(f => f.Skip(1).Any()).SelectMany(c => c);

            foreach (var duplicate in duplicates)
            {
                Console.WriteLine(duplicate.Path);
            }
            
            Console.WriteLine("Done. Press any key to continue.");

            Console.ReadKey();
        }

        private static string[] GetAllFiles()
        {
            return Directory.GetFiles(@"C:\Users\Home\Desktop\2017-08\", "*.jpdg");
        }

        private class FileWithHash
        {
            public string Path { get; set; }
            public string Hash { get; set; }
        }
    }
}
