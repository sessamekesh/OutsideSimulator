using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.Renderable;
using System.Diagnostics;

namespace OutsideSimulator.Commands
{
    /// <summary>
    /// Command to create a new scene. Contains a reference to the scene which it created.
    /// </summary>
    public class CreateNewDefaultScene
    {
        #region Properties
        protected bool IsExecuted;
        public SceneGraph SceneGraph { get; private set; }
        public Camera Camera { get; private set; }
        #endregion

        public CreateNewDefaultScene()
        {
            IsExecuted = false;
            SceneGraph = null;
            Camera = null;
        }

        #region ICommand
        /// <summary>
        /// Initialize the scene, that's pretty much it.
        /// </summary>
        public void CreateNewScene(out Camera o_Camera, out SceneGraph o_SceneGraph)
        {
            if (!IsExecuted)
            {
                // Create the scene graph
                SceneGraph = new SceneGraph(SlimDX.Matrix.Identity);
                IsExecuted = true;

                // Attach default terrain
                var TerrainThing = new SceneGraph(
                    SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity, new SlimDX.Vector3(1.0f, 1.0f, 1.0f), SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity, new SlimDX.Vector3(0.0f, 0.0f, 1.0f))
                );
                TerrainThing.Renderable = new TerrainRenderable(100.0f, 100.0f, 5, 5);

                SceneGraph.AttachChild("TerrainThing", TerrainThing);

                // Attach test object
                var CubeThing = new SceneGraph(
                    SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity, new SlimDX.Vector3(1.0f, 1.0f, 1.0f), SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity, new SlimDX.Vector3(38.0f, 1.0f, 1.0f))
                );
                CubeThing.Renderable = new TestRenderable();

                SceneGraph.AttachChild("CubeThing", CubeThing);

                // Create default camera
                var testCamera = new FlightCamera(new SlimDX.Vector3(10.0f, 8.0f, 0.0f));
                Camera = testCamera;
            }
            else
            {
                Debug.WriteLine("Already performed action CreateNewScene");
            }

            o_Camera = Camera;
            o_SceneGraph = SceneGraph;
        }
        #endregion
    }
}
