using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfEmployeeDirectory.Models
{
    public class Employee
    {
        private string _FirstName;
        private string _LastName;
        public int employeeID { get; set; }
        public String firstName 
        { 
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
            }
        }
        public String lastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
            }
        }
        public String startDate { get; set; }

        // include a fullName field from the model
        public String fullName
        {
            get
            {
                return _FirstName + " " + _LastName;
            }
        }
    }
}
