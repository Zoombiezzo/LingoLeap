namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    public interface IStorableData
    {
        IStorable Data { get; }
        IStorage Storage { get;}
        void Loaded(IStorableData data);
    }
}