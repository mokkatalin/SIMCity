using System;
using System.Reflection;
using CityBuilder.Model;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace CityBuilderTest
{
    [TestClass]
    public class ForestTests
    {
        private CityModel? _model;

        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
            _model.CreateMap("ForestTest");
        }

        [TestMethod]
        public void BasicForestTest()
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
            int noTrees = _model.GetHappiness;
            _model.Build(77, 8, FieldType.Forest);
            int withTrees = _model.GetHappiness;
            Assert.IsTrue(withTrees > noTrees);
        }
        [TestMethod]
        public void ForestTest2()
        {
            _model!.Build(80, 9, FieldType.Road);
            _model.Build(80, 10, FieldType.Road);
            _model.Build(80, 11, FieldType.Road);
            _model.Build(80, 12, FieldType.Road);
            _model.Build(80, 13, FieldType.Road);
            _model.Build(80, 14, FieldType.Road);
            _model.Build(80, 15, FieldType.Road);
            _model.Build(77, 9, FieldType.ResidentialZone);
            _model.Build(77, 12, FieldType.IndustrialZone);
            _model.Build(77, 15, FieldType.ResidentialZone);
            _model.Build(77, 8, FieldType.Forest);
            for (int i = 0; i < 40; i++)
            {
                _model.TimeAdvanced();
            }
            int withTrees = ((ResidentialZone)_model.GetField(77,9)).GetForestBonus;
            int noTrees = ((ResidentialZone)_model.GetField(77, 15)).GetBonus;
            Assert.IsTrue(withTrees > noTrees);
        }

        [TestMethod]
        public void ForestHappinessRange()
        {
            _model!.Build(80, 9, FieldType.Road);
            _model.Build(80, 10, FieldType.Road);
            _model.Build(80, 11, FieldType.Road);
            _model.Build(80, 12, FieldType.Road);
            _model.Build(80, 13, FieldType.Road);
            _model.Build(80, 14, FieldType.Road);
            _model.Build(80, 15, FieldType.Road);
            _model.Build(77, 9, FieldType.ResidentialZone);
            _model.Build(77, 12, FieldType.ResidentialZone);
            _model.Build(77, 15, FieldType.ResidentialZone);
            _model.Build(81, 9, FieldType.IndustrialZone);
            _model.Build(81, 12, FieldType.ResidentialZone);
            _model.Build(81, 15, FieldType.ResidentialZone);

            _model.Build(76, 11, FieldType.Forest);
            int nextToForest = ((ResidentialZone)_model.GetField(77, 9)).GetForestBonus;
            int closerToForest = ((ResidentialZone)_model.GetField(77, 12)).GetForestBonus;
            int farFromForest = ((ResidentialZone)_model.GetField(77, 15)).GetForestBonus;

            Assert.IsTrue(nextToForest > closerToForest);
            Assert.IsTrue(closerToForest > farFromForest);

            Assert.AreEqual(2,nextToForest);
            Assert.AreEqual(1, closerToForest);
            Assert.AreEqual(0, farFromForest);

        }

        [TestMethod]
        public void ForestAgeTest()
        {
            _model!.Build(76, 11, FieldType.Forest);
            Forest f = (Forest)_model.GetField(76, 11);
         
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, f.GetAge());
                for (int j = 0;j < 365; j++)
                {
                    _model.TimeAdvanced();
                }
                Assert.AreEqual(i + 1, f.GetAge());
            }
        }

        [TestMethod]
        public void ForestCostTest()
        {
            _model!.Build(76, 11, FieldType.Forest);
            int startB = _model.GetBalance;
            Assert.IsTrue(startB < 25000);
            Forest f = (Forest)_model.GetField(76, 11);

            for (int i = 0; i < 10 ; i++)
            {
                for (int j = 0; j < 366; j++)
                {
                    _model.TimeAdvanced();
                }

            }

            int afterTrees = _model.GetBalance;
            Assert.IsTrue(startB  > afterTrees);
            for (int j = 0; j < 3*366; j++)
            {
                _model.TimeAdvanced();
            }
            int noMoreTreeExpense = _model.GetBalance;
            Assert.IsTrue(afterTrees == noMoreTreeExpense);
        }

        [TestMethod]
        public void ForestEffectBlock()
        {
            _model!.Build(80, 9, FieldType.Road);
            _model.Build(80, 10, FieldType.Road);
            _model.Build(80, 11, FieldType.Road);
            _model.Build(80, 12, FieldType.Road);
            _model.Build(77, 9, FieldType.ResidentialZone);
            _model.Build(77, 12, FieldType.IndustrialZone);
            _model.Build(75, 8, FieldType.Road);
            _model.Build(75, 9, FieldType.Police);
            int bonus = ((ResidentialZone)_model.GetField(77, 9)).GetForestBonus;
            _model.Build(74, 9, FieldType.Forest);
            int withForest = ((ResidentialZone)_model.GetField(77, 9)).GetForestBonus;
            Assert.AreEqual(0, withForest);
            Assert.IsTrue(bonus == withForest);

        }
    }
    
}
