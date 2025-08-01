namespace DiscordTimeTracker.Application.Extensions;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class FileNameHelper
{
    public static string NormalizeFileName(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "arquivo";

        // Remove acentos e normaliza para FormD (decomposição)
        var normalized = input.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }

        var noAccents = builder.ToString().Normalize(NormalizationForm.FormC);

        // Substitui espaços e caracteres inválidos
        var cleaned = Regex.Replace(noAccents.ToLowerInvariant(), @"[^a-z0-9]+", "_");

        // Remove underscores duplicados e bordas
        cleaned = Regex.Replace(cleaned, @"_+", "_").Trim('_');

        return cleaned;
    }
}

