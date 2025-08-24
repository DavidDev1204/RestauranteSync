namespace RestauranteSync.Domain.Entities;

public class DatabaseInfo
{
    public string TableName { get; set; } = string.Empty;
    public List<ColumnInfo> Columns { get; set; } = new();
    public List<List<string>> Data { get; set; } = new();
}

public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string SqliteType => DataType switch
    {
        "integer" => "INTEGER",
        "bigint" => "INTEGER",
        "text" => "TEXT",
        "character varying" => "TEXT",
        "timestamp without time zone" => "TEXT",
        "boolean" => "BOOLEAN",
        _ => "TEXT"
    };
}
