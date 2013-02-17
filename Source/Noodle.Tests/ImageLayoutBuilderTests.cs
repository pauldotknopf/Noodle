using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Noodle.Imaging;
using Noodle.Tests;

namespace Noodle.Tests
{
    [TestFixture]
    public class ImageLayoutBuilderTests : TestBase
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
            AssertLayout("width=200&height=100", "",
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
            AssertLayout("width=200&height=100", "width=100",
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
            // assert we can't scale up
            AssertLayout("width=200&height=100", "width=400",
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
        }

        [Test]
        public void Can_scale_up_only()
        {
            // nothing changes
            AssertLayout("width=200&height=100", "scale=up",
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
            // assert we can scale up
            AssertLayout("width=200&height=100", "width=400&scale=up",
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
            // assert we can't scale down
            AssertLayout("width=200&height=100", "width=50&scale=up",
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
        }

        [Test]
        public void Can_scale_both()
        {
            // nothing changes
            AssertLayout("width=200&height=100", "scale=both",
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
            // assert we can scale up
            AssertLayout("width=200&height=100", "width=400&scale=both",
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
            // assert we can scale down
            AssertLayout("width=200&height=100", "width=100&scale=both",
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

        #endregion

        #region Fit

        [Test]
        public void Can_auto_choose_fit_based_on_values()
        {
            // both with and height given, we are using Pad, which adds padding
            AssertLayout("width=200&height=100", "width=400&height=400&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(400);
                    image.Height.ShouldEqual(200);
                    // we anchor center by default
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(400);
                    image.Top.ShouldEqual(100);
                    image.Bottom.ShouldEqual(300);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(400);
                    canvas.Height.ShouldEqual(400);
                });
            // specifying just width or height will result in using Max
            AssertLayout("width=200&height=100", "width=400&scale=both",
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
            // specifying just width or height will result in using Max
            AssertLayout("width=200&height=100", "height=400&scale=both",
                image =>
                {
                    image.Width.ShouldEqual(800);
                    image.Height.ShouldEqual(400);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(800);
                    canvas.Height.ShouldEqual(400);
                });
        }

        [Test]
        public void Can_fit_stretch()
        {
            // stretch behaves like max if only width or height specified
            AssertLayout("width=400&height=200", "height=100&mode=stretch",
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
            AssertLayout("width=400&height=200", "width=200&mode=stretch",
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
            // can stretch
            AssertLayout("width=400&height=200", "height=100&width=100&mode=stretch",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(100);
                    AssertOrigin(image);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
        }

        [Test]
        public void Can_fit_max()
        {
            AssertLayout("width=400&height=200", "height=100&width=100&mode=max",
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
            AssertLayout("width=400&height=200", "height=100&mode=max",
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
            AssertLayout("width=400&height=200", "width=100&mode=max",
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
        public void Can_fit_pad()
        {
            // specifying only one values acts like max
            AssertLayout("width=400&height=200", "height=100&mode=pad",
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
            // too long
            AssertLayout("width=400&height=200", "height=200&width=200&mode=pad",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
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
            // too tall
            AssertLayout("width=200&height=400", "height=200&width=200&mode=pad",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(200);
                    image.Left.ShouldEqual(50);
                    image.Right.ShouldEqual(150);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(200);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(200);
                });
        }

        [Test]
        public void Can_fit_crop()
        {
            // specifying only one values acts like max
            AssertLayout("width=400&height=200", "height=100&mode=crop",
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
            // too long
            AssertLayout("width=400&height=200", "height=100&width=100&mode=crop",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(-50);
                    image.Right.ShouldEqual(150);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(100);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
            // too tall
            AssertLayout("width=200&height=400", "height=100&width=100&mode=crop",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(200);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(100);
                    image.Top.ShouldEqual(-50);
                    image.Bottom.ShouldEqual(150);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
        }

        #endregion

        #region Anchor

        [Test]
        public void Can_anchor_content_with_pad()
        {
            // too long
            AssertLayout("width=400&height=200", "height=200&width=200&mode=pad&anchor=topCenter",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(200);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(100);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(200);
                });
            // too long
            AssertLayout("width=400&height=200", "height=200&width=200&mode=pad&anchor=bottomCenter",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(200);
                    image.Top.ShouldEqual(100);
                    image.Bottom.ShouldEqual(200);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(200);
                });
            // too tall
            AssertLayout("width=200&height=400", "height=200&width=200&mode=pad&&anchor=middleLeft",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(200);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(100);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(200);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(200);
                });
            // too tall
            AssertLayout("width=200&height=400", "height=200&width=200&mode=pad&&anchor=middleRight",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(200);
                    image.Left.ShouldEqual(100);
                    image.Right.ShouldEqual(200);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(200);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(200);
                    canvas.Height.ShouldEqual(200);
                });
        }

        [Test]
        public void Can_anchor_content_with_crop()
        {
            // too long
            AssertLayout("width=400&height=200", "height=100&width=100&mode=crop&anchor=middleLeft",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(200);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(100);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
            // too long
            AssertLayout("width=400&height=200", "height=100&width=100&mode=crop&anchor=middleRight",
                image =>
                {
                    image.Width.ShouldEqual(200);
                    image.Height.ShouldEqual(100);
                    image.Left.ShouldEqual(-100);
                    image.Right.ShouldEqual(100);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(100);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
            // too tall
            AssertLayout("width=200&height=400", "height=100&width=100&mode=crop&anchor=topCenter",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(200);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(100);
                    image.Top.ShouldEqual(0);
                    image.Bottom.ShouldEqual(200);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
            // too tall
            AssertLayout("width=200&height=400", "height=100&width=100&mode=crop&anchor=bottomCenter",
                image =>
                {
                    image.Width.ShouldEqual(100);
                    image.Height.ShouldEqual(200);
                    image.Left.ShouldEqual(0);
                    image.Right.ShouldEqual(100);
                    image.Top.ShouldEqual(-100);
                    image.Bottom.ShouldEqual(100);
                },
                canvas =>
                {
                    canvas.Width.ShouldEqual(100);
                    canvas.Height.ShouldEqual(100);
                });
        }

        #endregion

        private void AssertLayout(string sourceSize, string resizeSettings, Action<RectangleF> assertImage, Action<SizeF> assertCanvas)
        {
            var sourceSizeSettings = new ResizeSettings(sourceSize);
            var result = _imageLayoutBuilder.BuildLayout(new Size(sourceSizeSettings.Width, sourceSizeSettings.Height), new ResizeSettings(resizeSettings));
            if (assertCanvas != null)
                assertCanvas(result.CanvasSize);
            if (assertImage != null)
                assertImage(result.Image);

            var maxWidth = (int)(Math.Max(result.Image.Width, result.CanvasSize.Width));
            var maxHeight = (int)(Math.Max(result.Image.Height, result.CanvasSize.Height));

            var padding = (int)Math.Max(Math.Abs(result.Image.Y), Math.Abs(result.Image.X)) + 20;

            if((maxWidth + padding) < 400)
            {
                padding = (400 - maxWidth) / 2;
            }

            // create a bitmap for visualizing
            var bitmapSize = new RectangleF(0,0, maxWidth + padding * 2, maxHeight + (padding * 2));
            using (var bmp = new Bitmap((int)bitmapSize.Width, (int)bitmapSize.Height))
            {
                using (var gfx = Graphics.FromImage(bmp))
                {
                    // set the background
                    gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);

                    // output the results
                    gfx.DrawString("Source: " + sourceSize, new Font("Thaoma", 8), Brushes.Black, new RectangleF(0, 0, bmp.Width, bmp.Height), new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near });
                    gfx.DrawString("Destination: " + resizeSettings, new Font("Thaoma", 8), Brushes.Black, new RectangleF(0, 0, bmp.Width, bmp.Height), new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near });
                    gfx.DrawString("Canvas: " + result.CanvasSize.Width + "x" + result.CanvasSize.Height, new Font("Thaoma", 8), Brushes.Green, new RectangleF(0, 0, bmp.Width, bmp.Height), new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far });
                    gfx.DrawString("Image: " + result.Image.Width + "x" + result.Image.Height, new Font("Thaoma", 8), Brushes.Red, new RectangleF(0,0,bmp.Width,bmp.Height), new StringFormat{Alignment= StringAlignment.Far, LineAlignment=StringAlignment.Far});
                    
                    //PolygonMath.AlignWith()
                    var canvas = new RectangleF(padding, padding, result.CanvasSize.Width, result.CanvasSize.Height);
                    var image = new RectangleF(padding + result.Image.X, padding + result.Image.Y , result.Image.Width, result.Image.Height);
                    var points = new List<PointF>();
                    points.AddRange(PolygonMath.ToPoly(canvas));
                    points.AddRange(PolygonMath.ToPoly(image));
                    points = PolygonMath.AlignWith(points.ToArray(), PolygonMath.ToPoly(bitmapSize), ContentAlignment.MiddleCenter).ToList();
                    canvas = PolygonMath.GetBoundingBox(points.Take(4).ToArray());
                    image = PolygonMath.GetBoundingBox(points.Skip(4).Take(4).ToArray());
                    gfx.FillRectangle(new SolidBrush(Color.Green), canvas);
                    gfx.DrawRectangle(new Pen(Color.Red, 2), image.X, image.Y, image.Width, image.Height);
                }
                var fileName = sourceSize + "--" + resizeSettings + ".bmp";
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);
                bmp.Save(filePath, ImageFormat.Bmp);

                Trace.WriteLine("Source:        " + sourceSize);
                Trace.WriteLine("Destination:   " + resizeSettings);
                Trace.WriteLine("   Result:     " + filePath);
            }
        }

        private void AssertOrigin(RectangleF image)
        {
            image.X.ShouldEqual(0);
            image.Y.ShouldEqual(0);
        }
    }
}
