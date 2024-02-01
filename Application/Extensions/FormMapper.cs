namespace Affiliate.Application.Extensions;

public static class FormMapper
{
    public static void Map<TSrc, TDest>(TSrc source, TDest destination)
    {
        var sourceProperties = typeof(TSrc).GetProperties();
        var destinationProperties = typeof(TDest).GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var destinationProperty = destinationProperties.FirstOrDefault(p =>
                p.Name == sourceProperty.Name && p.PropertyType == sourceProperty.PropertyType);

            if (destinationProperty != null)
            {
                var sourceValue = sourceProperty.GetValue(source);
                var destinationValue = destinationProperty.GetValue(destination);

                if (destinationValue == null)
                {
                    destinationProperty.SetValue(destination, sourceValue);
                }
                else if (destinationValue is string value && string.IsNullOrEmpty(value))
                {
                    destinationProperty.SetValue(destination, sourceValue);
                }
            }
        }
    }
}