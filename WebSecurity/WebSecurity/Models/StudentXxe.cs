using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebSecurity.Models
{
    [Serializable]
    [XmlRoot(ElementName = "student")]
    public class StudentXxe
    {
        public StudentXxe()
        {
        }

        [XmlElement(ElementName = "firstname")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "lastname")]
        public string LastName { get; set; }
    }
}