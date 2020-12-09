using System.Reflection;

namespace NServiceBus.AttributeRouting.Tests
{
    static class ConventionsExtensions
    {
        public static void Add(this Conventions conventions, IMessageConvention convention)
        {
            var addMethod = typeof(Conventions).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
            addMethod.Invoke(conventions, new object[] {convention});
        }
    }
}