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

        #region ScaleMode

        [Test]
        public void Can_scale_down_only()
        {
            // nothing changes
            AssertLayout("width=100&height=50", "", 
                image => {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            // assert we can scale down
            AssertLayout("width=100&height=50", "width=50",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(25);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(25);
                });
            // assert we can't scale up
            AssertLayout("width=100&height=50", "width=200",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
        }

        [Test]
        public void Can_scale_up_only()
        {
            // nothing changes
            AssertLayout("width=100&height=50", "scale=up",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            // assert we can scale up
            AssertLayout("width=100&height=50", "width=200&scale=up",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(100);
                });
            // assert we can't scale down
            AssertLayout("width=100&height=50", "width=50&scale=up",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
        }

        [Test]
        public void Can_scale_both()
        {
            // nothing changes
            AssertLayout("width=100&height=50", "scale=both",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            // assert we can scale up
            AssertLayout("width=100&height=50", "width=200&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(100);
                });
            // assert we can scale down
            AssertLayout("width=100&height=50", "width=50&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(25);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(25);
                });
        }

        #endregion

        #region Fit

        [Test]
        public void Can_auto_choose_fit_based_on_values()
        {
            // both with and height given, we are using Pad, which adds padding
            AssertLayout("width=100&height=50", "width=200&height=200&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    // we anchor center by default
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(200);
                    image.Top.ShouldEqual(50);
                    image.Bottom.ShouldEqual(150);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(200);
                });
            // specifying just width or height will result in using Max
            AssertLayout("width=100&height=50", "width=200&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(100);
                });
            // specifying just width or height will result in using Max
            AssertLayout("width=100&height=50", "height=200&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(400);
                    image.Height.ShouldEqual(200);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(400);
                    canvas.Height.ShouldEqual(200);
                });
        }

        [Test]
        public void Can_fit_stretch()
        {
            // stretch behaves like max if only width or height specified
            AssertLayout("width=200&height=100", "height=50&mode=stretch",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            AssertLayout("width=200&height=100", "width=100&mode=stretch",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            // can stretch
            AssertLayout("width=200&height=100", "height=50&width=50&mode=stretch",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(50);
                });
        }

        [Test]
        public void Can_fit_max()
        {
            AssertLayout("width=200&height=100", "height=50&width=50&mode=max",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(25);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(25);
                });
            AssertLayout("width=200&height=100", "height=50&mode=max",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            AssertLayout("width=200&height=100", "width=50&mode=max",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(25);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(25);
                });
        }

        [Test]
        public void Can_fit_pad()
        {
            // specifying only one values acts like max
            AssertLayout("width=200&height=100", "height=50&mode=pad",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            // too long
            AssertLayout("width=200&height=100", "height=100&width=100&mode=pad",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(100);
                    image.Top.ShouldEqual(25);
                    image.Bottom.ShouldEqual(75);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
            // too tall
            AssertLayout("width=100&height=200", "height=100&width=100&mode=pad",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(25);
                    image.Right.ShouldEqual(75);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(100);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
        }

        [Test]
        public void Can_fit_crop()
        {
            // specifying only one values acts like max
            AssertLayout("width=200&height=100", "height=50&mode=crop",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(50);
                });
            // too long
            AssertLayout("width=200&height=100", "height=50&width=50&mode=crop",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(50);
                    image.Left.ShouldEqual(-25);
                    image.Right.ShouldEqual(75);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(50);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(50);
                });
            // too tall
            AssertLayout("width=100&height=200", "height=50&width=50&mode=crop",
                image =>
                {
                    image.Width.ShouldEqual(50);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(50);
                    image.Top.ShouldEqual(-25);
                    image.Bottom.ShouldEqual(75);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(50);
                    canvas.Height.ShouldEqual(50);
                });
        }

        #endregion

        ////  ScaleMode.DownscaleOnly
        //[TestCase(new object[] { 100, 50, 100, 50, "" })]
        //[TestCase(new object[] { 100, 50, 50, 25, "width=50" })]
        //[TestCase(new object[] { 100, 50, 50, 25, "height=25" })]
        ////      Ensure we can't upscale
        //[TestCase(new object[] { 100, 50, 100, 50, "height=200" })]
        //[TestCase(new object[] { 100, 50, 100, 50, "width=200" })]
        //// ScaleMode.UpscaleOnly
        //[TestCase(new object[] { 100, 50, 100, 50, "scaleMode=up" })]
        //[TestCase(new object[] { 100, 50, 400, 200, "height=200&scale=up" })]
        //[TestCase(new object[] { 100, 50, 200, 100, "width=200&scale=up" })]
        ////      Ensure we can't downscale
        //[TestCase(new object[] { 100, 50, 100, 50, "height=25&scale=up" })]
        //[TestCase(new object[] { 100, 50, 100, 50, "width=25&scale=up" })]
        //// ScaleModel.Both
        //[TestCase(new object[] { 100, 50, 50, 25, "width=50&scale=both" })]
        //[TestCase(new object[] { 100, 50, 50, 25, "height=25&scale=both" })]
        //[TestCase(new object[] { 100, 50, 400, 200, "height=200&scale=both" })]
        //[TestCase(new object[] { 100, 50, 200, 100, "width=200&scale=both" })]


        //[Test]
        //[TestCase(new object[] { 50, 50, 50, 50, "mode=stretch" })]
        //[TestCase(new object[] { 50, 50, 150, 50, "mode=stretch&width=150&height=50" })]
        //public void Can_streatch(int imgWidth, int imgHeight, float expectedWidth, float expectedHeight, string query)
        //{
        //    var layout = _imageLayoutBuilder.BuildLayout(new Size(imgWidth, imgHeight), new ResizeSettings(query));
        //    layout.CanvasSize.Height.ShouldEqual(expectedHeight);
        //    layout.CanvasSize.Width.ShouldEqual(expectedWidth);
        //    layout.DrawTo.Width.ShouldEqual(expectedWidth);
        //    layout.DrawTo.Height.ShouldEqual(expectedHeight);
        //}

        private void AssertLayout(string sourceSize, string resizeSettings, Action<RectangleF> assertImage, Action<SizeF> assertCanvas)
        {
            var sourceSizeSettings = new ResizeSettings(sourceSize);
            var result = _imageLayoutBuilder.BuildLayout(new Size(sourceSizeSettings.Width, sourceSizeSettings.Height), new ResizeSettings(resizeSettings));
            if (assertCanvas != null)
                assertCanvas(result.CanvasSize);
            if (assertImage != null)
                assertImage(result.Image);
        }

        private void AssertOrigin(RectangleF image)
        {
            image.X.ShouldEqual(0);
            image.Y.ShouldEqual(0);
        }
    }
}
