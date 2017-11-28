using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Bzway.Writer.App
{
    public class CommandHelper
    {
        static CommandHelper _this;
        IDictionary<string, ICommand> cmds;
        CommandHelper()
        {
            if (cmds == null)
            {
                cmds = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(m => typeof(ICommand).IsAssignableFrom(m) && !m.IsInterface))
                {
                    var cmd = (ICommand)Activator.CreateInstance(item);
                    if (cmd != null)
                    {
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
    }
}