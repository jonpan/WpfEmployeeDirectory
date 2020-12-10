using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfEmployeeDirectory.Commands;
using WpfEmployeeDirectory.Views;
using WpfEmployeeDirectory.Models;
using System.Windows;

namespace WpfEmployeeDirectory.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Add and edit employee data, ultimately one screen has a grid of all employees to select an employee the edit
        /// Add button goes to an empty edit screen to allow entry of new employee data
        /// </summary>
        static String dbConnectionString = "Data Source=DELL-FMQG6PQP5D\\SQLEXPRESS;Initial Catalog=EmployeeDB;Integrated Security=True";
        SqlConnection conn;
        SqlCommand sqlCommand;
        SqlDataAdapter sqlAdapter;
        DataSet employeeDS;

        // variable to store a item to go to in the datagrid
        private int currEmployeeIndex;
        // variable to temp store the current item in the grid.
        private int _selectedItem;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;
        private bool bCanUpdate;
        private bool bCanCancel;

        // bound variables to views
        public ObservableCollection<Employee> employees { get; set; }
        public string txtSelectedEmployee { get; set; }
        public string txtFirstName { get; set; }
        public string txtLastName { get; set; }
        public string txtStartDate { get; set; }
        //public Employee SelectedItem.
        public int SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
               _selectedItem = value;
                SelectedEmployee();
            }
        }
 

        // constructor
        public EmployeeViewModel()
        {

            // initialize button enable logic
            bCanDelete = false;
            bCanAdd = true;
            bCanEdit = false;
            bCanUpdate = false;
            bCanCancel = false;

            // initialize control commands
            AddEmployee = new AddEmployeeCommand(this);
            EditEmployee = new EditEmployeeCommand(this);
            UpdateEmployee = new UpdateEmployeeCommand(this);
            DeleteEmployee = new DeleteEmployeeCommand(this);
            CancelChanges = new CancelChangesCommand(this);
            
            // retrieve employee list and populate DataGrid control            
            FillDataGrid();

        }

 
        /// <summary>
        /// Logic to ensure commands can be executed
        /// </summary>
        public bool CanAddEmployee
        {
            get
            {
                return bCanAdd;
            }
            set { }
        }
        public bool CanEditEmployee
        {
            get
            {
                return bCanEdit;
            }
            set { }
        }
        public bool CanUpdateEmployee
        {
            get
            {
                return bCanUpdate;
            }
            set { }
        }
        public bool CanDeleteEmployee
        {
            get
            {
                return bCanDelete;
            }
            set { }
        }
        public bool CanCancelChanges
        {
            get
            {
                return bCanCancel;
            }
            set
            {

            }
        }

        /// <summary>
        /// actual functions that manipulate data
        /// </summary>
        public void FillDataGrid()
        {
            try
            {
                conn = new SqlConnection(dbConnectionString);
                conn.Open();
                sqlCommand = new SqlCommand("select * from dbo.EmployeeInfo", conn);
                sqlAdapter = new SqlDataAdapter(sqlCommand);
                employeeDS = new DataSet();
                sqlAdapter.Fill(employeeDS, "dbo.EmployeeInfo");

                if (employees == null)
                {
                    employees = new ObservableCollection<Employee>();
                }
                employees.Clear();
                foreach (DataRow row in employeeDS.Tables[0].Rows)
                {
                    employees.Add(new Employee
                    {
                        employeeID = Convert.ToInt32(row[0].ToString()),
                        firstName = row[1].ToString(),
                        lastName = row[2].ToString(),
                        startDate = (Convert.ToDateTime(row[3].ToString()).ToShortDateString())
                    });
                }

                // SelectedItem is triggered/reset after the DataGrid is reloaded, set selected item to the edited or added item after reload
                // -Improvement- scroll the DataGrid to the Selected item
                SelectedItem = currEmployeeIndex;
                // trigger relocation to either the edited employee or the added one
                if(SelectedItem >= 0 && SelectedItem < employees.Count)
                {
                    OnPropertyChanged("SelectedItem");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                employeeDS = null;
                sqlAdapter.Dispose();
                conn.Close();
                conn.Dispose();
            }
            // load the edit fields with blank to show inactive
            ClearTextFields("-none-");
        }
        public void InsertEmployee()
        {
            //set button logic
            bCanCancel = true;
            bCanUpdate = true;
            bCanAdd = true;
            bCanEdit = false;
            bCanDelete = false;

            //set field values, blank, adding a new employee
            ClearTextFields("");
        }
        public void ModifyEmployee()
        {
            // set button logic
            bCanCancel = true;
            bCanUpdate = true;
            bCanAdd = false;
            bCanEdit = true;
            bCanDelete = false;

            //load edit fields with current employee details
            txtFirstName = employees[SelectedItem].firstName;
            txtLastName = employees[SelectedItem].lastName;
            txtStartDate = employees[SelectedItem].startDate;
            OnPropertyChanged("txtFirstName");
            OnPropertyChanged("txtlastName");
            OnPropertyChanged("txtStartDate");
 
        }
        public void SaveEmployee()
        {
            try
            {
                if (bCanAdd)
                {
                    conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    sqlCommand = new SqlCommand("insert into dbo.EmployeeInfo (firstName, lastName, startDate) values (@fn, @ln, @sd)", conn);
                    sqlCommand.Parameters.AddWithValue("@fn", txtFirstName);
                    sqlCommand.Parameters.AddWithValue("@ln", txtLastName);
                    sqlCommand.Parameters.AddWithValue("@sd", txtStartDate);
                    int a = sqlCommand.ExecuteNonQuery();
                    if (a == 1)
                    {
                        // if successful set the selected item to the one that was just added.
                        currEmployeeIndex = employees.Count;
                    }

                }
                else if (bCanEdit)
                {

                    conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    sqlCommand = new SqlCommand("update dbo.EmployeeInfo set firstName=@fn, lastName=@ln, startDate=@sd where ID=@id", conn);
                    sqlCommand.Parameters.AddWithValue("@fn", txtFirstName);
                    sqlCommand.Parameters.AddWithValue("@ln", txtLastName);
                    sqlCommand.Parameters.AddWithValue("@sd", txtStartDate);
                    sqlCommand.Parameters.AddWithValue("@id", employees[SelectedItem].employeeID.ToString());
                    int a = sqlCommand.ExecuteNonQuery();
                    if (a == 1)
                    {
                        //MessageBox.Show("Database update successful");
                    }
                    currEmployeeIndex = SelectedItem;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            // get all employees and populate DataGrid
            FillDataGrid();

            // set button logic
            bCanCancel = false;
            bCanUpdate = false;
            bCanAdd = true;
            bCanEdit = false;
            bCanDelete = false;
        }
        public void RemoveEmployee()
        {
            try
            {
                MessageBoxResult mbr =  MessageBox.Show("Do You Want Delete ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(mbr== MessageBoxResult.Cancel)
                {                 
                    return;
                }
                else
                { 
                    conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    sqlCommand = new SqlCommand("delete from dbo.EmployeeInfo where ID=@id", conn);
                    sqlCommand.Parameters.AddWithValue("@id", employees[SelectedItem].employeeID.ToString());
                    int a = sqlCommand.ExecuteNonQuery();
                    if (a == 1)
                    {
                        //MessageBox.Show("Database update successful");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            FillDataGrid();

            //Set button logic
            bCanCancel = false;
            bCanUpdate = false;
            bCanAdd = true;
            bCanEdit = false;
            bCanDelete = false;

            //set field values
            txtFirstName = "-none-";
            txtLastName = "-none-";
            txtStartDate = "-none-";
            OnPropertyChanged("txtFirstName");
            OnPropertyChanged("txtLastName");
            OnPropertyChanged("txtStartDate");


        }
        public void CancelUpdate()
        {
            bCanCancel = false;
            bCanUpdate = false;
            bCanEdit = false;
            bCanAdd = true;

            //set field values
            ClearTextFields( "-none-");
        }
        public void SelectedEmployee()
        {
            if (SelectedItem >= 0 && SelectedItem < employees.Count)
            {
                //txtSelectedEmployee = SelectedItem.fullName + "(ID: " + SelectedItem.employeeID + ", Started: " + SelectedItem.startDate + ")";
                txtSelectedEmployee = employees[SelectedItem].fullName + "(ID: " + employees[SelectedItem].employeeID + ", Started: " + employees[SelectedItem].startDate + ")";
                OnPropertyChanged("txtSelectedEmployee");

                // set button logic
                bCanCancel = false;
                bCanUpdate = false;
                bCanAdd = true;
                bCanEdit = true;
                bCanDelete = true;

                ClearTextFields("-none-");
            }
        }
        public void ClearTextFields(String str)
        {
            //set field values
            txtFirstName = str;
            txtLastName = str;
            txtStartDate = str;
            OnPropertyChanged("txtFirstName");
            OnPropertyChanged("txtLastName");
            OnPropertyChanged("txtStartDate");
        }


        /// <summary>
        /// definition of commands for binding in xaml files
        /// </summary>
        public ICommand AddEmployee
        {
            get;
            private set;
        }
        public ICommand EditEmployee
        {
            get;
            private set;
        }
        public ICommand UpdateEmployee
        {
            get;
            private set;
        }
        public ICommand DeleteEmployee
        {
            get;
            private set;
        }
        public ICommand CancelChanges
        {
            get;
            private set;
        }


        /// <summary>
        /// Code Snippet for INotifyPropertyChanged implementation for WPF binding
        /// </summary>
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

# endregion
    }
}
