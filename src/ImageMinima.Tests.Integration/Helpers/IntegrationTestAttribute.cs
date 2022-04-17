using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ImageMinima.Tests.Integration.Helpers
{
    public class IntegrationTestDiscoverer : IXunitTestCaseDiscoverer
    {
        readonly IMessageSink diagnosticMessageSink;

        public IntegrationTestDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return Enumerable.Empty<IXunitTestCase>();

            //return Helper.Credentials == null
            //    ? Enumerable.Empty<IXunitTestCase>()
            //    : new[] { new XunitTestCase(diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), TestMethodDisplayOptions.None, testMethod) };
        }
    }

    [XunitTestCaseDiscoverer("ImageMinima.Tests.Integration.IntegrationTestDiscoverer", "ImageMinima.Tests.Integration")]
    public class IntegrationTestAttribute : FactAttribute
    {
    }
}
