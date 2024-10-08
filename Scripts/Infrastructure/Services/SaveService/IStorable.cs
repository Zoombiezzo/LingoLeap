namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    public interface IStorable
    {
        void Load(IStorage data);
        string ToStorage();
    }
}