// <copyright file="AccountRoleSettingsExtension.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    using DocuSign.eSign.Model;

    /// <summary>
    /// Temporary subclass for AccountRoleSettings
    /// This class is needed for now until DCM-3905 is ready
    /// </summary>
    public class AccountRoleSettingsExtension : AccountRoleSettings
    {
        [System.Runtime.Serialization.DataMember(Name = "signingUIVersion", EmitDefaultValue = false)]
        public string SigningUiVersion { get; set; }
    }
}
