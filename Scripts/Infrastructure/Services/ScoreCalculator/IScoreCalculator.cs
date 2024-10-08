namespace _Client.Scripts.Infrastructure.Services.ScoreCalculator
{
    public interface IScoreCalculator<out T> where T : struct
    {
        T Calculate();
    }
}