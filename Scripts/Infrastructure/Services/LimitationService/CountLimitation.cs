namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    public class CountLimitation : ILimitation
    {
        private int _count;

        public int Count => _count;
        
        public CountLimitation()
        {
            
        }

        public CountLimitation(int count)
        {
            _count = count;
        }
        
        public LimitationRecord CreateRecord()
        {
            return new CountLimitationRecord(_count);
        }
    }
}