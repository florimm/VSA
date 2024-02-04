using System.Globalization;

namespace DocumentManagementStore.Common;

public class SwashbuckleSchemaHelper
{
    private readonly Dictionary<string, int> _schemaNameRepetition;

    public SwashbuckleSchemaHelper() => _schemaNameRepetition = new Dictionary<string, int>();

    private string DefaultSchemaIdSelector(Type modelType)
    {
        if (!modelType.IsConstructedGenericType)
        {
            return modelType.Name.Replace("[]", "Array");
        }

        var prefix = modelType
            .GetGenericArguments()
            .Select(DefaultSchemaIdSelector)
            .Aggregate((previous, current) => previous + current);

        return prefix + modelType.Name.Split('`').First();
    }

    public string GetSchemaId(Type modelType)
    {
        var id = DefaultSchemaIdSelector(modelType);

        if (!_schemaNameRepetition.ContainsKey(id))
        {
            _schemaNameRepetition.Add(id, 0);
        }

        var count = _schemaNameRepetition[id] + 1;
        _schemaNameRepetition[id] = count;

        return $"{id}{(count > 1 ? count.ToString(CultureInfo.InvariantCulture) : "")}";
    }
}