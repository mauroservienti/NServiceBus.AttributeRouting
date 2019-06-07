using ApprovalTests;
using ApprovalTests.Reporters;
using PublicApiGenerator;
using System.Runtime.CompilerServices;
using Xunit;

namespace NServiceBus.AttributeRouting.Tests.API
{
    public class APIApprovals
    {
        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Approve_API()
        {
            var publicApi = ApiGenerator.GeneratePublicApi(typeof(RouteToAttribute).Assembly);

            Approvals.Verify(publicApi);
        }
    }
}
