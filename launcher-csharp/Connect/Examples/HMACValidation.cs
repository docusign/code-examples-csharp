﻿// <copyright file="HMACValidation.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Connect.Examples
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class HMACValidation
    {
        //ds-snippet-start:Connect1Step1
        public static string ComputeHash(string secret, string payload)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(secret);
            var hmac = new HMACSHA256(bytes);
            bytes = Encoding.UTF8.GetBytes(payload);

            return Convert.ToBase64String(hmac.ComputeHash(bytes));
        }

        public static bool HashIsValid(string secret, string payload, string verify)
        {
            ReadOnlySpan<byte> hashBytes = Convert.FromBase64String(ComputeHash(secret, payload));
            ReadOnlySpan<byte> verifyBytes = Convert.FromBase64String(verify);

            return CryptographicOperations.FixedTimeEquals(hashBytes, verifyBytes);
        }

        //ds-snippet-end:Connect1Step1
    }
}
