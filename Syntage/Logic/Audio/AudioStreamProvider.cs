namespace Syntage.Logic.Audio
{
    public interface IAudioStreamProvider
    {
        IAudioStream CreateAudioStream();
        void ReleaseAudioStream(IAudioStream stream);

        int CurrentStreamLenght { get; }
    }
}
