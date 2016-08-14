using System;
using System.Collections.Generic;
using System.Linq;
using Jacobi.Vst.Core;

namespace Syntage.Framework.Parameters
{
    public class ParametersManager
    {
        private List<Parameter> _parameters;
        private List<Program> _programs;
        private int _activeProgram;

        public IVstHostCommands20 HostAutomation { get; private set; }

        public Program ActiveProgram
        {
            get { return _programs[_activeProgram]; }
        }

        public IEnumerable<Parameter> Parameters
        {
            get { return _parameters; }
        }

        public IEnumerable<Program> Programs
        {
            get { return _programs; }
        }

        public void SetParameters(IEnumerable<Parameter> parameters)
        {
            _parameters = new List<Parameter>(parameters);
            for (int i = 0; i < _parameters.Count; ++i)
            {
                var parameter = _parameters[i];
                parameter.Index = i;
                parameter.Manger = this;
            }
        }

        public void SetPrograms(IEnumerable<Program> programs)
        {
            _programs = new List<Program>(programs);
            SetProgram(0);
        }

        public void SetHost(IVstHostCommands20 hostAutomation)
        {
            HostAutomation = hostAutomation;
        }

        public void SetParameter(int index, float value)
        {
            _parameters[index].SetValueFromHost(value);
        }

        public float GetParameter(int index)
        {
            return (float)_parameters[index].RealValue;
        }

        public void SetProgram(int programNumber)
        {
            // TODO optimize
            _activeProgram = programNumber;
            foreach (var parameter in ActiveProgram.Parameters)
                _parameters.Find( x=> x.Name == parameter.Key).SetValueFromPlugin(parameter.Value);
        }

        public int GetProgram()
        {
            return _activeProgram;
        }

        public void SetProgramName(string name)
        {
            ActiveProgram.Name = name;
        }

        public string GetProgramName()
        {
            return ActiveProgram.Name;
        }

        public string GetParameterLabel(int index)
        {
            return _parameters[index].Label;
        }

        public string GetParameterDisplay(int index)
        {
            return _parameters[index].GetDisplayValue();
        }

        public string GetParameterName(int index)
        {
            return _parameters[index].Name;
        }

        public bool CanParameterBeAutomated(int index)
        {
            return _parameters[index].CanBeAutomated;
        }

        public bool String2Parameter(int index, string str)
        {
            throw new System.NotImplementedException();
        }

        public string GetProgramNameIndexed(int index)
        {
            return _programs[index].Name;
        }

        public bool BeginSetProgram()
        {
            return true;
        }

        public bool EndSetProgram()
        {
            return true;
        }

        public Program CreateProgramFromSerializedParameters(string programName, IEnumerable<string> lines)
        {
            var program = new Program();
            program.Name = programName;

            var prmts = new List<Parameter>(Parameters);

            foreach (var line in lines)
            {
                var sline = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                var paramName = sline[0];
                var valueS = sline[1];

                var parameter = Parameters.FirstOrDefault( x=> x.Name == paramName);//TODO optimize
                if (parameter == null)
                    throw new NullReferenceException();

                prmts.Remove(parameter);
                
                program.Parameters.Add(parameter.Name, parameter.Parse(valueS));
            }

            if (prmts.Any())
                throw new ArgumentException();

            return program;
        }
    }
}
