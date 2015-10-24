using System.Windows.Forms;

using OutsideSimulator.D3DCore;

using SlimDX;

namespace OutsideSimulator.Scene.Cameras
{
    /// <summary>
    /// Base class for a camera. The default camera is extremely boring - it just sits there,
    ///  and is incapable of interaction with people.
    /// </summary>
    public class Camera
    {
        #region Dirtyables
        protected Dirtyable<Matrix> ViewMatrix;
        #endregion

        #region Members
        private Vector3 _position;
        protected Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                ViewMatrix.DoTheNasty();
            }
        }

        private Vector3 _lookat;
        protected Vector3 LookAt
        {
            get
            {
                return _lookat;
            }
            set
            {
                _lookat = value;
                ViewMatrix.DoTheNasty();
            }
        }

        private Vector3 _up;
        protected Vector3 Up
        {
            get
            {
                return _up;
            }
            set
            {
                _up = value;
                ViewMatrix.DoTheNasty();
            }
        }
        #endregion

        /// <summary>
        /// Get the D3D view matrix for this camera
        /// </summary>
        /// <returns></returns>
        public virtual Matrix GetViewMatrix()
        {
            return ViewMatrix;
        }

        /// <summary>
        /// Initialize to whatever, it really doesn't matter.
        /// </summary>
        public Camera()
        {
            ViewMatrix = new Dirtyable<Matrix>(() =>
            {
                return Matrix.LookAtLH(Position, LookAt, Up);
            });

            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            LookAt = new Vector3(0.0f, 0.0f, -1.0f);
        }

        /// <summary>
        /// Update the camera by the elapsed time amount given
        /// </summary>
        /// <param name="dt">Time, in seconds, that has elapsed</param>
        public virtual void Update(float dt) { }

        public virtual void OnKeyDown(KeyEventArgs e) { }
        public virtual void OnKeyUp(KeyEventArgs e) { }
        public virtual void OnMouseDown(MouseEventArgs e) { }
        public virtual void OnMouseUp(MouseEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }
        public virtual void OnMouseScroll(MouseEventArgs e) { }
    }
}
