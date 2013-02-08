using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Noodle.Tests;

namespace Noodle.Imaging.Tests
{
    [TestFixture]
    public class ImageLayoutBuilderTests : Noodle.Tests.TestBase
    {
        private IImageLayoutBuilder _imageLayoutBuilder;


        public override void SetUp()
        {
            base.SetUp();

            _imageLayoutBuilder = new ImageLayoutBuilder();
        }

        [Test]
        //  ScaleMode.DownscaleOnly
        [TestCase(new object[] { 100, 50, 100, 50, "" })]
        [TestCase(new object[] { 100, 50, 50, 25, "width=50" })]
        [TestCase(new object[] { 100, 50, 50, 25, "height=25" })]
        //      Ensure we can't upscale
        [TestCase(new object[] { 100, 50, 100, 50, "height=200" })]
        [TestCase(new object[] { 100, 50, 100, 50, "width=200" })]
        // ScaleMode.UpscaleOnly
        [TestCase(new object[] { 100, 50, 100, 50, "scaleMode=up" })]
        [TestCase(new object[] { 100, 50, 400, 200, "height=200&scale=up" })]
        [TestCase(new object[] { 100, 50, 200, 100, "width=200&scale=up" })]
        //      Ensure we can't downscale
        [TestCase(new object[] { 100, 50, 100, 50, "height=25&scale=up" })]
        [TestCase(new object[] { 100, 50, 100, 50, "width=25&scale=up" })]
        // ScaleModel.Both
        [TestCase(new object[] { 100, 50, 50, 25, "width=50&scale=both" })]
        [TestCase(new object[] { 100, 50, 50, 25, "height=25&scale=both" })]
        [TestCase(new object[] { 100, 50, 400, 200, "height=200&scale=both" })]
        [TestCase(new object[] { 100, 50, 200, 100, "width=200&scale=both" })]
        public void Can_build_sizes_with_scale_mode(int imgWidth, int imgHeight, float expectedWidth, float expectedHeight, string query)
        {
            var layout = _imageLayoutBuilder.BuildLayout(new Size(imgWidth, imgHeight), new ResizeSettings(query));
            AssertLayout(new Size(100, 50), new ResizeSettings("width=50"), 
                image => {

                }, 
                imageArea => {

                });
        }

        [Test]
        [TestCase(new object[] { 50, 50, 50, 50, "mode=stretch" })]
        [TestCase(new object[] { 50, 50, 150, 50, "mode=stretch&width=150&height=50" })]
        public void Can_streatch(int imgWidth, int imgHeight, float expectedWidth, float expectedHeight, string query)
        {
            var layout = _imageLayoutBuilder.BuildLayout(new Size(imgWidth, imgHeight), new ResizeSettings(query));
            layout.CanvasSize.Height.ShouldEqual(expectedHeight);
            layout.CanvasSize.Width.ShouldEqual(expectedWidth);
            layout.DrawTo.Width.ShouldEqual(expectedWidth);
            layout.DrawTo.Height.ShouldEqual(expectedHeight);
        }

        private void AssertLayout(Size sourceSize, ResizeSettings resizeSettings, Action<RectangleF> assertImageArea,
                                 Action<RectangleF> assertImage)
        {

        }
    }
}
