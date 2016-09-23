namespace Syntage.Framework.Audio
{
    public interface IAudioStreamProvider
    {
        IAudioStream CreateAudioStream();
        void ReleaseAudioStream(IAudioStream stream);

        int CurrentStreamLenght { get; }
    }
}
