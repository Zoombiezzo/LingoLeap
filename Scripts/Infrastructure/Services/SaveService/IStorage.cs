namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    public interface IStorage
    {
        int Version { get; }
        IStorage ToStorage(string data);
        string ToData(IStorable data);
    }
}