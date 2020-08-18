using System.IO;

namespace EMBC.ExpenseAuthorization.Api.Email
{
    public static class EmbeddedResource
    {
        public static string Get<T>(string filename)
        {
            var assembly = typeof(EmbeddedResource).Assembly;

            var typeNamespace = typeof(T).Namespace;

            using var stream = assembly.GetManifestResourceStream(typeNamespace + "." + filename);

            using StreamReader reader = new StreamReader(stream);

            string text = reader.ReadToEnd();

            return text;
        }
    }
}
