using System.Collections.Generic;

namespace Bzway.Writer.App
{

    public interface ICommand
    {
        string Name { get; }
        void Execute(IEnumerable<string> args);

        string Usage { get; }
    }
}
