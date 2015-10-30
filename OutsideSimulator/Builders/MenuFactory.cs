using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutsideSimulator.Scene;

namespace OutsideSimulator.Builders
{
    /// <summary>
    /// "MenuFactory" is a bit of a misnomer, since there is indeed only one menu being built here...
    /// </summary>
    public static class MenuFactory
    {
        public static Menu BuildMainMenu()
        {
            Menu MainMenu = new Menu("../../assets/MenuBases/MainMenu.dds", System.Windows.Forms.Keys.Escape);

            MainMenu.AddAction("../../assets/MenuButtons/NewSimulation.dds", () => {
                // Create a new simulation
                OutsideSimulatorApp.GetInstance().CreateNewScene();
            });

            MainMenu.AddAction("../../assets/MenuButtons/ExitButton.dds", () => { OutsideSimulatorApp.GetInstance().Close(); });

            return MainMenu;
        }
    }
}
