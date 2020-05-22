using System;
using System.Drawing;
using FluentAssertions;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class ColorTests : BaseQmlTests<ColorTests.ColorTestsQml>
    {
        public class ColorTestsQml
        {
            public Color Value { get; set; }

            public Color NullColor => Color.Empty;

            public bool CheckResult { get; set; }

            public int A { get; set; }

            public int R { get; set; }

            public int G { get; set; }

            public int B { get; set; }
        }

        [Fact]
        public void Converts_empty_dotnet_color_to_null()
        {
            RunQmlTest(
                "test",
                @"
                    test.checkResult = test.nullColor === null;
                ");

            Mock.Object.CheckResult.Should().BeTrue();
        }

        [Fact]
        public void Converts_null_to_empty_dotnet_color()
        {
            Mock.Object.Value = Color.Red;

            RunQmlTest(
                "test",
                @"
                    test.value = null;
                ");

            Mock.Object.Value.Should().Be(Color.Empty);
        }

        [Fact]
        public void Converts_predefined_colors_to_rgba()
        {
            Mock.Object.Value = Color.MediumAquamarine;

            RunQmlTest(
                "test",
                @"
                    const {a, r, g, b} = test.value;
                    test.a = a * 255;
                    test.r = r * 255;
                    test.g = g * 255;
                    test.b = b * 255;
                ");

            Mock.Object.A.Should().Be(Color.MediumAquamarine.A);
            Mock.Object.R.Should().Be(Color.MediumAquamarine.R);
            Mock.Object.G.Should().Be(Color.MediumAquamarine.G);
            Mock.Object.B.Should().Be(Color.MediumAquamarine.B);
        }

        [Fact]
        public void Converts_custom_colors_to_rgba()
        {
            Mock.Object.Value = Color.FromArgb(100, 150, 200, 250);

            RunQmlTest(
                "test",
                @"
                    const {a, r, g, b} = test.value;
                    test.a = a * 255;
                    test.r = r * 255;
                    test.g = g * 255;
                    test.b = b * 255;
                ");

            Mock.Object.A.Should().Be(100);
            Mock.Object.R.Should().Be(150);
            Mock.Object.G.Should().Be(200);
            Mock.Object.B.Should().Be(250);
        }

        [Fact]
        public void Converted_color_is_comparable_to_Qt_color()
        {
            Mock.Object.Value = Color.FromArgb(100, 150, 200, 250);

            RunQmlTest(
                "test",
                @"
                    if (test.value !== Qt.rgba(150 / 255, 200 / 255, 250 / 255, 100 / 255)) {
                        throw new Error('Converted color should be comparable to color, but got: ' + test.value);
                    }
                ");
        }

        [Fact]
        public void Converts_qml_color_to_dotnet_color()
        {
            Mock.Object.Value = Color.Empty;

            RunQmlTest(
                "test",
                @"
                    test.value = Qt.rgba(150 / 255.0, 200 / 255.0, 250 / 255.0, 100 / 255.0);
                ");

            Mock.Object.Value.Should().Be(Color.FromArgb(100, 150, 200, 250));
        }
    }
}