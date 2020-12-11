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

        static String dbConnectionString; 
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
        public string txtTitle { get; set; }
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
            dbConnectionString = Properties.Settings.Default.DBConnectionString;
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
            SelectedItem = -1;
            OnPropertyChanged("SelectedItem");

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
                        title = row[3].ToString(),
                        startDate = (Convert.ToDateTime(row[4].ToString()).ToShortDateString())
                    });
                }

                // SelectedItem is triggered/reset after the DataGrid is reloaded, set selected item to the edited or added item after reload
                // -Improvement- scroll the DataGrid to the Selected item


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
            txtTitle = employees[SelectedItem].title;
            txtStartDate = employees[SelectedItem].startDate;
            OnPropertyChanged("txtFirstName");
            OnPropertyChanged("txtlastName");
            OnPropertyChanged("txtTitle");
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
                    sqlCommand = new SqlCommand("insert into dbo.EmployeeInfo (firstName, lastName, title, startDate) values (@fn, @ln, @tl, @sd)", conn);
                    sqlCommand.Parameters.AddWithValue("@fn", txtFirstName);
                    sqlCommand.Parameters.AddWithValue("@ln", txtLastName);
                    sqlCommand.Parameters.AddWithValue("@tl", txtTitle);
                    sqlCommand.Parameters.AddWithValue("@sd", txtStartDate);
                    int a = sqlCommand.ExecuteNonQuery();
                    if (a == 1)
                    {
                        // get all employees and populate DataGrid
                        FillDataGrid();
                        // if successful set the selected item to the one that was just added.
                        // index zero based 
                        currEmployeeIndex = employees.Count-1;
                        MessageBox.Show("Employee: " + employees[currEmployeeIndex].fullName + " Successfully Added...");                    
                    }

                }
                else if (bCanEdit)
                {

                    conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    sqlCommand = new SqlCommand("update dbo.EmployeeInfo set firstName=@fn, lastName=@ln, title=@tl, startDate=@sd where employeeID=@id", conn);
                    sqlCommand.Parameters.AddWithValue("@fn", txtFirstName);
                    sqlCommand.Parameters.AddWithValue("@ln", txtLastName);
                    sqlCommand.Parameters.AddWithValue("@tl", txtTitle);
                    sqlCommand.Parameters.AddWithValue("@sd", txtStartDate);
                    sqlCommand.Parameters.AddWithValue("@id", employees[SelectedItem].employeeID.ToString());
                    int a = sqlCommand.ExecuteNonQuery();
                    if (a == 1)
                    {
                        // get all employees and populate DataGrid
                        currEmployeeIndex = SelectedItem;
                        FillDataGrid();
                        MessageBox.Show("Employee: " + employees[currEmployeeIndex].fullName + " Successfully Updated...");
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
            // Load fields with affected employee information
            SelectedItem = currEmployeeIndex;
            OnPropertyChanged("SelectedItem");
            // set button logic
            SetButtonsSelected();
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
                    string deletedEmployee = employees[SelectedItem].fullName;
                    conn = new SqlConnection(dbConnectionString);
                    conn.Open();
                    sqlCommand = new SqlCommand("delete from dbo.EmployeeInfo where employeeID=@id", conn);
                    sqlCommand.Parameters.AddWithValue("@id", employees[SelectedItem].employeeID.ToString());
                    int a = sqlCommand.ExecuteNonQuery();
                    if (a == 1)
                    {
                        MessageBox.Show("Employee: " + deletedEmployee + " Successfully Deleted...");
                        SelectedItem = -1;
                        OnPropertyChanged("SelectedItem");
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
            SetButtonsNotSelected();

            //set field values
            ClearTextFields("-none-");

        }
        public void CancelUpdate()
        {

            // set button logic
            SetButtonsSelected();
            //set field values
            ClearTextFields( "-none-");
        }
        public void SelectedEmployee()
        {
            if (SelectedItem >= 0 && SelectedItem < employees.Count)
            {
                txtSelectedEmployee = employees[SelectedItem].fullName + "(ID: " + employees[SelectedItem].employeeID + ", Title: " + employees[SelectedItem].title + ", Started: " + employees[SelectedItem].startDate + ")";
                OnPropertyChanged("txtSelectedEmployee");

                // set button logic
                SetButtonsSelected();
            }
            else
            {
                txtSelectedEmployee = "";
                OnPropertyChanged("txtSelectedEmployee");
                //set button logic
                SetButtonsNotSelected();
            }
            ClearTextFields("-none-");

        }
        public void ClearTextFields(String str)
        {
            //set field values
            txtFirstName = str;
            txtLastName = str;
            txtTitle = str;
            txtStartDate = str;
            OnPropertyChanged("txtFirstName");
            OnPropertyChanged("txtLastName");
            OnPropertyChanged("txtTitle");
            OnPropertyChanged("txtStartDate");
        }
        public void SetButtonsNotSelected()
        {
            bCanCancel = false;
            bCanUpdate = false;
            bCanAdd = true;
            bCanEdit = false;
            bCanDelete = false;

        }

        public void SetButtonsSelected()
        {
            bCanCancel = false;
            bCanUpdate = false;
            bCanAdd = true;
            bCanEdit = true;
            bCanDelete = true;
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
