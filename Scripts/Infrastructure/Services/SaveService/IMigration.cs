namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    public interface IMigration
    {
        IStorage From { get; }
        IStorage To { get; }
        string Migrate(string storage);
    }
}