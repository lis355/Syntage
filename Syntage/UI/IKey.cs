using System;

namespace Syntage.UI
{
    public interface IKey
    {
        void Press();
        void Release();

        event Action OnPressFromUI;
        event Action OnReleaseFromUI;
    }
}
