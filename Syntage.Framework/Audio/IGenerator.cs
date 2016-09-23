namespace Syntage.Framework.Audio
{
    // Генератор не создает каждый раз новый буфер, он использует старый
    public interface IGenerator
    {
        IAudioStream Generate();
    }
}
