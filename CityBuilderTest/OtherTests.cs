using System;
using System.Reflection;
using CityBuilder.Model;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

    
namespace CityBuilderTest
{
    [TestClass]
    public class OtherTests
    {
        private CityModel _model;
              
        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
            _model.CreateMap("teszt");            
        }

        [TestMethod]
        public void CheckStarterDataTest()
        {
            Assert.AreEqual("teszt", _model.GetCityName);
            Assert.AreEqual(0, _model.GetPopulation);
            Assert.AreEqual(0,_model.GetIncomes.Count);
            Assert.AreEqual(0,_model.GetExpenses.Count);
            Assert.IsTrue(_model.GetTax > 0);
            Assert.AreEqual(0,_model.GetTime);
            Assert.AreEqual(25000,_model.GetBalance);
            Assert.IsTrue(_model.GetHappiness / 5 > 0);
            Assert.IsTrue(_model.GetHappiness / 5 <= 100);
            Assert.AreEqual(0,_model.GetPopulation);
        }

        [TestMethod]
        public void PeopleMovingInTest()
        {
            Assert.IsTrue(_model.GetPopulation == 0);
            _model.Build(87, 5, FieldType.Road);
            _model.Build(87, 6, FieldType.Road);
            _model.Build(87, 7, FieldType.Road);
            _model.Build(87, 8, FieldType.Road);
            _model.Build(84, 5, FieldType.CommercialZone);
            _model.Build(84, 8, FieldType.ResidentialZone);
            _model.Build(87, 9, FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetPopulation == 0);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.AreEqual(20,_model.GetPopulation);
            
            Assert.IsTrue(_model.GetBalance < 25000);
            Assert.IsTrue(_model.GetHappiness < 250);
        }
             
        
        [TestMethod]
        public void ZoneUpgradeTest()
        {
            _model.Build(12, 12, FieldType.Road);
            _model.Build(12, 13, FieldType.Road);
            _model.Build(12, 14, FieldType.Road);
            _model.Build(12, 15, FieldType.Road);
            _model.Build(9, 15, FieldType.ResidentialZone);
            
            Zone zR = (Zone)_model.GetField(9, 15);
            Assert.AreEqual(1, zR.GetCurrentLevel);
            Assert.AreEqual(0,zR.GetOccupancy);
            _model.Build(12, 9, FieldType.IndustrialZone);
            Zone zI = (Zone)_model.GetField(12, 9);
            for (int i = 0; i < 10; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.AreEqual(20, zR.GetOccupancy);
            zI.Upgrade();
            Assert.AreEqual(2, zI.GetCurrentLevel);
            zR.Upgrade();
            Assert.AreEqual(2, zR.GetCurrentLevel);
            Assert.AreEqual(20, zR.GetOccupancy);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.AreEqual(50, zR.GetOccupancy);
            zI.Upgrade();
            Assert.AreEqual(3, zI.GetCurrentLevel);
            zR.Upgrade();
            Assert.AreEqual(3, zR.GetCurrentLevel);
            Assert.AreEqual(50, zR.GetOccupancy);
            for (int i = 0; i < 50; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.AreEqual(100, zR.GetOccupancy);
        }

        [TestMethod]
        public void TaxIncomeTest()
        {
            _model.Build(12, 12, FieldType.Road);
            _model.Build(12, 13, FieldType.Road);
            _model.Build(12, 14, FieldType.Road);
            _model.Build(12, 15, FieldType.Road);
            _model.Build(9, 15, FieldType.ResidentialZone);
            _model.Build(12, 9, FieldType.IndustrialZone);
            for (int i = 0; i < 366; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.IsTrue(_model.GetIncomes.Count > 0);
        }

        [TestMethod]
        public void BuildingExpenseTest()
        {
            _model.Build(12, 12, FieldType.Road);
            _model.Build(12, 13, FieldType.Road);
            _model.Build(12, 14, FieldType.Road);
            _model.Build(9, 14, FieldType.ResidentialZone);
            _model.Build(12, 9, FieldType.IndustrialZone);
            _model.TimeAdvanced();
            Assert.AreEqual(5, _model.GetExpenses.Count);
            Assert.IsTrue(_model.GetBalance < 25000);
        }

        [TestMethod]
        public void CatastropheTest1()
        {
            Assert.IsTrue(_model.GetPopulation == 0);
            _model.Build(57, 5, FieldType.Road);
            _model.Build(57, 6, FieldType.Road);
            _model.Build(57, 7, FieldType.Road);
            _model.Build(57, 8, FieldType.Road);
            _model.Build(54, 5, FieldType.CommercialZone);
            _model.Build(54, 8, FieldType.ResidentialZone);
            _model.Build(57, 9, FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetPopulation == 0);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            Console.WriteLine(_model.GetHappiness);
            Assert.IsTrue(_model.GetPopulation == 20);
            _model.Catastrophe(54, 8);
            Assert.IsTrue(_model.GetPopulation == 0);
            Assert.IsTrue(_model.GetField(54, 8).IsDemolished);
        }
        [TestMethod]
        public void CatastropheTest2()
        {
            _model.Build(57, 5, FieldType.Road);
            _model.Build(57, 6, FieldType.Road);
            _model.Build(57, 7, FieldType.Road);
            _model.Build(57, 8, FieldType.Road);
            _model.Build(54, 5, FieldType.CommercialZone);
            _model.Build(54, 8, FieldType.ResidentialZone);
            _model.Build(57, 9, FieldType.IndustrialZone);
            Assert.IsTrue(_model.GetPopulation == 0);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            int beforeH = _model.GetHappiness;
            int beforeP = _model.GetPopulation;
            _model.Catastrophe(54, 8);
            _model.TimeAdvanced();
            int afterH = _model.GetHappiness;
            int afterP = _model.GetPopulation;
            Assert.IsTrue(beforeH > afterH);
            Assert.IsTrue(beforeP > afterP);
            Assert.IsTrue(_model.GetField(54, 8).IsDemolished);
        }

        [TestMethod]
        public void InfoTest()
        {
            _model.Build(57, 5, FieldType.Road);
            _model.Build(57, 6, FieldType.Road);
            _model.Build(57, 7, FieldType.Road);
            _model.Build(57, 8, FieldType.Road);
            _model.Build(54, 5, FieldType.CommercialZone);
            _model.Build(54, 8, FieldType.ResidentialZone);
            _model.Build(57, 9, FieldType.IndustrialZone);
            _model.Build(57, 4, FieldType.Forest);

            Assert.IsTrue(_model.GetField(57,5).GetInfo() == "");
            Assert.IsTrue(_model.GetField(57, 6).GetInfo() == "");
            Assert.IsTrue(_model.GetField(57, 7).GetInfo() == "");
            Assert.IsFalse(_model.GetField(54, 8).GetInfo() == "");
            Assert.IsFalse(_model.GetField(54, 5).GetInfo() == "");
            Assert.IsFalse(_model.GetField(57,9).GetInfo() == "");
            Assert.IsFalse(_model.GetField(57, 4).GetInfo() == "");
            Assert.IsFalse(_model.GetField(57, 9).GetInfo() == "");
            Assert.IsFalse(_model.GetField(57, 4).GetInfo() == "");


        }
        
       
    }
}