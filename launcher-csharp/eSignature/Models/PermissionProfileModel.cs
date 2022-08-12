// <copyright file="PermissionProfileModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    public class PermissionProfileModel
    {
        public string ProfileId { get; set; }

        public string ProfileName { get; set; }

        public AccountRoleSettingsModel AccountRoleSettingsModel { get; set; }
    }
}
