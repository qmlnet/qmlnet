using Xunit;

namespace Qt.NetCore.Tests.Interop
{
    public class StringInteropTests
    {
        [Fact]
        public void Can_use_reference()
        {
            using(var interop = new NetTestStringInterop())
            {
                Assert.Null(interop.GetStringReference());

                interop.SetStringReference(null);
                Assert.Null(interop.GetStringReference());

                interop.SetStringReference("");
                Assert.Equal(interop.GetStringReference(), "");

                interop.SetStringReference("TEΏST");
                Assert.Equal(interop.GetStringReference(), "TEΏST");

                interop.SetStringReference(null);
                Assert.Null(interop.GetStringReference());
            }
        }

        [Fact]
        public void Can_use_value()
        {
            using (var interop = new Qt.NetCore.NetTestStringInterop())
            {
                Assert.Null(interop.GetStringValue());

                interop.SetStringValue(null);
                Assert.Null(interop.GetStringValue());

                interop.SetStringValue("");
                Assert.Equal(interop.GetStringValue(), "");

                interop.SetStringValue("TEΏST");
                Assert.Equal(interop.GetStringValue(), "TEΏST");

                interop.SetStringValue(null);
                Assert.Null(interop.GetStringValue());
            }
        }

        [Fact]
        public void Can_use_pointer()
        {
            using (var interop = new NetTestStringInterop())
            {
                Assert.Null(interop.GetStringPointer());

                interop.SetStringPointer(null);
                Assert.Null(interop.GetStringPointer());

                interop.SetStringPointer("");
                Assert.Equal(interop.GetStringPointer(), "");

                interop.SetStringPointer("TEΏST");
                Assert.Equal(interop.GetStringPointer(), "TEΏST");

                interop.SetStringPointer(null);
                Assert.Null(interop.GetStringPointer());
            }
        }
    }
}