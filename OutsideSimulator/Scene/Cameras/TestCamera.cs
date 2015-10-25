using SlimDX;

namespace OutsideSimulator.Scene.Cameras
{
    /// <summary>
    /// ONLY FOR TESTING
    /// </summary>
    public class TestCamera : Camera
    {
        public TestCamera() : base() { }

        public TestCamera(Vector3 pos, Vector3 lookat, Vector3 up) : base()
        {
            SetPosition(pos);
            SetLookAt(lookat);
            SetUp(up);
        }

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        public void SetLookAt(Vector3 l)
        {
            LookAt = l;
        }

        public void SetLookAt(float x, float y, float z)
        {
            LookAt = new Vector3(x, y, z);
        }

        public void SetUp(Vector3 up)
        {
            Up = up;
        }

        public void SetUp(float x, float y, float z)
        {
            Up = new Vector3(x, y, z);
        }
    }
}
