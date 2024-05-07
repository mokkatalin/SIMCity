using System;
using System.Reflection;
using CityBuilder.Model;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityBuilderTest
{
    [TestClass]
    public class EventTests
    {
        private CityModel _model;
        private bool eventCalled = false;
        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
        }
        [TestMethod]
        public void GameAdvancedEvent()
        {
            _model.CreateMap("EventTest");
            _model.GameAdvanced += (sender, args) => eventCalled = true;
            Assert.IsFalse(eventCalled);
            _model.Build(77, 5, FieldType.Road);
            _model.Build(77, 6, FieldType.Road);
            _model.Build(77, 7, FieldType.Road);
            _model.Build(77, 8, FieldType.Road);
            _model.Build(74, 5, FieldType.CommercialZone);
            _model.Build(74, 8, FieldType.ResidentialZone);
            _model.Build(77, 3, FieldType.School);
            _model.TimeAdvanced();
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void GameOverEvent()
        {
            _model.CreateMap("EventTest");
            _model.GameOver += (sender, args) => eventCalled = true;
            _model.Build(87, 5, FieldType.Road);
            _model.Build(87, 6, FieldType.Road);
            _model.Build(87, 7, FieldType.Road);
            _model.Build(87, 8, FieldType.Road);
            _model.Build(84, 5, FieldType.IndustrialZone);
            _model.Build(84, 8, FieldType.ResidentialZone);
            Assert.IsFalse(eventCalled);
            _model.SetTax(100);
            _model.TimeAdvanced();
            int i = 0;
            while(i < 5* 365 || !eventCalled)
            {
                _model.TimeAdvanced();
                i++;
            }
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void IncomeEvent()
        {
            _model.CreateMap("EventTest");
            _model.IncomeChanged += (sender, args) => eventCalled = true;
            _model.Build(87, 5, FieldType.Road);
            _model.Build(87, 6, FieldType.Road);
            _model.Build(87, 7, FieldType.Road);
            _model.Build(87, 8, FieldType.Road);
            _model.Build(84, 5, FieldType.IndustrialZone);
            _model.Build(84, 8, FieldType.ResidentialZone);
            Assert.IsFalse(eventCalled);
            for (int i = 0; i < 365; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void ExpenseEvent()
        {
            _model.CreateMap("EventTest");
            _model.ExpenseChanged += (sender, args) => eventCalled = true;
            Assert.IsFalse(eventCalled);
            _model.Build(87, 5, FieldType.Road);
            _model.Build(87, 6, FieldType.Road);
            _model.Build(87, 7, FieldType.Road);
            _model.Build(87, 8, FieldType.Road);
            _model.Build(84, 5, FieldType.IndustrialZone);
            _model.Build(84, 8, FieldType.ResidentialZone);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void CatastropheEvent()
        {
            _model.CreateMap("EventTest");
            _model.CatastropheChanged += (sender, args) => eventCalled = true;
            _model.Build(57, 5, FieldType.Road);
            _model.Build(57, 6, FieldType.Road);
            _model.Build(57, 7, FieldType.Road);
            _model.Build(57, 8, FieldType.Road);
            _model.Build(54, 5, FieldType.CommercialZone);
            _model.Build(54, 8, FieldType.ResidentialZone);
            _model.Build(57, 9, FieldType.IndustrialZone);
            Assert.IsFalse(eventCalled);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            _model.Catastrophe(54, 8);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void GameCreatedEvent()
        {
            _model.GameCreated += (sender, args) => eventCalled = true;
            Assert.IsFalse(eventCalled);
            _model.CreateMap("EventTest");
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void FieldChangedEvent()
        {
            _model.CreateMap("EventTest");
            _model.FieldChanged += (sender, args) => eventCalled = true;
            Assert.IsFalse(eventCalled);
            _model.Build(57, 7, FieldType.Road);
            Assert.IsTrue(eventCalled);
        }

        [TestMethod]
        public void FieldChangedEvent2()
        {
            _model.CreateMap("EventTest");
            _model.FieldChanged += (sender, args) => eventCalled = true;
            Assert.IsFalse(eventCalled);
            _model.Build(57, 7, FieldType.Road);
            eventCalled = false;
            _model.UnBuild(57, 7);
            Assert.IsTrue(eventCalled);
        }
    }
}
