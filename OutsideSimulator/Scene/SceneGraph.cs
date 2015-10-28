using System.Collections.Generic;
using System;

using System.Linq;

using SlimDX;

using OutsideSimulator.D3DCore;
using OutsideSimulator.Renderable;

namespace OutsideSimulator.Scene
{
    public class SceneGraph
    {
        #region Dirtyable Properties
        protected Dirtyable<Matrix> D_Transform;
        protected Dirtyable<Matrix> D_WorldTransform;
        #endregion

        #region Base Properties
        private Vector3 _translation;
        public  Vector3 Translation
        {
            get
            {
                return _translation;
            }
            set
            {
                _translation = value;
                D_Transform.DoTheNasty();
                D_WorldTransform.DoTheNasty();
            }
        }

        private Quaternion _rotation;
        public Quaternion Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                D_Transform.DoTheNasty();
                D_WorldTransform.DoTheNasty();
            }
        }

        private Vector3 _scale;
        public Vector3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                D_Transform.DoTheNasty();
                D_WorldTransform.DoTheNasty();
            }
        }

        private SceneGraph _parent;
        public SceneGraph Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                D_WorldTransform.DoTheNasty();
            }
        }

        public Dictionary<string, SceneGraph> Children;
        public IRenderable Renderable;

        public Matrix WorldTransform
        {
            get
            {
                return D_WorldTransform;
            }
        }
        public Matrix Transform
        {
            get
            {
                return D_Transform;
            }
        }
        #endregion

        #region Methods
        #endregion

        public SceneGraph(Matrix transformMatrix)
        {
            //
            // Setup dirtyables...
            //
            D_Transform = new Dirtyable<Matrix>(() => { return Matrix.Transformation(Vector3.Zero, Quaternion.Identity, Scale, Vector3.Zero, Rotation, Translation); });
            D_WorldTransform = new Dirtyable<Matrix>(() =>
            {
                if (Parent == null)
                {
                    return D_Transform.Value;
                }
                else
                {
                    // TODO KAM: I'm not sure if this is the proper order in which to multiply matrices to find the WorldTransform
                    return D_Transform.Value * Parent.D_WorldTransform.Value;
                }
            });

            //
            // Setup transformation data...
            //
            Vector3 trans;
            Quaternion rot;
            Vector3 scale;
            transformMatrix.Decompose(out scale, out rot, out trans);
            Translation = trans;
            Rotation = rot;
            Scale = scale;

            //
            // Setup other data...
            //
            Parent = null;
            Children = new Dictionary<string, SceneGraph>();
        }

        public BoundingBox? GetBoundingBox()
        {
            if (Renderable == null)
            {
                return null;
            }
            else
            {
                return BoundingBox.FromPoints(
                    Renderable.GetVertexList(Effects.EffectsGlobals.BasicEffectName).Select((x) =>
                    {
                        return ((Effects.BasicEffect.BasicEffectVertex)x).Pos;
                    }).ToArray());
            }
        }

        public void AttachChild(string Name, SceneGraph Child)
        {
            Children.Add(Name, Child);
            Child.Parent = this;
        }
    }
}
