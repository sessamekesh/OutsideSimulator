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
            Menu ModifySelectedMenu = new Menu("../../assets/MenuBases/ModifySelected.dds", null);

            //
            // Modify Selected Menu
            //
            ModifySelectedMenu.AddAction("../../assets/MenuButtons/MoveObject.dds", () =>
            {
                if (OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode == null) return;

                Scene.UserInteractions.ObjectMover.getInstance().SetNode(
                    OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode
                );
            });

            ModifySelectedMenu.AddAction("../../assets/MenuButtons/UseNextTexture.dds", () =>
            {
                OutsideSimulatorApp.GetInstance().ObjectPicker.GetNextRenderable();
            });

            ModifySelectedMenu.AddAction("../../assets/MenuButtons/DeleteObject.dds", () =>
            {
                OutsideSimulatorApp.GetInstance().ObjectPicker.DeleteSelected();
            });

            //
            // Main Menu
            //
            MainMenu.AddAction("../../assets/MenuButtons/NewSimulation.dds", () =>
            {
                // Create a new simulation
                OutsideSimulatorApp.GetInstance().CreateNewScene();
            });

            MainMenu.AddAction("../../assets/MenuButtons/ChangeFillColor.dds", () =>
            {
                // Open a color dialog
                var ColorDialog = new System.Windows.Forms.ColorDialog();
                ColorDialog.Color = (System.Drawing.Color) OutsideSimulatorApp.GetInstance().FillColor;

                if (ColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OutsideSimulatorApp.GetInstance().FillColor = ColorDialog.Color;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Pick background color aborted", "Outside Simulator 2015");
                }
            });

            MainMenu.AddAction("../../assets/MenuButtons/Undo.dds", () =>
            {
                OutsideSimulatorApp.GetInstance().CommandStack.Undo();
            });

            MainMenu.AddSubmenu("../../assets/MenuButtons/ModifySelected.dds", ModifySelectedMenu);

            MainMenu.AddAction("../../assets/MenuButtons/ExitButton.dds", () => { OutsideSimulatorApp.GetInstance().Close(); });

            return MainMenu;
        }
    }
}
