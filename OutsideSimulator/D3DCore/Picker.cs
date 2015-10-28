using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;

namespace OutsideSimulator.D3DCore
{
    public static class Picker
    {
        public static SceneGraph PickClosestObject(SceneGraph root, Camera Camera, Matrix ProjMatrix, Vector2 ClickPoint, Vector2 ScreenDims)
        {
            // Convert screen pixel to view space
            Vector2 viewSpacePixel = new Vector2((2.0f * ClickPoint.X / ScreenDims.X - 1.0f) / ProjMatrix.M11, (-2.0f * ClickPoint.Y / ScreenDims.Y + 1.0f) / ProjMatrix.M22);
            Console.WriteLine("ScreenDim: " + ScreenDims.ToString());
            Console.WriteLine("ClickPoint: " + ClickPoint.ToString());
            Console.WriteLine("ViewSpace: " + viewSpacePixel.ToString());
            Ray viewRay = new Ray(new Vector3(), new Vector3(viewSpacePixel.X, viewSpacePixel.Y, 1.0f));

            // View Ray is now the thing we will use...
            Matrix toWorld = Matrix.Invert(Camera.GetViewMatrix());
            viewRay.Direction = Vector3.TransformNormal(viewRay.Direction, toWorld);
            viewRay.Position = Vector3.TransformCoordinate(viewRay.Position, toWorld);

            return pickClosestObject(root, Camera.GetViewMatrix(), viewRay);
        }

        private static SceneGraph pickClosestObject(SceneGraph Root, Matrix ViewProj, Ray ViewRay2)
        {
            // TODO KAM: This totally isn't working.
            // (1) If the node we're looking at may be picked, select it
            //var toWorld = Matrix.Invert(Camera.GetViewMatrix() * ProjMatrix);

            //viewRay = new Ray(Vector3.TransformCoordinate(viewRay.Position, toWorld), Vector3.TransformNormal(viewRay.Direction, toWorld));
            //viewRay.Direction.Normalize();

            var invWorld = Matrix.Invert(Root.WorldTransform);
            var oVR2 = new Ray(ViewRay2.Position, ViewRay2.Direction);
            oVR2.Direction = Vector3.TransformNormal(oVR2.Direction, invWorld);
            oVR2.Position = Vector3.TransformCoordinate(oVR2.Position, invWorld);
            oVR2.Direction.Normalize();

            var boundingBox = Root.GetBoundingBox();
            float dist;
            boundingBox = (Ray.Intersects(oVR2, boundingBox.Value, out dist)) ? boundingBox : null;
            var element = (boundingBox == null) ? null : Root;

            // (2) Find all children picked
            foreach (var child in Root.Children)
            {
                BoundingBox? childbox;
                var childret = pickClosestObject(child.Value, ViewProj, ViewRay2);
                if (childret == null)
                {
                    childbox = null;
                }
                else
                {
                    childbox = childret.GetBoundingBox();
                }

                if (boundingBox == null)
                {
                    boundingBox = childbox;
                }
                else if (childbox != null)
                {
                    float currDist, otherDist;
                    Ray.Intersects(oVR2, boundingBox.Value, out currDist);

                    var oVR3 = new Ray(ViewRay2.Position, ViewRay2.Direction);
                    var cInvWorld = Matrix.Invert(childret.WorldTransform);
                    oVR3.Direction = Vector3.TransformNormal(oVR3.Direction, cInvWorld);
                    oVR3.Position = Vector3.TransformCoordinate(oVR3.Position, cInvWorld);

                    Ray.Intersects(oVR3, childbox.Value, out otherDist);

                    // (3) Take the smallest of the list and return it
                    if (otherDist < currDist)
                    {
                        boundingBox = childbox;
                        element = child.Value;
                    }
                }
            }

            return element;
        }
    }
}
