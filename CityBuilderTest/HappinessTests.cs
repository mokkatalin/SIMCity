using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityBuilder.Model;

namespace CityBuilderTest
{
    [TestClass]
    public class HappinessTests
    {
        private CityModel? _model;
        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
            _model.CreateMap("HappinessTests");
        }

        [TestMethod]
        public void HappinessChangebyPolice()
        {
            _model!.Build(80, 9, FieldType.Road);
            _model.Build(80, 10, FieldType.Road);
            _model.Build(80, 11, FieldType.Road);
            _model.Build(80, 12, FieldType.Road);
            _model.Build(77, 9, FieldType.ResidentialZone);
            _model.Build(77, 12, FieldType.IndustrialZone);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.IsTrue(_model.GetPopulation == 20);
            int withoutPolice = _model.GetHappiness;
            _model.Build(80, 7, FieldType.Police);
            int withPolice = _model.GetHappiness;
            Assert.IsTrue(withPolice > withoutPolice);
            _model.UnBuild(80, 7);
            withoutPolice = _model.GetHappiness;
            Assert.IsTrue(withoutPolice < withPolice);
        }

        [TestMethod]
        public void HappinessChangeByIndustrial()
        {
            _model!.Build(87, 5, FieldType.Road);
            _model.Build(87, 6, FieldType.Road);
            _model.Build(87, 7, FieldType.Road);
            _model.Build(87, 8, FieldType.Road);
            _model.Build(84, 5, FieldType.CommercialZone);
            _model.Build(84, 8, FieldType.ResidentialZone);
            for (int i = 0; i < 10; i++)
            {
                _model.TimeAdvanced();
            }
            int withoutInd = _model.GetHappiness;
            _model.Build(87, 9, FieldType.IndustrialZone);
            int withInd = _model.GetHappiness;
            Assert.IsTrue(withoutInd > withInd);
            _model.UnBuild(87, 9);
            withoutInd = _model.GetHappiness;
            Assert.IsTrue(withoutInd > withInd);
        }

        [TestMethod]
        public void HappinessOfFootballLovers()
        {
            _model!.Build(80, 9, FieldType.Road);
            _model.Build(80, 10, FieldType.Road);
            _model.Build(80, 11, FieldType.Road);
            _model.Build(80, 12, FieldType.Road);
            _model.Build(77, 9, FieldType.ResidentialZone);
            _model.Build(77, 12, FieldType.IndustrialZone);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.IsTrue(_model.GetPopulation == 20);
            int noFootball = _model.GetHappiness;
            _model.Build(80, 7, FieldType.Stadium);
            int withStadium = _model.GetHappiness;
            Assert.IsTrue(withStadium > noFootball);
            _model.UnBuild(80, 7);
            noFootball = _model.GetHappiness;
            Assert.IsTrue(withStadium > noFootball);
        }
    }
}
