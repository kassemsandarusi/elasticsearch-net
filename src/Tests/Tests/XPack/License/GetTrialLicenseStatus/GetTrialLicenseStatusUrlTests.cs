﻿using System.Threading.Tasks;
using Elastic.Xunit.XunitPlumbing;
using Nest;
using Tests.Framework;
using static Tests.Framework.UrlTester;

namespace Tests.XPack.License.GetTrialLicenseStatus
{
	public class GetTrialLicenseStatusUrlTests : UrlTestsBase
	{
		[U] public override async Task Urls() => await GET("/_license/trial_status")
			.Fluent(c => c.GetTrialLicenseStatus())
			.Request(c => c.GetTrialLicenseStatus(new GetTrialLicenseStatusRequest()))
			.FluentAsync(c => c.GetTrialLicenseStatusAsync())
			.RequestAsync(c => c.GetTrialLicenseStatusAsync(new GetTrialLicenseStatusRequest()));
	}
}
