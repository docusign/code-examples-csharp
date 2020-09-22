using DocuSign.eSign.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DocuSign.CodeExamples.Models
{
	public class AccountRoleSettingsModel
	{
		public AccountRoleSettingsModel()
		{
		}

		public AccountRoleSettingsModel(AccountRoleSettings accountRoleSettings)
		{
			if (accountRoleSettings == null)
				return;

			AllowApiAccess = bool.TryParse(accountRoleSettings.AllowApiAccess, out bool allowApiAccess) && allowApiAccess;
			AllowApiAccessToAccount = bool.TryParse(accountRoleSettings.AllowApiAccessToAccount, out bool allowApiAccessToAccount) && allowApiAccessToAccount;
		}

		[DisplayName("Use new DocuSign experience interface")]
		public bool UseNewDocuSignExperienceInterface { get; set; } = true;

		[DisplayName("Enable sequential signing interface")]
		public bool EnableSequentialSigningInterface { get; set; } = true;

		[DisplayName("Allow bulk sending")]
		public bool AllowBulkSending { get; set; }

		[DisplayName("Allow envelope sending")]
		public bool AllowEnvelopeSending { get; set; }

		[DisplayName("Allow signer attachments")]
		public bool AllowSignerAttachments { get; set; }

		[DisplayName("Allow tagging in send and correct")]
		public bool AllowTaggingInSendAndCorrect { get; set; } = true;

		[DisplayName("Allow wet signing override")]
		public bool AllowWetSigningOverride { get; set; }

		[DisplayName("Enable recipient viewing notifications")]
		public bool EnableRecipientViewingNotifications { get; set; }

		[DisplayName("Receive completed self signed documents as email links")]
		public bool ReceiveCompletedSelfSignedDocumentsAsEmailLinks { get; set; }

		[DisplayName("Use new sending interface")]
		public bool UseNewSendingInterface { get; set; }

		[DisplayName("Allow API access")]
		public bool AllowApiAccess { get; set; }

		[DisplayName("Allow API access to account")]
		public bool AllowApiAccessToAccount { get; set; }

		[DisplayName("Allow API sending on behalf of others")]
		public bool AllowApiSendingOnBehalfOfOthers { get; set; }

		[DisplayName("Allow API sequential signing")]
		public bool AllowApiSequentialSigning { get; set; }

		[DisplayName("Enable API request logging")]
		public bool EnableApiRequestLogging { get; set; }

		[DisplayName("Allow DocuSign desktop client")]
		public bool AllowDocuSignDesktopClient { get; set; }

		[DisplayName("Allow senders to set recipient email language")]
		public bool AllowSendersToSetRecipientEmailLanguage { get; set; }

		[DisplayName("Allow vaulting")]
		public bool AllowVaulting { get; set; }

		[DisplayName("Allowed to be envelope transfer recipient")]
		public bool AllowedToBeEnvelopeTransferRecipient { get; set; }

		[DisplayName("Enable transaction point integration")]
		public bool EnableTransactionPointIntegration { get; set; }

		public string AllowedAddressBookAccess { get; set; } = "personalAndShared";
		public string SigningUiVersion { get; set; } = "v2";
		public string AllowedTemplateAccess { get; set; } = "share";
		public string PowerFormRole { get; set; } = "admin";
		public string VaultingMode { get; set; } = "none";

	}
}
