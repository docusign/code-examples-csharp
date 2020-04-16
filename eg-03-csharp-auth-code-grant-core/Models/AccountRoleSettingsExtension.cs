using DocuSign.eSign.Model;

namespace eg_03_csharp_auth_code_grant_core.Models
{
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
