namespace AtlasOps.ReferenceData.Infrastructure;

using System.Runtime.CompilerServices;
using System.Text;

public static class QuotedCsvReader
{
    public static async IAsyncEnumerable<IReadOnlyList<string>> ReadAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        List<string> row = [];
        StringBuilder field = new();
        bool quoted = false;
        int charactersRead = 0;
        while (reader.Read() is int rawValue && rawValue >= 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            char value = (char)rawValue;
            charactersRead++;
            if (charactersRead % 16_384 == 0)
            {
                await Task.Yield();
            }

            if (value == '"')
            {
                if (quoted && reader.Peek() == '"')
                {
                    reader.Read();
                    field.Append('"');
                    charactersRead++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (value == ',' && !quoted)
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if ((value == '\r' || value == '\n') && !quoted)
            {
                if (value == '\r' && reader.Peek() == '\n')
                {
                    reader.Read();
                    charactersRead++;
                }

                row.Add(field.ToString());
                field.Clear();
                if (row.Any(static item => item.Length > 0))
                {
                    yield return row.ToArray();
                }

                row.Clear();
            }
            else
            {
                field.Append(value);
            }
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            yield return row.ToArray();
        }
    }

    public static Dictionary<string, int> CreateHeaderMap(IReadOnlyList<string> header)
    {
        return header
            .Select(static (name, index) => new KeyValuePair<string, int>(name, index))
            .ToDictionary(static item => item.Key, static item => item.Value, StringComparer.OrdinalIgnoreCase);
    }

    public static string Get(
        IReadOnlyList<string> row,
        IReadOnlyDictionary<string, int> header,
        string name)
    {
        return header.TryGetValue(name, out int index) && index < row.Count
            ? row[index].Trim()
            : string.Empty;
    }
}