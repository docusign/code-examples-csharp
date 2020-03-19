namespace eg_03_csharp_auth_code_grant_core.Models
{
	public class PermissionProfileModel
	{
		public string ProfileId { get; set; }

		public string ProfileName { get; set; }

		public AccountRoleSettingsModel AccountRoleSettingsModel { get; set; }
	}
}
