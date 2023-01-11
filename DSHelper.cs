// <copyright file="DSHelper.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Common
{
    using System.IO;
    using System.Runtime.InteropServices;

    internal class DSHelper
    {
        internal static string PrepareFullPrivateKeyFilePath(string fileName)
        {
            const string DefaultRSAPrivateKeyFileName = "private.key";

            var fileNameOnly = Path.GetFileName(fileName);
            if (string.IsNullOrEmpty(fileNameOnly))
            {
                fileNameOnly = DefaultRSAPrivateKeyFileName;
            }

            var filePath = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Directory.GetCurrentDirectory();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && Directory.GetCurrentDirectory().Contains("bin"))
            {
                fileNameOnly = DefaultRSAPrivateKeyFileName;
                filePath = Path.GetFullPath(filePath);
            }

            return Path.Combine(filePath, fileNameOnly);
        }

        internal static byte[] ReadFileContent(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
