using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Plugin;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;
using Syntage.Framework.UI;

namespace Syntage.Framework
{
	public abstract class SyntagePluginCommandStub<T> : StdPluginCommandStub where T : SyntagePlugin
	{
	    public new T Plugin
	    {
	        get { return (T)base.Plugin; }
	    }

        public override VstParameterProperties GetParameterProperties(int index)
	    {
		    LogMethod(MethodBase.GetCurrentMethod());

            // NOTE comment from wdl-ol
            //could implement effGetParameterProperties to group parameters, but can't find a host that supports it
            return null;
          
			//return base.GetParameterProperties(index);
		}

        protected override VstPluginInfo CreatePluginInfo(IVstPlugin plugin)
        {
            var info = base.CreatePluginInfo(plugin);

            info.ParameterCount = Plugin.ParametersManager.Parameters.Count();
            info.ProgramCount = Plugin.ParametersManager.Programs.Count();

            return info;
        }

        protected void LogMethod(MethodBase method, params object[] p)
        {
            //var name = method.Name;
            //Tools.Log.Instance.WriteToFile = true;
            //Tools.Log.Print(name + " " + string.Join(" ", p));
        }

        //public override void ProcessReplacing(VstAudioBuffer[] inputs, VstAudioBuffer[] outputs)
        //{
        //    LogMethod(MethodBase.GetCurrentMethod());
		//
        //    base.ProcessReplacing(inputs, outputs);
        //}
		//
        //public override void ProcessReplacing(VstAudioPrecisionBuffer[] inputs, VstAudioPrecisionBuffer[] outputs)
        //{
		//	LogMethod(MethodBase.GetCurrentMethod());
		//
        //    base.ProcessReplacing(inputs, outputs);
        //}

        public override void SetParameter(int index, float value)
        {
            LogMethod(MethodBase.GetCurrentMethod(), index, value);

            Plugin.ParametersManager.SetParameter(index, value);

            //base.SetParameter(index, value);
        }

        public override float GetParameter(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod(), index);

            return (float)Plugin.ParametersManager.GetParameter(index).RealValue;

            //return base.GetParameter(index);
        }

        public override void Open()
        {
            LogMethod(MethodBase.GetCurrentMethod());
            
            base.Open();
        }

        public override void Close()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            base.Close();
        }

        public override void SetProgram(int programNumber)
        {
            LogMethod(MethodBase.GetCurrentMethod(), programNumber);

            Plugin.ParametersManager.SetProgram(programNumber);

            //base.SetProgram(programNumber);
        }

        public override int GetProgram()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.GetProgram();

            //return base.GetProgram();
        }

        public override void SetProgramName(string name)
        {
            LogMethod(MethodBase.GetCurrentMethod(), name);

            Plugin.ParametersManager.SetProgramName(name);

            //base.SetProgramName(name);
        }

        public override string GetProgramName()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.GetProgramName();

            //return base.GetProgramName();
        }

        public override string GetParameterLabel(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.GetParameterLabel(index);

            //return base.GetParameterLabel(index);
        }

        public override string GetParameterDisplay(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.GetParameterDisplay(index);

            //return base.GetParameterDisplay(index);
        }

        public override string GetParameterName(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.GetParameterName(index);

            //return base.GetParameterName(index);
        }

        public override void SetSampleRate(float sampleRate)
        {
            LogMethod(MethodBase.GetCurrentMethod(), sampleRate);

            base.SetSampleRate(sampleRate);
        }

        public override void SetBlockSize(int blockSize)
        {
            LogMethod(MethodBase.GetCurrentMethod(), blockSize);

            base.SetBlockSize(blockSize);
        }

        public override void MainsChanged(bool onoff)
        {
            LogMethod(MethodBase.GetCurrentMethod(), onoff);

            base.MainsChanged(onoff);
        }

        public override bool EditorGetRect(out Rectangle rect)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.EditorGetRect(out rect);
        }

        public override bool EditorOpen(IntPtr hWnd)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.EditorOpen(hWnd);
        }

        public override void EditorClose()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            base.EditorClose();
        }

        public override void EditorIdle()
        {
            //LogMethod(MethodBase.GetCurrentMethod());

            UIThread.Instance.Update();

            base.EditorIdle();
        }

        public override byte[] GetChunk(bool isPreset)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetChunk(isPreset);
        }

        public override int SetChunk(byte[] data, bool isPreset)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.SetChunk(data, isPreset);
        }

        //public override bool ProcessEvents(VstEvent[] events)
        //{
        //    LogMethod(MethodBase.GetCurrentMethod());
        //
        //    return base.ProcessEvents(events);
        //}

        public override bool CanParameterBeAutomated(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.CanParameterBeAutomated(index);

            //return base.CanParameterBeAutomated(index);
        }

        public override bool String2Parameter(int index, string str)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.String2Parameter(index, str);

            //return base.String2Parameter(index, str);
        }

        public override string GetProgramNameIndexed(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod(), index);

            return Plugin.ParametersManager.GetProgramNameIndexed(index);

            //return base.GetProgramNameIndexed(index);
        }

        public override VstPinProperties GetInputProperties(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetInputProperties(index);
        }

        public override VstPinProperties GetOutputProperties(int index)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetOutputProperties(index);
        }

        public override VstPluginCategory GetCategory()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetCategory();
        }

        public override bool SetSpeakerArrangement(VstSpeakerArrangement saInput, VstSpeakerArrangement saOutput)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.SetSpeakerArrangement(saInput, saOutput);
        }

        public override bool SetBypass(bool bypass)
        {
            LogMethod(MethodBase.GetCurrentMethod(), bypass);

            return base.SetBypass(bypass);
        }

        public override string GetEffectName()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetEffectName();
        }

        public override string GetVendorString()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetVendorString();
        }

        public override string GetProductString()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetProductString();
        }

        public override int GetVendorVersion()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetVendorVersion();
        }

        public override VstCanDoResult CanDo(string cando)
        {
            LogMethod(MethodBase.GetCurrentMethod(), cando);

            return base.CanDo(cando);
        }

        public override int GetTailSize()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetTailSize();
        }

        public override int GetVstVersion()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetVstVersion();
        }

        public override bool EditorKeyDown(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.EditorKeyDown(ascii, virtualKey, modifers);
        }

        public override bool EditorKeyUp(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.EditorKeyUp(ascii, virtualKey, modifers);
        }

        public override bool SetEditorKnobMode(VstKnobMode mode)
        {
            LogMethod(MethodBase.GetCurrentMethod(), mode);

            return base.SetEditorKnobMode(mode);
        }

        public override int GetMidiProgramName(VstMidiProgramName midiProgram, int channel)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetMidiProgramName(midiProgram, channel);
        }

        public override int GetCurrentMidiProgramName(VstMidiProgramName midiProgram, int channel)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetCurrentMidiProgramName(midiProgram, channel);
        }

        public override int GetMidiProgramCategory(VstMidiProgramCategory midiCat, int channel)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetMidiProgramCategory(midiCat, channel);
        }

        public override bool HasMidiProgramsChanged(int channel)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.HasMidiProgramsChanged(channel);
        }

        public override bool GetMidiKeyName(VstMidiKeyName midiKeyName, int channel)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetMidiKeyName(midiKeyName, channel);
        }

        public override bool BeginSetProgram()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.BeginSetProgram();

            //return base.BeginSetProgram();
        }

        public override bool EndSetProgram()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return Plugin.ParametersManager.EndSetProgram();

            //return base.EndSetProgram();
        }

        public override bool GetSpeakerArrangement(out VstSpeakerArrangement input, out VstSpeakerArrangement output)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetSpeakerArrangement(out input, out output);
        }

        public override int GetNextPlugin(out string name)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetNextPlugin(out name);
        }

        public override int StartProcess()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.StartProcess();
        }

        public override int StopProcess()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.StopProcess();
        }

        public override bool SetPanLaw(VstPanLaw type, float gain)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.SetPanLaw(type, gain);
        }

        public override VstCanDoResult BeginLoadBank(VstPatchChunkInfo chunkInfo)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.BeginLoadBank(chunkInfo);
        }

        public override VstCanDoResult BeginLoadProgram(VstPatchChunkInfo chunkInfo)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.BeginLoadProgram(chunkInfo);
        }

        public override bool SetProcessPrecision(VstProcessPrecision precision)
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.SetProcessPrecision(precision);
        }

        public override int GetNumberOfMidiInputChannels()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetNumberOfMidiInputChannels();
        }

        public override int GetNumberOfMidiOutputChannels()
        {
            LogMethod(MethodBase.GetCurrentMethod());

            return base.GetNumberOfMidiOutputChannels();
        }
    }
}
