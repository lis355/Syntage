using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    // Генератор не создает каждый раз новый буфер, он использует старый
    public interface IGenerator
    {
        IAudioStream Generate();
    }
}
