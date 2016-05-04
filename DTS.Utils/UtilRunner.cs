using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public class UtilRunner
    {
        private readonly List<UtilBase> _utils;

        public UtilRunner()
        {
            _utils = new List<UtilBase>();
        }
        public UtilRunner Util(UtilBase utilBase)
        {
            _utils.Add(utilBase);
            return this;
        }

        public void Run(IInput input, IOutput output)
        {
            while (true)
            {
                var line = input.ReadLine();
                
                if (line == "exit")
                {
                    break;
                }

                var util = _utils.SingleOrDefault(x => x.Name == line);

                if (util == null)
                {
                    output.WriteLine($"Unrecognised utility: {line}");
                }
                else
                {
                    util.Run(input, output);
                }
            }
        }
    }

    public interface IInput
    {
        string ReadLine();
    }
}