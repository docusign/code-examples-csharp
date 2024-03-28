// <copyright file="CreateWorkflowService.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.WebForms.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Maestro.Api;
    using DocuSign.Maestro.Client;
    using DocuSign.Maestro.Model;
    using Newtonsoft.Json.Linq;

    public static class CreateWorkflowService
    {
        private const string Versions = "1.0.0";

        public static NewOrUpdatedWorkflowDefinitionResponse CreateWorkflowDefinition(
            DocuSignClient docuSignClient,
            string accountId,
            string templateId)
        {
            var maestroApi = new WorkflowManagementApi(docuSignClient);
            var signerId = Guid.NewGuid();
            var ccId = Guid.NewGuid();
            var triggerId = "wfTrigger";

            var createModel = new CreateOrUpdateWorkflowDefinitionRequestBody()
            {
                WorkflowDefinition = new WorkflowDefinition()
                {
                    WorkflowName = "Example workflow - send invite to signer",
                    WorkflowDescription = string.Empty,
                    AccountId = accountId,
                    DocumentVersion = Versions,
                    SchemaVersion = Versions,
                    Participants = PrepareParticipants(signerId, ccId),
                    Trigger = PrepareTrigger(triggerId),
                    Variables = PrepareVariables(triggerId),
                    Steps = PrepareSteps(templateId, signerId, triggerId),
                },
            };

            return maestroApi.CreateWorkflowDefinition(accountId, createModel);
        }

        private static List<object> PrepareSteps(string templateId, Guid signerId, string triggerId)
        {
            return new List<object>
            {
                StepOneSetUpInvite(signerId, triggerId),
                StepTwoGetSignatures(templateId, signerId, triggerId),
                StepThreeConfirm(signerId),
            };
        }

        private static JObject StepThreeConfirm(Guid signerId)
        {
            return new JObject
            {
                { "id", "step3" },
                { "name", "Show a Confirmation Screen" },
                { "moduleName",  "ShowConfirmationScreen" },
                { "configurationProgress",  "Completed" },
                { "type",  "DS-ShowScreenStep" },
                {
                    "config",
                    new JObject
                    {
                        { "participantId",  signerId.ToString() },
                    }
                },
                {
                    "input",
                    new JObject
                    {
                        { "httpType", "Post" },
                        {
                            "payload",
                            new JObject
                            {
                                { "participantId",  signerId.ToString() },
                                {
                                    "confirmationMessage",
                                    new JObject
                                    {
                                        { "title", "Tasks complete" },
                                        { "description", "You have completed all your workflow tasks." },
                                    }
                                },
                            }
                        },
                    }
                },
                { "output", new JObject() },
            };
        }

        private static JObject StepTwoGetSignatures(string templateId, Guid signerId, string triggerId)
        {
            return new JObject
            {
                { "id", "step2" },
                { "name", "Get Signatures" },
                { "moduleName",  "ESign" },
                { "configurationProgress",  "Completed" },
                { "type",  "DS-Sign" },
                {
                    "config",
                    new JObject
                    {
                        { "participantId",  signerId.ToString() },
                    }
                },
                {
                    "input",
                    new JObject
                    {
                        { "isEmbeddedSign", true },
                        {
                            "documents",
                            new JArray
                            {
                                new JObject
                                {
                                    { "type", "FromDSTemplate" },
                                    { "eSignTemplateId", templateId },
                                },
                            }
                        },
                        { "emailSubject", "Please sign this document" },
                        { "emailBlurb", string.Empty },
                        {
                            "recipients",
                            new JObject
                            {
                                {
                                    "signers",
                                    new JArray
                                    {
                                        new JObject
                                        {
                                            { "defaultRecipient", "false" },
                                            {
                                                "tabs",
                                                new JObject
                                                {
                                                    {
                                                        "signHereTabs",
                                                        new JArray
                                                        {
                                                            new JObject
                                                            {
                                                                { "stampType", "signature" },
                                                                { "name", "SignHere" },
                                                                { "tabLabel", "Sign Here" },
                                                                { "scaleValue", "1" },
                                                                { "optional", "false" },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "191" },
                                                                { "yPosition", "148" },
                                                                { "tabId", "1" },
                                                                { "tabType", "signhere" },
                                                            },
                                                        }
                                                    },
                                                    {
                                                        "textTabs",
                                                        new JArray
                                                        {
                                                            new JObject
                                                            {
                                                                { "requireAll", "false" },
                                                                { "value", string.Empty },
                                                                { "required", "false" },
                                                                { "locked", "false" },
                                                                { "concealValueOnDocument", "false" },
                                                                { "disableAutoSize", "false" },
                                                                { "tabLabel", "text" },
                                                                { "font", "helvetica" },
                                                                { "fontSize", "size14" },
                                                                { "localePolicy", new JObject() },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "153" },
                                                                { "yPosition", "230" },
                                                                { "width", "84" },
                                                                { "height", "23" },
                                                                { "tabId", "2" },
                                                                { "tabType", "text" },
                                                            },
                                                        }
                                                    },
                                                    {
                                                        "checkboxTabs",
                                                        new JArray
                                                        {
                                                            new JObject
                                                            {
                                                                { "name", string.Empty },
                                                                { "tabLabel", "ckAuthorization" },
                                                                { "selected", "false" },
                                                                { "selectedOriginal", "false" },
                                                                { "requireInitialOnSharedChange", "false" },
                                                                { "required", "true" },
                                                                { "locked", "false" },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "75" },
                                                                { "yPosition", "417" },
                                                                { "width", "0" },
                                                                { "height", "0" },
                                                                { "tabId", "3" },
                                                                { "tabType", "checkbox" },
                                                            },
                                                            new JObject
                                                            {
                                                                { "name", string.Empty },
                                                                { "tabLabel", "ckAuthentication" },
                                                                { "selected", "false" },
                                                                { "selectedOriginal", "false" },
                                                                { "requireInitialOnSharedChange", "false" },
                                                                { "required", "true" },
                                                                { "locked", "false" },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "75" },
                                                                { "yPosition", "447" },
                                                                { "width", "0" },
                                                                { "height", "0" },
                                                                { "tabId", "4" },
                                                                { "tabType", "checkbox" },
                                                            },
                                                            new JObject
                                                            {
                                                                { "name", string.Empty },
                                                                { "tabLabel", "ckAgreement" },
                                                                { "selected", "false" },
                                                                { "selectedOriginal", "false" },
                                                                { "requireInitialOnSharedChange", "false" },
                                                                { "required", "true" },
                                                                { "locked", "false" },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "75" },
                                                                { "yPosition", "478" },
                                                                { "width", "0" },
                                                                { "height", "0" },
                                                                { "tabId", "5" },
                                                                { "tabType", "checkbox" },
                                                            },
                                                            new JObject
                                                            {
                                                                { "name", string.Empty },
                                                                { "tabLabel", "ckAcknowledgement" },
                                                                { "selected", "false" },
                                                                { "selectedOriginal", "false" },
                                                                { "requireInitialOnSharedChange", "false" },
                                                                { "required", "true" },
                                                                { "locked", "false" },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "75" },
                                                                { "yPosition", "508" },
                                                                { "width", "0" },
                                                                { "height", "0" },
                                                                { "tabId", "6" },
                                                                { "tabType", "checkbox" },
                                                            },
                                                        }
                                                    },
                                                    {
                                                        "radioGroupTabs",
                                                        new JArray
                                                        {
                                                            new JObject
                                                            {
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "groupName", "radio1" },
                                                                {
                                                                    "radios",
                                                                    new JArray
                                                                    {
                                                                        new JObject
                                                                        {
                                                                            { "pageNumber", "1" },
                                                                            { "xPosition", "142" },
                                                                            { "yPosition", "384" },
                                                                            { "value", "white" },
                                                                            { "selected", "false" },
                                                                            { "tabId", "7" },
                                                                            { "required", "false" },
                                                                            { "locked", "false" },
                                                                            { "bold", "false" },
                                                                            { "italic", "false" },
                                                                            { "underline", "false" },
                                                                            { "fontColor", "black" },
                                                                            { "fontSize", "size7" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "pageNumber", "1" },
                                                                            { "xPosition", "74" },
                                                                            { "yPosition", "384" },
                                                                            { "value", "red" },
                                                                            { "selected", "false" },
                                                                            { "tabId", "8" },
                                                                            { "required", "false" },
                                                                            { "locked", "false" },
                                                                            { "bold", "false" },
                                                                            { "italic", "false" },
                                                                            { "underline", "false" },
                                                                            { "fontColor", "black" },
                                                                            { "fontSize", "size7" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "pageNumber", "1" },
                                                                            { "xPosition", "220" },
                                                                            { "yPosition", "384" },
                                                                            { "value", "blue" },
                                                                            { "selected", "false" },
                                                                            { "tabId", "9" },
                                                                            { "required", "false" },
                                                                            { "locked", "false" },
                                                                            { "bold", "false" },
                                                                            { "italic", "false" },
                                                                            { "underline", "false" },
                                                                            { "fontColor", "black" },
                                                                            { "fontSize", "size7" },
                                                                        },
                                                                    }
                                                                },
                                                                { "shared", "false" },
                                                                { "requireInitialOnSharedChange", "false" },
                                                                { "requireAll", "false" },
                                                                { "tabType", "radiogroup" },
                                                                { "value", string.Empty },
                                                                { "originalValue", string.Empty },
                                                            },
                                                        }
                                                    },
                                                    {
                                                        "listTabs",
                                                        new JArray
                                                        {
                                                            new JObject
                                                            {
                                                                {
                                                                    "listItems",
                                                                    new JArray
                                                                    {
                                                                        new JObject
                                                                        {
                                                                            { "text", "Red" },
                                                                            { "value", "red" },
                                                                            { "selected", "false" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "text", "Orange" },
                                                                            { "value", "orange" },
                                                                            { "selected", "false" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "text", "Yellow" },
                                                                            { "value", "yellow" },
                                                                            { "selected", "false" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "text", "Green" },
                                                                            { "value", "green" },
                                                                            { "selected", "false" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "text", "Blue" },
                                                                            { "value", "blue" },
                                                                            { "selected", "false" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "text", "Indigo" },
                                                                            { "value", "indigo" },
                                                                            { "selected", "false" },
                                                                        },
                                                                        new JObject
                                                                        {
                                                                            { "text", "Violet" },
                                                                            { "value", "violet" },
                                                                            { "selected", "false" },
                                                                        },
                                                                    }
                                                                },
                                                                { "value", string.Empty },
                                                                { "originalValue", string.Empty },
                                                                { "required", "false" },
                                                                { "locked", "false" },
                                                                { "requireAll", "false" },
                                                                { "tabLabel", "list" },
                                                                { "font", "helvetica" },
                                                                { "fontSize", "size14" },
                                                                { "localePolicy", new JObject() },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "142" },
                                                                { "yPosition", "291" },
                                                                { "width", "78" },
                                                                { "height", "0" },
                                                                { "tabId", "10" },
                                                                { "tabType", "list" },
                                                            },
                                                        }
                                                    },
                                                    {
                                                        "numericalTabs",
                                                        new JArray
                                                        {
                                                            new JObject
                                                            {
                                                                { "validationType", "currency" },
                                                                { "value", string.Empty },
                                                                { "required", "false" },
                                                                { "locked", "false" },
                                                                { "concealValueOnDocument", "false" },
                                                                { "disableAutoSize", "false" },
                                                                { "tabLabel", "numericalCurrency" },
                                                                { "font", "helvetica" },
                                                                { "fontSize", "size14" },
                                                                {
                                                                    "localePolicy",
                                                                    new JObject
                                                                    {
                                                                        { "cultureName", "en-US" },
                                                                        { "currencyPositiveFormat", "csym_1_comma_234_comma_567_period_89" },
                                                                        { "currencyNegativeFormat", "opar_csym_1_comma_234_comma_567_period_89_cpar" },
                                                                        { "currencyCode", "usd" },
                                                                    }
                                                                },
                                                                { "documentId", "1" },
                                                                { "recipientId", "1" },
                                                                { "pageNumber", "1" },
                                                                { "xPosition", "163" },
                                                                { "yPosition", "260" },
                                                                { "width", "84" },
                                                                { "height", "0" },
                                                                { "tabId", "11" },
                                                                { "tabType", "numerical" },
                                                            },
                                                        }
                                                    },
                                                }
                                            },
                                            { "signInEachLocation", "false" },
                                            { "agentCanEditEmail", "false" },
                                            { "agentCanEditName", "false" },
                                            { "requireUploadSignature", "false" },
                                            {
                                                "name",
                                                new JObject
                                                {
                                                    { "source", "step" },
                                                    { "propertyName", "signerName" },
                                                    { "stepId", triggerId },
                                                }
                                            },
                                            {
                                                "email",
                                                new JObject
                                                {
                                                    { "source", "step" },
                                                    { "propertyName", "signerEmail" },
                                                    { "stepId", triggerId },
                                                }
                                            },
                                            { "recipientId", "1" },
                                            { "recipientIdGuid", "00000000-0000-0000-0000-000000000000" },
                                            { "accessCode", string.Empty },
                                            { "requireIdLookup", "false" },
                                            { "routingOrder", "1" },
                                            { "note", string.Empty },
                                            { "roleName", "signer" },
                                            { "completedCount", "0" },
                                            { "deliveryMethod", "email" },
                                            { "templateLocked", "false" },
                                            { "templateRequired", "false" },
                                            { "inheritEmailNotificationConfiguration", "false" },
                                            { "recipientType", "signer" },
                                        },
                                    }
                                },
                                {
                                    "carbonCopies",
                                    new JArray
                                    {
                                        new JObject
                                        {
                                            { "agentCanEditEmail", "false" },
                                            { "agentCanEditName", "false" },
                                            {
                                                "name",
                                                new JObject
                                                {
                                                    { "source", "step" },
                                                    { "propertyName", "ccName" },
                                                    { "stepId", triggerId },
                                                }
                                            },
                                            {
                                                "email",
                                                new JObject
                                                {
                                                    { "source", "step" },
                                                    { "propertyName", "ccEmail" },
                                                    { "stepId", triggerId },
                                                }
                                            },
                                            { "recipientId", "2" },
                                            { "recipientIdGuid", "00000000-0000-0000-0000-000000000000" },
                                            { "accessCode", string.Empty },
                                            { "requireIdLookup", "false" },
                                            { "routingOrder", "2" },
                                            { "note", string.Empty },
                                            { "roleName", "cc" },
                                            { "completedCount", "0" },
                                            { "deliveryMethod", "email" },
                                            { "templateLocked", "false" },
                                            { "templateRequired", "false" },
                                            { "inheritEmailNotificationConfiguration", "false" },
                                            { "recipientType", "carboncopy" },
                                        },
                                    }
                                },
                            }
                        },
                    }
                },
                {
                    "output",
                    new JObject
                    {
                        {
                            "envelopeId_step2",
                            new JObject
                            {
                                { "source", "step" },
                                { "propertyName", "envelopeId" },
                                { "stepId", "step2" },
                                { "type", "String" },
                            }
                        },
                        {
                            "combinedDocumentsBase64_step2",
                            new JObject
                            {
                                { "source", "step" },
                                { "propertyName", "combinedDocumentsBase64" },
                                { "stepId", "step2" },
                                { "type", "File" },
                            }
                        },
                        {
                            "fields.signer.text.value_step2",
                            new JObject
                            {
                                { "source", "step" },
                                { "propertyName", "fields.signer.text.value" },
                                { "stepId", "step2" },
                                { "type", "String" },
                            }
                        },
                    }
                },
            };
        }

        private static JObject StepOneSetUpInvite(Guid signerId, string triggerId)
        {
            return new JObject
            {
                { "id", "step1" },
                { "name", "Set Up Invite" },
                { "moduleName",  "Notification-SendEmail" },
                { "configurationProgress",  "Completed" },
                { "type",  "DS-EmailNotification" },
                {
                    "config",
                    new JObject
                    {
                        { "templateType",  "WorkflowParticipantNotification" },
                        { "templateVersion",  1 },
                        { "language",  "en" },
                        { "sender_name",  "DocuSign Orchestration" },
                        { "sender_alias",  "Orchestration" },
                        { "participantId",  signerId.ToString() },
                    }
                },
                {
                    "input",
                    new JObject
                    {
                        {
                            "recipients",
                            new JArray
                            {
                                new JObject
                                {
                                    {
                                        "name",
                                        new JObject
                                        {
                                            { "source", "step" },
                                            { "propertyName", "signerName" },
                                            { "stepId", triggerId },
                                        }
                                    },
                                    {
                                        "email",
                                        new JObject
                                        {
                                            { "source", "step" },
                                            { "propertyName", "signerEmail" },
                                            { "stepId", triggerId },
                                        }
                                    },
                                },
                            }
                        },
                        {
                            "mergeValues",
                            new JObject
                            {
                                { "CustomMessage", "Follow this link to access and complete the workflow." },
                                {
                                    "ParticipantFullName",
                                    new JObject
                                    {
                                        { "source", "step" },
                                        { "propertyName", "signerName" },
                                        { "stepId", triggerId },
                                    }
                                },
                            }
                        },
                    }
                },
                {
                    "output",
                    new JObject()
                },
            };
        }

        private static Dictionary<string, object> PrepareVariables(string triggerId)
        {
            return new Dictionary<string, object>
            {
                {
                    "dacId_" + triggerId,
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "dacId" },
                        { "stepId", triggerId },
                    }
                },
                {
                    "id_" + triggerId,
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "id" },
                        { "stepId", triggerId },
                    }
                },
                {
                    "signerName_" + triggerId,
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "signerName" },
                        { "stepId", triggerId },
                    }
                },
                {
                    "signerEmail_" + triggerId,
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "signerEmail" },
                        { "stepId", triggerId },
                    }
                },
                {
                    "ccName_" + triggerId,
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "ccName" },
                        { "stepId", triggerId },
                    }
                },
                {
                    "ccEmail_" + triggerId,
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "ccEmail" },
                        { "stepId", triggerId },
                    }
                },
                {
                    "envelopeId_step2",
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "envelopeId" },
                        { "stepId", "step2" },
                    }
                },
                {
                    "combinedDocumentsBase64_step2",
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "combinedDocumentsBase64" },
                        { "stepId", "step2" },
                    }
                },
                {
                    "fields.signer.text.value_step2",
                    new JObject
                    {
                        { "source", "step" },
                        { "propertyName", "fields.signer.text.value" },
                        { "stepId", "step2" },
                    }
                },
            };
        }

        private static DSWorkflowTrigger PrepareTrigger(string triggerId)
        {
            return new DSWorkflowTrigger()
            {
                Name = "Get_URL",
                Type = DSWorkflowTriggerTypes.Http,
                HttpType = HttpTypes.Get,
                Id = triggerId,
                Input = new Dictionary<string, object>
                {
                    {
                        "metadata",
                        new JObject
                        {
                            { "customAttributes", new JObject { } },
                        }
                    },
                    {
                        "payload",
                        new JObject
                        {
                            {
                                "dacId_" + triggerId,
                                new JObject
                                {
                                    { "source", "step" },
                                    { "propertyName", "dacId" },
                                    { "stepId", triggerId },
                                }
                            },
                            {
                                "id_" + triggerId,
                                new JObject
                                {
                                    { "source", "step" },
                                    { "propertyName", "id" },
                                    { "stepId", triggerId },
                                }
                            },
                            {
                                "signerName_" + triggerId,
                                new JObject
                                {
                                    { "source", "step" },
                                    { "propertyName", "signerName" },
                                    { "stepId", triggerId },
                                }
                            },
                            {
                                "signerEmail_" + triggerId,
                                new JObject
                                {
                                    { "source", "step" },
                                    { "propertyName", "signerEmail" },
                                    { "stepId", triggerId },
                                }
                            },
                            {
                                "ccName_" + triggerId,
                                new JObject
                                {
                                    { "source", "step" },
                                    { "propertyName", "ccName" },
                                    { "stepId", triggerId },
                                }
                            },
                            {
                                "ccEmail_" + triggerId,
                                new JObject
                                {
                                    { "source", "step" },
                                    { "propertyName", "ccEmail" },
                                    { "stepId", triggerId },
                                }
                            },
                        }
                    },
                    {
                        "participants",
                        new JObject()
                    },
                },
                Output = new Dictionary<string, object>
                {
                    {
                        "dacId_" + triggerId,
                        new JObject
                        {
                            { "source", "step" },
                            { "propertyName", "dacId" },
                            { "stepId", triggerId },
                        }
                    },
                },
            };
        }

        private static Dictionary<string, Participant> PrepareParticipants(Guid signerId, Guid ccId)
        {
            return new Dictionary<string, Participant>()
            {
                {
                    signerId.ToString(),
                    new Participant()
                    {
                        ParticipantRole = "Signer",
                    }
                },
                {
                    ccId.ToString(),
                    new Participant()
                    {
                        ParticipantRole = "CC",
                    }
                },
            };
        }
    }
}
