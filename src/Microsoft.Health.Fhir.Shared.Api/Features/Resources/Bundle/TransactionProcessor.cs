// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.Api.Features.Resources.Bundle
{
    public class TransactionProcessor
    {
        public Hl7.Fhir.Model.Bundle PreProcessBundleTransaction(Hl7.Fhir.Model.Bundle bundleResource)
        {
            BundleValidator.ValidateTransactionBundle(bundleResource);

            var responseBundle = new Hl7.Fhir.Model.Bundle
            {
                Type = Hl7.Fhir.Model.Bundle.BundleType.TransactionResponse,
            };

            return responseBundle;
        }

        public static void ThrowTransactionException(HttpContext httpContext, OperationOutcome operationOutcome)
        {
            var operationOutcomeIssues = GetOperationOutcomeIssues(operationOutcome.Issue);

            var errorMessage = string.Format(Api.Resources.TransactionFailed, httpContext.Request.Method, httpContext.Request.Path);

            throw new TransactionFailedException(errorMessage, (HttpStatusCode)httpContext.Response.StatusCode, operationOutcomeIssues);
        }

        public static List<OperationOutcomeIssue> GetOperationOutcomeIssues(List<OperationOutcome.IssueComponent> operationoutcomeIssueList)
        {
            var issues = new List<OperationOutcomeIssue>();

            operationoutcomeIssueList.ForEach(x =>
                issues.Add(new OperationOutcomeIssue(
                    x.Severity.ToString(),
                    x.Code.ToString(),
                    x.Diagnostics)));

            return issues;
        }
    }
}
