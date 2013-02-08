using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Noodle.Imaging.Tests
{
    [TestFixture]
    public class ImageLayoutBuilderTests
    {
        [Test]
        [TestCase(new object[] { 200, 200, 100, 100, 100, 100}, "rotate=90" )]
        [TestCase(new object[] { 200, 200, 100, 100, 50, 50 }, "width=100" )]
        [TestCase(new object[] { 200, 200, 100, 100, 50, 10}, "width=100&height=20&stretch=fill" )]
        public void TranslatePoints(int imgWidth, int imgHeight, float x, float y, float expectedX, float expectedY, string query)
        {
            PointF result = c.CurrentImageBuilder.TranslatePoints(new PointF[] { new PointF(x, y) }, new Size(imgWidth, imgHeight), new ResizeSettings(query))[0];
            Assert.AreEqual<PointF>(new PointF(expectedX, expectedY), result);
        }
    }
}
