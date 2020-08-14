using System.IO;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api
{
    public static class EmbeddedResource
    {
        public static string Get<T>(string filename)
        {
            var assembly = typeof(EmbeddedResource).Assembly;

            var typeNamespace = typeof(T).Namespace;

            using var stream = assembly.GetManifestResourceStream(typeNamespace + "." + filename);

            Assert.NotNull(stream);

            using StreamReader reader = new StreamReader(stream);

            string text = reader.ReadToEnd();
            Assert.NotNull(text);

            return text;

        }
    }
}
