using System;
using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class QQmlApplicationEngineTests : BaseQmlTests<QQmlApplicationEngineTests.QQmlApplicationEngineQml>
    {
        public class QQmlApplicationEngineQml
        {
            public Guid Guid { get; set; }
        }

        [Fact]
        public void Can_set_context_property()
        {
            var propName = Guid.NewGuid().ToString().Replace("-", "");
            qmlApplicationEngine.GetContextProperty(propName).Should().BeNull();
            qmlApplicationEngine.SetContextProperty(propName, 2);
            qmlApplicationEngine.GetContextProperty(propName).Should().Be(2);
            qmlApplicationEngine.SetContextProperty(propName, null);
            qmlApplicationEngine.GetContextProperty(propName).Should().BeNull();
            var o = new QQmlApplicationEngineQml();
            o.Guid = Guid.NewGuid();
            qmlApplicationEngine.SetContextProperty(propName, o);
            ((QQmlApplicationEngineQml) qmlApplicationEngine.GetContextProperty(propName)).Guid.Should().Be(o.Guid);
        }
    }
}