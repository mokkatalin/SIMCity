using CityBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilderTest
{
    [TestClass]
    public class EducationalTest
    {
        private CityModel _model;
        [TestInitialize]
        public void Initialize()
        {
            _model = new CityModel();
            _model.CreateMap("EducationalTest");
        }
        
        [TestMethod]
        public void SchoolTest()
        {
            _model.Build(77, 5, FieldType.Road);
            _model.Build(77, 6, FieldType.Road);
            _model.Build(77, 7, FieldType.Road);
            _model.Build(77, 8, FieldType.Road);
            _model.Build(74, 5, FieldType.CommercialZone);
            _model.Build(74, 8, FieldType.ResidentialZone);
            _model.Build(77, 3, FieldType.School);
            _model.Build(77, 9, FieldType.University);
            Assert.IsTrue(_model.GetSchoolDiploma == 0);
            for (int i = 0; i < 5 * 365; i++)
            {
                _model.TimeAdvanced();
            }
            int firstFinished = _model.GetSchoolDiploma;
            Assert.IsTrue(firstFinished > 0);
            Assert.IsTrue(firstFinished >= _model.GetPopulation / 5);
            for (int i = 0; i < 5 * 365; i++)
            {
                _model.TimeAdvanced();
            }
            int secondFinished = _model.GetSchoolDiploma;           
            Assert.IsTrue(firstFinished < secondFinished);
        }

        [TestMethod]
        public void UniTest()
        {
            _model.Build(77, 5, FieldType.Road);
            _model.Build(77, 6, FieldType.Road);
            _model.Build(77, 7, FieldType.Road);
            _model.Build(77, 8, FieldType.Road);
            _model.Build(77, 9, FieldType.Road);
            _model.Build(77, 10, FieldType.Road);
            _model.Build(77, 11, FieldType.Road);
            _model.Build(74, 5, FieldType.CommercialZone);
            _model.Build(74, 8, FieldType.ResidentialZone);
            _model.Build(78, 5, FieldType.CommercialZone);
            _model.Build(78, 8, FieldType.ResidentialZone);
            _model.Build(77, 3, FieldType.School);
            _model.Build(77, 12, FieldType.University);
            Assert.IsTrue(_model.GetUniversityDiploma == 0);
            for (int i = 0; i < 5 * 365; i++)
            {
                _model.TimeAdvanced();
            }
            int schoolFinished = _model.GetSchoolDiploma;
            Assert.IsTrue(schoolFinished > 0);
            Assert.IsTrue(schoolFinished >= _model.GetPopulation / 5);
            for (int i = 0; i < 10 * 365; i++)
            {
                _model.TimeAdvanced();
            }
            int uniFinished = _model.GetUniversityDiploma;
            Assert.IsTrue(uniFinished > 0);
        }

        
        [TestMethod]
        public void EducationalData()
        {
            _model.Build(77, 5, FieldType.Road);
            _model.Build(77, 6, FieldType.Road);
            _model.Build(77, 7, FieldType.Road);
            _model.Build(77, 8, FieldType.Road);
            
            _model.Build(77, 3, FieldType.School);
            _model.Build(77, 9, FieldType.University);
            School s = (School)_model.GetField(77, 3);
            University u = (University)_model.GetField(77, 9);
            Assert.AreEqual(500, s.GetCapacity);
            Assert.AreEqual(1000, u.GetCapacity);
            Assert.IsTrue(s.IsDestroyable());
            Assert.IsTrue(u.IsDestroyable());
            Assert.AreEqual(0, s.GetOccupancy);
            Assert.AreEqual(0, u.GetOccupancy);

            _model.Build(74, 5, FieldType.CommercialZone);
            _model.Build(74, 8, FieldType.ResidentialZone);

            for (int i = 0; i < 5 * 365; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.AreNotEqual(0, s.GetOccupancy);
            Assert.IsFalse(s.IsDestroyable());

        }

        [TestMethod]
        public void Collapse()
        {
            _model.Build(77, 5, FieldType.Road);
            _model.Build(77, 6, FieldType.Road);
            _model.Build(77, 7, FieldType.Road);
            _model.Build(77, 8, FieldType.Road);
            _model.Build(74, 5, FieldType.CommercialZone);
            _model.Build(74, 8, FieldType.ResidentialZone);
            _model.Build(77, 3, FieldType.School);
            _model.Build(77, 9, FieldType.University);
            School s = (School)_model.GetField(77, 3);
            University u = (University)_model.GetField(77, 9);
            
            Assert.IsTrue(_model.GetSchoolDiploma == 0);
            for (int i = 0; i < 5 * 365; i++)
            {
                _model.TimeAdvanced();
            }
            Assert.IsTrue(s.GetInfo() != "");
            Assert.IsTrue(u.GetInfo() != "");
            s.Collapse();
            u.Collapse();
            Assert.AreEqual(0, s.GetOccupancy);
            Assert.AreEqual(0, u.GetOccupancy);

        }
    }
}
