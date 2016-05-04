using System;
using DTS.Utils;
using DTS.Utils.Core;

namespace DTS.Cli
{
    class InputOutput : IInput, IOutput
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        public void WriteReturnValue(ReturnValue returnValue)
        {
            Console.WriteLine(returnValue.Message);
        }
    }
}