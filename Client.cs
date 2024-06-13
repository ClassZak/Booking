using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
    public class Client
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FatherName { get; set; }

        public Client() { }
        public Client(string name, string surname, string fatherName)
        {
            Name = name;
            Surname = surname;
            FatherName = fatherName;
        }
    }
}
