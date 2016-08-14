using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public interface IProcessor
    {
        void Process(IAudioStream stream);
    }
}
