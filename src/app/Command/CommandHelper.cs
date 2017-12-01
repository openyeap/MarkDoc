using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace Bzway.Writer.App
{
    public class CommandHelper : IEnumerable<ICommand>
    {
        static CommandHelper _this;
        IDictionary<string, ICommand> cmds;
        List<ICommand> list;
        CommandHelper()
        {
            if (cmds == null)
            {
                cmds = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                list = new List<ICommand>();
                foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(m => typeof(ICommand).IsAssignableFrom(m) && !m.IsInterface))
                {
                    var cmd = (ICommand)Activator.CreateInstance(item);
                    if (cmd != null)
                    {
                        list.Add(cmd);
                        foreach (var name in cmd.Name.Split('|'))
                        {
                            cmds.Add(name, cmd);
                        }
                    }
                }
            }
        }
        public static CommandHelper Default
        {
            get
            {
                if (_this == null)
                {
                    _this = new CommandHelper();
                }
                return _this;
            }
        }
        public ICommand this[string key]
        {
            get
            {
                if (cmds.TryGetValue(key, out ICommand cmd))
                {
                    return cmd;
                }
                else
                {
                    return new HelpCommand();
                }
            }
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            return this.list.OrderBy(m => m.Name).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}