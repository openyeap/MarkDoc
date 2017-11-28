using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace CommandLine
{
  
    public interface ICommand
    {
        string Name { get; }
        void Execute(IEnumerable<string> args);
        object Options { get; }
    }
}
