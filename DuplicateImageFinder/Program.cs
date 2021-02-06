using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace DuplicateImageFinder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var hashedFiles = ProcessFiles();

            var filesToDelete = GetFilesToDelete(hashedFiles);

            foreach (var duplicate in filesToDelete)
            {
                Console.WriteLine($"{duplicate.Path} created at {duplicate.Date.ToShortDateString()}");
            }

            Console.WriteLine($"{filesToDelete.Count()} file(s) deleted. Press any key to continue.");
            Console.ReadKey();
        }

        private static IEnumerable<FileWithHash> ProcessFiles()
        {
            using var sha1 = new SHA1CryptoServiceProvider();
            return
                (from file in GetAllFiles()
                 let byteArray = File.ReadAllBytes(file)
                 let hash = Convert.ToBase64String(sha1.ComputeHash(byteArray))
                 select new FileWithHash
                 {
                     Path = file,
                     Hash = hash,
                     Date = File.GetLastWriteTime(file)
                 }).ToList();
        }

        private static IList<FileWithHash> GetFilesToDelete(IEnumerable<FileWithHash> hashedFiles)
        {
            return hashedFiles
                .GroupBy(f => f.Hash)
                .Where(f => f.Skip(1).Any())
                .SelectMany(c => c)
                .OrderBy(f => f.Date)
                .ThenByDescending(f => f.Path)
                .Skip(1)
                .ToList();
        }

        private static IEnumerable<string> GetAllFiles()
        {
            // Put into config / startup args
            return Directory.GetFiles(@"C:\Users\bob\Desktop\pics\", "*.*");
        }

        private class FileWithHash
        {
            public string Path { get; set; }
            public string Hash { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
