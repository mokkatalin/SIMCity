using System;
using System.Reflection;
using CityBuilder.Model;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityBuilderTest
{
    [TestClass]
    public class SizeTests
    {
        private CityModel _model;
        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
            _model.CreateMap("teszt");
        }

        [TestMethod]
        public void ZoneSizeTest()
        {
            _model.Build(87, 5, FieldType.Road);
            _model.Build(87, 6, FieldType.Road);
            _model.Build(87, 7, FieldType.Road);
            _model.Build(87, 8, FieldType.Road);
            _model.Build(84, 5, FieldType.CommercialZone);
            _model.Build(84, 8, FieldType.ResidentialZone);
            _model.Build(87, 9, FieldType.IndustrialZone);

            Assert.IsTrue(_model.GetField(84, 6).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(84, 7).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(85, 5).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(85, 6).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(85, 7).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(86, 5).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(86, 6).GetType == FieldType.CommercialZone);
            Assert.IsTrue(_model.GetField(86, 7).GetType == FieldType.CommercialZone);

            Assert.IsTrue(_model.GetField(84, 9).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(84, 10).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(85, 8).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(85, 9).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(85, 10).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(86, 8).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(86, 9).GetType == FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(86, 10).GetType == FieldType.ResidentialZone);

            Assert.IsTrue(_model.GetField(87, 10).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(87, 11).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(88, 9).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(88, 10).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(88, 11).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(89, 9).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(89, 10).GetType == FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetField(89, 11).GetType == FieldType.IndustrialZone);

        }

        [TestMethod]
        public void BuildingSizeTest()
        {
            _model.Build(60, 9, FieldType.Road);
            _model.Build(60, 10, FieldType.Road);
            _model.Build(60, 11, FieldType.Road);
            _model.Build(60, 12, FieldType.Road);
            _model.Build(59, 9, FieldType.School);
            _model.Build(58, 11, FieldType.University);
            _model.Build(60, 7, FieldType.Police);
            _model.Build(60, 13, FieldType.Stadium);

            Assert.AreEqual(2, _model.GetField(59, 10).GetWidth());
            Assert.AreEqual(1, _model.GetField(59, 10).GetHeight());
            Assert.IsTrue(_model.GetField(59, 10).GetType == FieldType.School);

            Assert.AreEqual(2, _model.GetField(58, 11).GetWidth());
            Assert.AreEqual(2, _model.GetField(58, 11).GetHeight());
            Assert.IsTrue(_model.GetField(58, 12).GetType == FieldType.University);
            Assert.IsTrue(_model.GetField(59, 11).GetType == FieldType.University);
            Assert.IsTrue(_model.GetField(59, 12).GetType == FieldType.University);

            Assert.AreEqual(2, _model.GetField(60, 7).GetWidth());
            Assert.AreEqual(2, _model.GetField(60, 7).GetHeight());
            Assert.IsTrue(_model.GetField(60, 8).GetType == FieldType.Police);
            Assert.IsTrue(_model.GetField(61, 7).GetType == FieldType.Police);
            Assert.IsTrue(_model.GetField(61, 8).GetType == FieldType.Police);

            Assert.AreEqual(2, _model.GetField(60, 13).GetWidth());
            Assert.AreEqual(2, _model.GetField(60, 13).GetHeight());
            Assert.IsTrue(_model.GetField(60, 14).GetType == FieldType.Stadium);
            Assert.IsTrue(_model.GetField(61, 13).GetType == FieldType.Stadium);
            Assert.IsTrue(_model.GetField(61, 14).GetType == FieldType.Stadium);

        }
    }
}
