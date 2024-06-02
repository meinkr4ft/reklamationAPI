namespace ReklamationAPI.Tests
{
    public static class Util
    {
        public static T? GetValue<T>(this Object o, string key)
        {
            var type = o.GetType();
            if (type != null)
            {
                var property = type.GetProperty(key);
                if (property != null)
                {
                    var value = (T?)property.GetValue(o);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }
            return default;
        }
    }
}
