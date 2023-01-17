// <copyright file="SetViewBagAttribute.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Common
{
    using System;
    using DocuSign.CodeExamples.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class SetViewBagAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var egController = context.Controller as EgController;

            if (egController == null)
            {
                throw new InvalidOperationException("Controller is not of type EgController, attribute is not applicable.");
            }

            egController.ViewBag.SupportingTexts = egController.LauncherTexts.ManifestStructure.SupportingTexts;
        }
    }
}
