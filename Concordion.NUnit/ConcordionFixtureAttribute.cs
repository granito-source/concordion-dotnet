using System.Reflection;
using Concordion.Internal;

namespace Concordion.NUnit;

[AttributeUsage(AttributeTargets.Class)]
public class ConcordionFixtureAttribute : NUnitAttribute, IFixtureBuilder {
    public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
    {
        yield return new ConcordionFixture(typeInfo);
    }

    private class ConcordionRunner(ITypeInfo typeInfo) {
        public void ConcordionTest()
        {
            var fixture = Activator.CreateInstance(typeInfo.Type);
            var result = new FixtureRunner().Run(fixture!);

            if (result.HasExceptions)
                throw new Exception(
                    "Exception in Concordion test: please see Concordion test reports");

            if (result.HasFailures)
                Assert.Fail($"""
                    Concordion Test Failures: {result.FailureCount}
                    for stack trace, please see Concordion test reports
                    """);
        }
    }

    private class ConcordionFixture : TestFixture {
        public override string TestType => nameof(TestFixture);

        public ConcordionFixture(ITypeInfo typeInfo) :
            base(new TypeWrapper(typeof(ConcordionRunner)), [typeInfo])
        {
            Name = typeInfo.Name;
            FullName = typeInfo.FullName;
            AddMethod();
        }

        private void AddMethod()
        {
            var methods = TypeInfo.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance);

            foreach (var method in methods) {
                if (method.Name == nameof(ConcordionRunner.ConcordionTest) &&
                    method.ReturnType.Type == typeof(void) &&
                    method.GetParameters().Length == 0) {
                    Add(new ConcordionTestMethod(method, this));

                    return;
                }
            }
        }
    }

    private class ConcordionTestMethod : TestMethod {
        private static readonly Randomizer Randomizer =
            Randomizer.CreateRandomizer();

        public ConcordionTestMethod(IMethodInfo method, Test parent) :
            base(method, parent)
        {
            Seed = Randomizer.Next();
        }
    }
}
