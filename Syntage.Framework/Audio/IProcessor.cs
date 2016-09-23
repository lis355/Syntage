namespace Syntage.Framework.Audio
{
    public interface IProcessor
    {
        void Process(IAudioStream stream);
    }
}
