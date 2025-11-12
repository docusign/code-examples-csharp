// <copyright file="ExamplesApiTypeExtensions.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Common
{
    using System.ComponentModel;

    public enum ExamplesApiType
    {
        /// <summary>
        /// Rooms API
        /// </summary>
        [Description("reg")]
        Rooms = 0,

        /// <summary>
        /// ESignature API
        /// </summary>
        [Description("eg")]
        ESignature = 1,

        /// <summary>
        /// Click API
        /// </summary>
        [Description("ceg")]
        Click = 2,

        /// <summary>
        /// Monitor API
        /// </summary>
        [Description("meg")]
        Monitor = 3,

        /// <summary>
        /// Admin API
        /// </summary>
        [Description("aeg")]
        Admin = 4,

        /// <summary>
        /// Connect API
        /// </summary>
        [Description("con")]
        Connect = 5,

        /// <summary>
        /// Web Forms API
        /// </summary>
        [Description("web")]
        WebForms = 6,

        /// <summary>
        /// Notary API
        /// </summary>
        [Description("neg")]
        Notary = 7,

        /// <summary>
        /// Connected Fields
        /// </summary>")]
        [Description("cf")]
        ConnectedFields = 8,

        /// <summary>
        /// Navigator
        /// </summary>")]
        [Description("nav")]
        Navigator = 9,

        /// <summary>
        /// Maestro
        /// </summary>")]
        [Description("mae")]
        Maestro = 10,

        /// <summary>
        /// Workflows
        /// </summary>")]
        [Description("work")]
        Workspaces = 11,
    }

    public static class ExamplesApiTypeExtensions
    {
        public static string ToKeywordString(this ExamplesApiType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
