using System;
using System.Collections.Generic;
using System.IO;
using Qml.Net;

namespace Qml.Net.Sandbox
{
    class Program
    {
        [Signal("testSignal", NetVariantType.String)]
        public class TestQmlImport
        {
            public TestQmlImport()
            {
                Contacts = new List<Contact>
                {
                    new Contact
                    {
                        Id = 3,
                        Name = "wer"
                    },
                    new Contact
                    {
                        Id = 5,
                        Name = "we"
                    }
                };
            }

            public Contact CreateContact(int id, string name)
            {
                return new Contact
                {
                    Id = id,
                    Name = name
                };
            }
            
            public List<Contact> Contacts { get; set; }
        }

        public class Contact
        {
            public int Id { get; set; }
            
            public string Name { get; set; }
        }

        static int Main(string[] args)
        {
            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    engine.AddImportPath(Path.Combine(Directory.GetCurrentDirectory(), "Qml"));
                    
                    QQmlApplicationEngine.RegisterType<TestQmlImport>("test");

                    engine.Load("main.qml");

                    return app.Exec();
                }
            }
        }
    }
}
