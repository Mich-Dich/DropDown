namespace UnitTest
{
    using Xunit;
    using Core.UI;
    using System.Numerics;

    public class horizontalBoxTest
    {
        [Fact]
        public void TestHorizontalBoxAddRemoveElement()
        {
            var box = new HorizontalBox(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(10, 10), new System.Numerics.Vector2(1, 1));
            var element = new Text("Test");
            box.AddElement(element);
            Assert.Contains(element, box.elements);
            box.RemoveElement(element);
            Assert.DoesNotContain(element, box.elements);
        }
    }
}