using Jacobi.Vst.Core.Host;

namespace SimplyHost
{
    public class HostCommandStub : IVstHostCommandStub
    {
        public IVstPluginContext PluginContext { get; set; }
        
        public void SetParameterAutomated(int index, float value)
        {
        }

        public bool BeginEdit(int index)
        {
            return true;
        }
        
        public bool EndEdit(int index)
        {
            return true;
        }

        public Jacobi.Vst.Core.VstCanDoResult CanDo(string cando)
        {
            return Jacobi.Vst.Core.VstCanDoResult.Unknown;
        }
        
        public bool CloseFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
        {
            return false;
        }
        
        public Jacobi.Vst.Core.VstAutomationStates GetAutomationState()
        {
            return Jacobi.Vst.Core.VstAutomationStates.Off;
        }
        
        public int GetBlockSize()
        {
            return 1024;
        }
        
        public string GetDirectory()
        {
            return null;
        }
        
        public int GetInputLatency()
        {
            return 0;
        }
        
        public Jacobi.Vst.Core.VstHostLanguage GetLanguage()
        {
            return Jacobi.Vst.Core.VstHostLanguage.NotSupported;
        }
        
        public int GetOutputLatency()
        {
            return 0;
        }
        
        public Jacobi.Vst.Core.VstProcessLevels GetProcessLevel()
        {
            return Jacobi.Vst.Core.VstProcessLevels.Unknown;
        }
        
        public string GetProductString()
        {
            return "SimplyHost";
        }
        
        public float GetSampleRate()
        {
            return 44.1f;
        }
        
        public Jacobi.Vst.Core.VstTimeInfo GetTimeInfo(Jacobi.Vst.Core.VstTimeInfoFlags filterFlags)
        {
            return null;
        }
        
        public string GetVendorString()
        {
            return "VST .NET TESTS";
        }
        
        public int GetVendorVersion()
        {
            return 1000;
        }
        
        public bool IoChanged()
        {
            return false;
        }
        
        public bool OpenFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
        {
            return false;
        }
        
        public bool ProcessEvents(Jacobi.Vst.Core.VstEvent[] events)
        {
            return false;
        }
        
        public bool SizeWindow(int width, int height)
        {
            return false;
        }
        
        public bool UpdateDisplay()
        {
            return false;
        }
        
        public int GetCurrentPluginID()
        {
            return PluginContext.PluginInfo.PluginID;
        }
        
        public int GetVersion()
        {
            return 1000;
        }
        
        public void ProcessIdle()
        {
        }
    }
}
