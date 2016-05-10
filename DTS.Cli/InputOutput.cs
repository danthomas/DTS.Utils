﻿using System;
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

        public void WriteLines(params string[] lines)
        {
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public void WriteReturnValue(ReturnValue returnValue)
        {
            Console.WriteLine(returnValue.Message);
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}