using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace ETicaretAPI.API.Configurations.ColumnWriters;

public class UsernameColumnWriter : ColumnWriterBase
{
    public UsernameColumnWriter() : base(NpgsqlDbType.Varchar) // property configuration
    {
    }

    public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
    {
        // burda aradığımız anahtar olarak user_name verme sebebimiz => program içindeki => LogContext.PushProperty("user_name", username); diye key değerini user_name verdiğimiz için burda aynı isimle yakalıyoruz.
        var (usernama, value) = logEvent.Properties.FirstOrDefault(p => p.Key == "user_name");
        return value?.ToString() ?? null;
        //return value;
    }
}
