namespace TGParser.Configuration.Models;

public record PostgresModel(string Host, int Port, string Login, string Password, string DatabaseName)
{
    public string ConnectionString => $"User ID={Login};Password={Password};Host={Host};Port={Port};Database={DatabaseName};Pooling=true;CommandTimeout=300";
}
