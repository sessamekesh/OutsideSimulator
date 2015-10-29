using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

using OutsideSimulator.Scene;

namespace OutsideSimulator.Commands
{
    /// <summary>
    /// Generate or parse files
    /// </summary>
    public static class OutsideFileGenerator
    {
        public static string SerializeScene(SceneGraph RootNode, CommandStack CommandStack)
        {
            return new XElement("OutsideSimulatorScene",
                new XElement("Scene", "SCENE DATA HERE"),
                new XElement("CommandStack", "COMMAND STACK HERE")
            ).ToString();
        }

        public static void DeserializeScene(string inString, out SceneGraph RootNode, out CommandStack CommandStack)
        {
            RootNode = null;
            CommandStack = null;
        }
    }
}
