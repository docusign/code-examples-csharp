// <copyright file="ExamplesAPIType.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace DocuSign.CodeExamples.Common
{
    public enum ExamplesAPIType
    {
        [Description("reg")]
        Rooms = 0,

        [Description("eg")]
        ESignature = 1,

        [Description("ceg")]
        Click = 2,

        [Description("meg")]
        Monitor = 3,

        [Description("aeg")]
        Admin = 4,
    }

    public static class ExamplesAPITypeExtensions
    {
        public static string ToKeywordString(this ExamplesAPIType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
