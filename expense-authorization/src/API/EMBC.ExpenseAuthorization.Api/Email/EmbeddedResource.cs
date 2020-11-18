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

            if (stream == null)
            {
                // should never happen unless the specified file name is wrong
                return string.Empty;
            }

            using StreamReader reader = new StreamReader(stream);

            string text = reader.ReadToEnd();

            return text;
        }
    }
}
