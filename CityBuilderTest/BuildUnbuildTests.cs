using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityBuilder.Model;

namespace CityBuilderTest
{
    [TestClass]
    public class BuildUnBuildTests
    {
        private CityModel? _model;
        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
            _model.CreateMap("BuildUnBuildTests");
        }

        [TestMethod]
        public void BuildTest1()
        {
            Assert.ThrowsException<Exception>(() => _model.Build(55, 5, FieldType.ResidentialZone));
            _model.Build(55, 5, FieldType.Road);
            _model.Build(55, 6, FieldType.Road);
            _model.Build(55, 7, FieldType.ResidentialZone);
            Assert.IsTrue(_model.GetField(55, 7).GetType == FieldType.ResidentialZone);
            Assert.ThrowsException<Exception>(() => _model.Build(97, 8, FieldType.Stadium));
            Assert.ThrowsException<Exception>(() => _model.Build(90, 20, FieldType.IndustrialZone));
            Assert.ThrowsException<Exception>(() => _model.Build(92, 42, FieldType.CommercialZone));
            Assert.ThrowsException<Exception>(() => _model.Build(90, 12, FieldType.Police));
        }

        [TestMethod]
        public void BuildTest2()
        {
            _model.Build(5, 5, FieldType.Road);
            Assert.ThrowsException<Exception>(() => _model.Build(5, 5, FieldType.Road));
            _model.Build(5, 6, FieldType.Road);
            Assert.ThrowsException<Exception>(() => _model.Build(5, 5, FieldType.CommercialZone));
            Assert.ThrowsException<Exception>(() => _model.Build(5, 6, FieldType.IndustrialZone));
            _model.Build(5, 7, FieldType.Road);
            Assert.ThrowsException<Exception>(() => _model.Build(5, 6, FieldType.Police));
            Assert.ThrowsException<Exception>(() => _model.Build(5, 7, FieldType.University));

        }
        [TestMethod]
        public void BuildingOnRightEdge()
        {
            _model!.Build(50,99,FieldType.Road);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _model.Build(51, 98, FieldType.ResidentialZone));
            Assert.ThrowsException<IndexOutOfRangeException>(() => _model.Build(51, 99, FieldType.ResidentialZone));
            Assert.ThrowsException<IndexOutOfRangeException>(() => _model.Build(51, 100, FieldType.ResidentialZone));
        }

        [TestMethod]
        public void BuildingOnDownEdge()
        {
            _model!.Build(99, 42, FieldType.Road);
            Assert.ThrowsException<IndexOutOfRangeException>(() => _model.Build(99, 43, FieldType.ResidentialZone));
            Assert.ThrowsException<IndexOutOfRangeException>(() => _model.Build(98, 43, FieldType.ResidentialZone));
            Assert.ThrowsException<IndexOutOfRangeException>(() => _model.Build(100, 43, FieldType.ResidentialZone));
        }


        [TestMethod]
        public void UnBuildTest1()
        {
            _model.Build(72, 5, FieldType.Road);
            _model.Build(72, 6, FieldType.Road);
            _model.Build(72, 7, FieldType.Road);
            _model.Build(72, 8, FieldType.Road);
            _model.Build(73, 8, FieldType.ResidentialZone);
            for (int i = 0; i < 10; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.IsTrue(_model.GetField(72, 5).IsDestroyable());
            Assert.IsTrue(_model.GetField(72, 6).IsDestroyable());
            Assert.IsTrue(_model.GetField(72, 7).IsDestroyable());
            Assert.IsTrue(_model.GetField(72, 8).IsDestroyable());
            Assert.IsTrue(_model.GetField(73, 8).IsDestroyable());
            _model.UnBuild(72, 5);
            _model.UnBuild(72, 6);
            _model.UnBuild(72, 7);
            _model.UnBuild(72, 8);
            _model.UnBuild(73, 8);

            Assert.IsTrue(_model.GetField(72, 5).GetType == FieldType.Empty);
            Assert.IsTrue(_model.GetField(72, 6).GetType == FieldType.Empty);
            Assert.IsTrue(_model.GetField(72, 7).GetType == FieldType.Empty);
            Assert.IsTrue(_model.GetField(72, 8).GetType == FieldType.Empty);
            Assert.IsTrue(_model.GetField(73, 8).GetType == FieldType.Empty);
        }

        [TestMethod]
        public void UnbuildTest2()
        {
            _model.Build(67, 5, FieldType.Road);
            _model.Build(67, 6, FieldType.Road);
            _model.Build(67, 7, FieldType.Road);
            _model.Build(67, 8, FieldType.Road);
            _model.Build(64, 5, FieldType.CommercialZone);
            _model.Build(64, 8, FieldType.ResidentialZone);
            _model.Build(67, 9, FieldType.IndustrialZone);
            _model.Build(67, 13, FieldType.Forest);
            for (int i = 0; i < 10; i++)
            {
                _model.TimeAdvanced();
            }

            Assert.IsFalse(_model.GetField(64, 5).IsDestroyable());
            Assert.IsFalse(_model.GetField(64, 8).IsDestroyable());
            Assert.IsFalse(_model.GetField(67, 9).IsDestroyable());
            Assert.IsFalse(_model.GetField(67, 13).IsDestroyable());

            Assert.ThrowsException<Exception>(() => _model.UnBuild(67, 13));
            Assert.ThrowsException<Exception>(() => _model.UnBuild(64, 5));
            Assert.ThrowsException<Exception>(() => _model.UnBuild(64, 8));
            Assert.ThrowsException<Exception>(() => _model.UnBuild(67, 9));
        }
    }
}