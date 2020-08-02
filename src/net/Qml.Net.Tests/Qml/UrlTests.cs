using Moq;
using Qml.Net.Internal.Qml;
using System.Diagnostics;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class UrlTests : BaseQmlTests<UrlTests.UrlTestsQml>
    {
        public class UrlTestsQml
        {
            public virtual string Property { get; set; }

            public virtual void MethodParameter(string value)
            {
            }

            public virtual string MethodReturn()
            {
                return "relative/url";
            }
        }

        [Fact]
        public void Can_read_write_relative_url_value()
        {
            Mock.Setup(x => x.Property).Returns("some/relative/url");

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "some/relative/url", Times.Once);
        }

        [Fact]
        public void Can_convert_to_and_from_QUrl()
        {
            var imageUrl =
                "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
            Mock.Setup(x => x.Property).Returns(imageUrl);

            RunQmlTest(
                "test",
                @"
                    const img = Qt.createQmlObject('import QtQuick 2.0; Image {}', test, 'dummyImg');
                    img.source = test.property; // Write string->QUrl property
                    test.methodParameter(img.source); // Pass through QUrl property to our string property
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<string>(y => y == imageUrl)), Times.Once);
        }

        [Fact]
        public void Can_call_method_with_return()
        {
            Mock.Setup(x => x.MethodReturn()).Returns("some/relative/url");

            RunQmlTest(
                "test",
                @"
                    test.methodParameter(test.methodReturn())
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<string>(y => y == "some/relative/url")), Times.Once);
        }
    }
}