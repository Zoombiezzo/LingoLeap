namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    public interface IQueryBuilder
    {
        string Query { get; }
        string Build();
    }
}