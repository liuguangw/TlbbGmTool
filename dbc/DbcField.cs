namespace liuguang.Dbc;
public class DbcField
{
    public readonly DbcFieldType FieldType;
    public readonly int IntValue = 0;
    public readonly float FloatValue = 0;
    public readonly string StringValue = string.Empty;

    public DbcField(int value)
    {
        FieldType = DbcFieldType.T_INT;
        IntValue = value;
    }

    public DbcField(float value)
    {
        FieldType = DbcFieldType.T_FLOAT;
        FloatValue = value;
    }

    public DbcField(string value)
    {
        FieldType = DbcFieldType.T_STRING;
        StringValue = value;
    }
}
