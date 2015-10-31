using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OutsideSimulator.Effects;
using OutsideSimulator.Renderable;

namespace UnitTests.RenderableTests
{
    [TestClass]
    public class TestRenderableTest
    {
        [TestMethod]
        public void CreateTestRenderable()
        {
            var tr = new TestRenderable();

            Assert.IsNotNull(tr, "is not null");
        }

        [TestMethod]
        public void GetVertexList()
        {
            var tr = new TestRenderable();

            //
            // Basic Effect Test
            //
            var BasicEffectVerts = tr.GetVertexList(EffectsGlobals.BasicEffectName);
            Assert.IsNotNull(BasicEffectVerts, "BasicEffectVerts list should not be null");
            Assert.IsTrue(BasicEffectVerts.Length > 0, "BasicEffectsVerts list should not be empty");
            Assert.IsInstanceOfType(BasicEffectVerts.ToArray()[0], typeof(OutsideSimulator.Effects.BasicEffect.BasicEffectVertex), "produces basic vertex type for basic effect");

            //
            // Menu Effect Test
            //
            try
            {
                var MenuEffectVerts = tr.GetVertexList(EffectsGlobals.MenuEffectName);
                Assert.Fail("Exception should be thrown when attempting to retrieve menu vertices on a TestObject");
            }
            catch (CannotResolveVerticesException crve)
            {
                Assert.AreEqual(EffectsGlobals.MenuEffectName, crve.EffectName, "EffectName given should be MenuEffect when failed on MenuEffect");
                Assert.AreEqual("TestRenderable", crve.RenderableName);
            }

            //
            // Arbirary String Effect Test
            //
            try
            {
                var MenuEffectVerts = tr.GetVertexList("ArbitraryEffect");
                Assert.Fail("Exception should be thrown when attempting to retrieve menu vertices on a TestObject");
            }
            catch (CannotResolveVerticesException crve)
            {
                Assert.AreEqual("ArbitraryEffect", crve.EffectName);
                Assert.AreEqual("TestRenderable", crve.RenderableName);
            }
        }

        [TestMethod]
        public void GetIndexList()
        {
            var tr = new TestRenderable();

            //
            // Basic Effect Test
            //
            var BasicEffectIndices = tr.GetIndexList(EffectsGlobals.BasicEffectName);
            Assert.IsNotNull(BasicEffectIndices, "BasicEffectIndices list should not be null");
            Assert.IsTrue(BasicEffectIndices.Length > 0, "BasicEffectsIndices list should not be empty");
            Assert.IsInstanceOfType(BasicEffectIndices.ToArray()[0], typeof(uint), "produces uint type for basic effect");

            //
            // Menu Effect Test
            //
            try
            {
                var MenuEffectIndices = tr.GetIndexList(EffectsGlobals.MenuEffectName);
                Assert.Fail("Exception should be thrown when attempting to retrieve menu indices on a TestObject");
            }
            catch (CannotResolveIndicesException crve)
            {
                Assert.AreEqual(EffectsGlobals.MenuEffectName, crve.EffectName, "EffectName given should be MenuEffect when failed on MenuEffect");
                Assert.AreEqual("TestRenderable", crve.RenderableName);
            }

            //
            // Arbirary String Effect Test
            //
            try
            {
                var MenuEffectIndices = tr.GetIndexList("ArbitraryEffect");
                Assert.Fail("Exception should be thrown when attempting to retrieve menu indices on a TestObject");
            }
            catch (CannotResolveIndicesException crve)
            {
                Assert.AreEqual("ArbitraryEffect", crve.EffectName);
                Assert.AreEqual("TestRenderable", crve.RenderableName);
            }
        }

        [TestMethod]
        public void Serialization()
        {
            var tr = new TestRenderable();
            var tr2 = RenderableFactory.Deserialize(RenderableFactory.Serialize(tr).ToString());

            Assert.IsInstanceOfType(tr2, typeof(TestRenderable), "Deserialized result should be an instance of TestRenderable");
        }
    }
}
