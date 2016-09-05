using System;
using System.Collections.Generic;
using System.Linq;
using Jacobi.Vst.Core;

namespace Syntage.Framework.Parameters
{
    public class ParametersManager
    {
        private List<Parameter> _parametersList;
        private Dictionary<string, Parameter> _parametersDict;
        private List<Program> _programs;
        private int _activeProgram;

        public IVstHostCommands20 HostAutomation { get; private set; }

        public Program ActiveProgram
        {
            get { return _programs[_activeProgram]; }
        }

        public IEnumerable<Parameter> Parameters
        {
            get { return _parametersList; }
        }

        public IEnumerable<Program> Programs
        {
            get { return _programs; }
        }

        public void SetParameters(IEnumerable<Parameter> parameters)
        {
            _parametersList = new List<Parameter>(parameters);
            _parametersDict = new Dictionary<string, Parameter>();

            for (int i = 0; i < _parametersList.Count; ++i)
            {
                var parameter = _parametersList[i];
                parameter.Index = i;
                parameter.Manger = this;

                _parametersDict.Add(parameter.Name, parameter);
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
            _parametersList[index].SetValueFromHost(value);
        }
        
        public Parameter GetParameter(int index)
        {
            return _parametersList[index];
        }

        public int GetParameterIndex(Parameter parameter)
        {
            return _parametersList.IndexOf(parameter);
        }

        public Parameter FindParameter(string parameterName)
        {
            return _parametersList.FirstOrDefault(x => parameterName == x.Name);
        }

        public void SetProgram(int programNumber)
        {
            _activeProgram = programNumber;
            foreach (var parameter in ActiveProgram.Parameters)
                _parametersDict[parameter.Key].SetValueFromPlugin(parameter.Value);
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
            return _parametersList[index].Label;
        }

        public string GetParameterDisplay(int index)
        {
            return _parametersList[index].GetDisplayValue();
        }

        public string GetParameterName(int index)
        {
            return _parametersList[index].Name;
        }

        public bool CanParameterBeAutomated(int index)
        {
            return _parametersList[index].CanBeAutomated;
        }

        public bool String2Parameter(int index, string str)
        {
            throw new NotImplementedException();
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
                var sline = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                var paramName = sline[0];
                var valueS = sline[1];

                var parameter = _parametersDict[paramName];
                if (parameter == null)
                    throw new NullReferenceException();

                prmts.Remove(parameter);

                program.Parameters.Add(parameter.Name, parameter.Parse(valueS));
            }

            if (prmts.Any())
                throw new ArgumentException();

            return program;
        }

        public Program CreateProgramFromSerializedParameters(string programName, string preset)
        {
            return CreateProgramFromSerializedParameters(programName,
                preset.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
