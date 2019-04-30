using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Instructions;
using LoadFileAdapter;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class Sandbox
    {
        [TestMethod]
        public void Test()
        {
            String one = "blah blah blah";
            String two = one;
            one = "tuti fruti";
            //Assert.AreEqual(one, two);
            Int32 j = 1;
            Int32 k = j;
            j++;
            //Assert.AreEqual(j, k);
        }

        [TestMethod]
        public void SandboxTest()
        {
            string xml = System.IO.File.ReadAllText("X:\\dev\\TestData\\DAT_Edit.xml");
            Job job = Job.Deserialize(xml);
            job.Imports.First().File = new System.IO.FileInfo(@"X:\dev\TestData\SAMPLE.DAT");
            job.Exports.First().File = new System.IO.FileInfo(@"X:\dev\TestData\OUTPUT.DAT");
            job.Execute();            
            xml = System.IO.File.ReadAllText(@"X:\dev\TestData\XREF_TEST.xml");
            job = Job.Deserialize(xml);

        }


        abstract class Widget { }
        abstract class Settings { }
        class WidgetX : Widget { public WidgetX(SettingsX settingsX) { } }
        class WidgetY : Widget { public WidgetY(SettingsY settingsY) { } }
        class SettingsX : Settings { }
        class SettingsY : Settings { }

        class WidgetFactory
        {
            public static WidgetX CreateWidgetX(SettingsX settings)
            {
                return new WidgetX(settings);
            }

            public static WidgetY CreateWidgetY(SettingsY settings)
            {
                return new WidgetY(settings);
            }

            public static Widget CreateWidget(Settings settings)
            {
                Dictionary<Type, Func<Settings, Widget>> map = new Dictionary<Type, Func<Settings, Widget>>();
                map.Add(typeof(SettingsX), (s) => CreateWidgetX((SettingsX)s));
                map.Add(typeof(SettingsY), (s) => CreateWidgetY((SettingsY)s));
                Widget widget = map[settings.GetType()].Invoke(settings);
                return widget;
            }
        }
        
        Widget getWidget(Settings settings)
        {
            Widget widget = null;

            if (settings.GetType().Equals(typeof(SettingsX)))
                widget = new WidgetX((SettingsX)settings);
            else if (settings.GetType().Equals(typeof(SettingsY)))
                widget = new WidgetY((SettingsY)settings);

            return widget;
        }
                
        [TestMethod]
        public void test()
        {
            Automobile.AutomobileBuilder carBuilder = new Automobile.AutomobileBuilder();
            Vehicle car = carBuilder.setName("car").setAxels(2).setTireSize(16).Build();
            Boat.BoatBuilder boatBuilder = new Boat.BoatBuilder();
            Vehicle boat = boatBuilder.setName("boat").setHullType("blah").setPropType("meh").Build();
            Assert.AreEqual("car", car.getName());
            Assert.AreEqual(2, ((Automobile)car).getAxels());
            Assert.AreEqual(16, ((Automobile)car).getTireSize());
            Assert.AreEqual("boat", boat.getName());
            Assert.AreEqual("blah", ((Boat)boat).getHullType());
            Assert.AreEqual("meh", ((Boat)boat).getPropType());
        } 

        abstract class Vehicle
        {
            private readonly string name;

            public Vehicle(VehicleBaseBuilder builder)
            {                
                this.name = builder.getName();
            }
            public string getName() { return name; }
        }

        abstract class VehicleBaseBuilder
        {
            protected string name;
            public string getName() { return name; }
        }

        abstract class VehicleBuilder<T> : VehicleBaseBuilder where T : VehicleBuilder<T>
        {
            public T setName(string value)
            {
                name = value;
                return (T)this;
            }
            public abstract Vehicle Build();
        }

        class Automobile : Vehicle
        {
            private readonly int axels;
            private readonly int tireSize;

            private Automobile(AutomobileBuilder builder) : base(builder)
            {
                
                this.axels = builder.getAxels();
                this.tireSize = builder.getTireSize();
            }
                        
            public int getAxels() { return axels; }
            public int getTireSize() { return tireSize; }

            public class AutomobileBuilder : VehicleBuilder<AutomobileBuilder>
            {
                private int axels;
                private int tireSize;
                                
                public int getAxels() { return axels; }
                public int getTireSize() { return tireSize; }
                
                public AutomobileBuilder setAxels(int count)
                {
                    axels = count;
                    return this;
                }
                
                public AutomobileBuilder setTireSize(int count)
                {
                    tireSize = count;
                    return this;
                }
                public override Vehicle Build()
                {
                    return new Automobile(this);
                }
            }
        }

        class Boat : Vehicle
        {            
            private readonly string hullType;
            private readonly string propType;

            private Boat(BoatBuilder builder) : base(builder)
            {
                this.hullType = builder.getHullType();
                this.propType = builder.getPropType();
            }
                        
            public string getHullType() { return hullType; }
            public string getPropType() { return propType; }

            public class BoatBuilder : VehicleBuilder<BoatBuilder>
            {                
                private string hullType;
                private string propType;
                
                public string getHullType() { return hullType; }
                public string getPropType() { return propType; }
                
                public BoatBuilder setHullType(string value)
                {
                    hullType = value;
                    return this;
                }

                public BoatBuilder setPropType(string value)
                {
                    propType = value;
                    return this;
                }

                public override Vehicle Build()
                {
                    return new Boat(this);
                }
            }
        }

        
        
    }
}
