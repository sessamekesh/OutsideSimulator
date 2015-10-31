using System.Collections.Generic;
using System;

using System.Xml.Linq;
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

        public SceneGraph(Matrix transformMatrix, IRenderable renderable = null)
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
            Renderable = renderable;
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

        public void RemoveDescendent(SceneGraph ToRemove)
        {
            if (Children.ContainsValue(ToRemove))
            {
                Children.Remove(Children.First((x) => x.Value == ToRemove).Key);
            }
            else
            {
                foreach (var Child in Children)
                {
                    Child.Value.RemoveDescendent(ToRemove);
                }
            }
        }

        public void RemoveDescendent(string ToRemove)
        {
            if (Children.ContainsKey(ToRemove))
            {
                Children.Remove(ToRemove);
            }
            else
            {
                foreach (var Child in Children)
                {
                    Child.Value.RemoveDescendent(ToRemove);
                }
            }
        }

        public SceneGraph GetDescendant(string Name)
        {
            if (Children.ContainsKey(Name))
            {
                return Children[Name];
            }
            else
            {
                foreach (var Child in Children)
                {
                    var sg = Child.Value.GetDescendant(Name);
                    if (sg != null)
                    {
                        return sg;
                    }
                }
            }

            return null;
        }

        public static SceneGraph Deserialize(string XMLString)
        {
            // TODO KAM: Complete the deserialize here (and also in your actions, camera...)
            XElement xe = XElement.Parse(XMLString);

            if (xe.Name != "SceneGraph")
            {
                return null;
            }
            else
            {
                Vector3 translation = Util.DeserializeVector((xe.Nodes().First((x) => (x as XElement).Name == "Translation") as XElement).FirstNode.ToString());
                Quaternion rotation = Util.DeserializeQuaternion((xe.Nodes().First((x) => (x as XElement).Name == "Rotation") as XElement).FirstNode.ToString());
                Vector3 scale = Util.DeserializeVector((xe.Nodes().First((x) => (x as XElement).Name == "Scaling") as XElement).FirstNode.ToString());
                IRenderable child = RenderableFactory.Deserialize((xe.Nodes().First((x) => (x as XElement).Name == "Renderable") as XElement).FirstNode.ToString());

                var tr = new SceneGraph(Matrix.Transformation(Vector3.Zero, Quaternion.Identity, scale, Vector3.Zero, rotation, translation), child);

                // Do all children...
                foreach (var xchild in (xe.Nodes().Where((x) => (x as XElement).Name == "Child")))
                {
                    tr.AttachChild(
                        ((xchild as XElement).Nodes().First((x) => (x as XElement).Name == "Name") as XElement).Value,
                        SceneGraph.Deserialize(((xchild as XElement).Nodes().First((x) => (x as XElement).Name == "SceneGraph") as XElement).ToString())
                    );
                }

                return tr;
            }
        }

        public static XElement Serialize(SceneGraph SceneGraph)
        {
            var tr = new XElement("SceneGraph",
                new XElement("Translation", Util.SerializeVector(SceneGraph.Translation)),
                new XElement("Rotation", Util.SerializeQuaternion(SceneGraph.Rotation)),
                new XElement("Scaling", Util.SerializeVector(SceneGraph.Scale)),
                new XElement("Renderable", RenderableFactory.Serialize(SceneGraph.Renderable))
            );

            foreach (var Child in SceneGraph.Children)
            {
                tr.Add(new XElement("Child",
                    new XElement("Name", Child.Key),
                    Serialize(Child.Value)
                ));
            }

            return tr;
        }
    }
}
