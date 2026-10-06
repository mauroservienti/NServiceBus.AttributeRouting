using NUnit.Framework;
using PublicApiGenerator;
using System.Runtime.CompilerServices;

namespace NServiceBus.AttributeRouting.Tests.API
{
    public class APIApprovals
    {
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Approve_API()
        {
            var publicApi = typeof(AttributeRoutingFeature).Assembly.GeneratePublicApi(new ApiGeneratorOptions()
            {
                ExcludeAttributes =
                [
                    "System.Runtime.Versioning.TargetFrameworkAttribute"
                ]
            });

            Approver.Verify(publicApi.Replace(".git", ""));
        }
    }
}