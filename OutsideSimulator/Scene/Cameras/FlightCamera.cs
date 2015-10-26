using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutsideSimulator.Commands.Events;
using System.Drawing;

using OutsideSimulator.D3DCore;

namespace OutsideSimulator.Scene.Cameras
{
    public class FlightCamera : Camera, MouseDownSubscriber, MouseMoveSubscriber, MouseUpSubscriber, KeyDownSubscriber, KeyUpSubscriber
    {
        #region State Information
        private bool _isWDown;
        private bool _isSDown;
        private bool _isADown;
        private bool _isDDown;
        private bool _isRightMouseDown;

        private Point _lastMousePos;

        private float _forwardSpeed;
        private float _rightSpeed;

        private float _phi;
        private float _theta;

        protected SlimDX.Vector3 BasePosition;
        #endregion

        #region Rate Constantes
        public readonly float FlyRate = 0.015f;
        public readonly float RotateRate = 0.003f;
        #endregion

        public FlightCamera(SlimDX.Vector3 startPos) : base()
        {
            _lastMousePos = new Point(0, 0);
            _isWDown = false;
            _isSDown = false;
            _isADown = false;
            _isDDown = false;

            _isRightMouseDown = false;

            _forwardSpeed = 0.0f;
            _rightSpeed = 0.0f;

            _phi = MathF.PI / 2.0f;
            _theta = 0.0f;

            BasePosition = startPos;

            OutsideSimulatorApp.GetInstance().Subscribe(this);
        }

        #region Subscriptions
        public void OnKeyPress(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _isWDown = true;
                    if (_isSDown)
                        _forwardSpeed = 0.0f;
                    else
                        _forwardSpeed = 1.0f;
                    break;
                case Keys.S:
                    _isSDown = true;
                    if (_isWDown)
                        _forwardSpeed = 0.0f;
                    else
                        _forwardSpeed = -1.0f;
                    break;
                case Keys.A:
                    _isADown = true;
                    if (_isDDown)
                        _rightSpeed = 0.0f;
                    else
                        _rightSpeed = -1.0f;
                    break;
                case Keys.D:
                    _isDDown = true;
                    if (_isADown)
                        _rightSpeed = 0.0f;
                    else
                        _rightSpeed = 1.0f;
                    break;
            }
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _isWDown = false;
                    if (_isSDown)
                        _forwardSpeed = -1.0f;
                    else
                        _forwardSpeed = 0.0f;
                    break;
                case Keys.S:
                    _isSDown = false;
                    if (_isWDown)
                        _forwardSpeed = 1.0f;
                    else
                        _forwardSpeed = 0.0f;
                    break;
                case Keys.A:
                    _isADown = false;
                    if (_isDDown)
                        _rightSpeed = 1.0f;
                    else
                        _rightSpeed = 0.0f;
                    break;
                case Keys.D:
                    _isDDown = false;
                    if (_isADown)
                        _rightSpeed = -1.0f;
                    else
                        _rightSpeed = 0.0f;
                    break;
            }
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                _isRightMouseDown = true;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (_isRightMouseDown)
            {
                // Rotate Camera by Amount
                _theta -= (e.X - _lastMousePos.X) * RotateRate;
                _phi += (e.Y - _lastMousePos.Y) * RotateRate;

                _phi = MathF.Clamp(_phi, 0.01f, MathF.PI - 0.01f);
            }

            _lastMousePos = e.Location;
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                _isRightMouseDown = false;
        }
        #endregion

        public override void Update(float dt)
        {
            base.Update(dt);

            SlimDX.Vector3 ForwardDir = LookAt - Position;
            SlimDX.Vector3 RightDir = SlimDX.Vector3.Cross(ForwardDir, Up) * -1.0f;

            BasePosition += (ForwardDir * _forwardSpeed * FlyRate) + (RightDir * _rightSpeed * FlyRate);

            // Get camera position from polar coords, plus camera origin
            var x = MathF.SinF(_phi) * MathF.CosF(_theta);
            var z = MathF.SinF(_phi) * MathF.SinF(_theta);
            var y = MathF.CosF(_phi);

            Position = BasePosition;
            LookAt = Position + new SlimDX.Vector3(x, y, z);
        }
    }
}
