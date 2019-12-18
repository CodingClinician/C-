using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary1;
using System.Runtime.Serialization.Json;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace WpfApp_HWK_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        CourseWork cw = new CourseWork();
  

        string selectedFileName;

        static string connString = @"server=(LocalDB)\MSSQLLocalDB;" +
                "integrated security=SSPI;" +
                "database=CourseWork;" +
            "MultipleActiveResultSets=True";

        SqlConnection sqlConn = new SqlConnection(connString);




        public MainWindow()
        {
          
            InitializeComponent();

            ////OPEN file dialog below  this is the first method in this even handler
            #region            
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = @"C:\Users\toby2\source\repos\WpfApp1_HWK_4\WpfApp1_HWK_4\bin\Debug";
            ofd.Title = "Find JSON file source";

            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Filter = "(*.json) | *.json";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            ofd.ReadOnlyChecked = true;
            ofd.ShowReadOnly = true;


            if (ofd.ShowDialog() == true)  /// is this right
            {
                string filename = "courseworkansi.json";
                filename = ofd.FileName;
                selectedFileName = ofd.FileName;            /// set to global filename will be used for JSON read
            }
            else { return; }///////END  OPENfile Dialog
            //////
            /////  Read JSON begins here 
            ///
            #region
            FileStream reader = new FileStream(selectedFileName, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(reader, Encoding.UTF8);
            string jsonString = streamReader.ReadToEnd();

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream stream = new MemoryStream(byteArray);

            DataContractJsonSerializer inputSerializer;
            inputSerializer = new DataContractJsonSerializer(typeof(CourseWork));
            cw = (CourseWork)inputSerializer.ReadObject(stream);
            stream.Close();

            ////SQL connection 
            sqlConn.Open();
            #endregion

            ////////////// db insertion after JSON read  //////////
            #region
            listBox.Items.Clear();
            SqlCommand comm = new SqlCommand("delete from SubmissionsTable", sqlConn);
            int rowAffected = comm.ExecuteNonQuery();
            foreach (Submission sub in cw.submissions)
            {
                string sql = "INSERT INTO SubmissionsTable" +
              "(CategoryName, AssignmentName, Grade) Values" +
              "(@param1, @param2, @param3)";
                SqlConnection sqlConn1 =  new SqlConnection(connString);
                sqlConn1.Open();

                SqlCommand command = new SqlCommand(sql, sqlConn);
                command.Parameters.Add(new SqlParameter("@param1", sub.categoryName));
                command.Parameters.Add(new SqlParameter("@param2", sub.assignmentName));
                command.Parameters.Add(new SqlParameter("@param3", sub.grade));
                int rowsAffected = command.ExecuteNonQuery();

            }

            // add code to get data from database and populate LB from there

        }

        /// ////////////////////////////////////////////////////////////////
        ///     Event  handling for importJSON course work sub menu
        ///     This seb menu is under import
        ///     Will generate a file dialog box
        ///     File dialog box will allow selection of a JSON file
        ///     To be read and parsed
        ///     PArsed JSON file will set values to database
        ///     Values to be set in database
        ///         Category Name
        ///         Assignment Name
        ///         Grade
        /// 
        /// 
        ////////////////////////////////////////////////////////////////
        #region
        private void selectJSONFile(object sender, RoutedEventArgs e)
        {
            /*
            SqlConnection sqlConn;
            sqlConn = new SqlConnection(connString);
            
            /sqlConn.Open(); // Open the connectio
            */
            string sql = "SELECT * FROM SubmissionsTable";
            SqlCommand command = new SqlCommand(sql, sqlConn);

            // Retrieve the data from the database
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            while (reader.Read())
            {
                
               
               listBox.Items.Add(reader["AssignmentName"]);
                //listBox.Items.Add(reader["CategoryName"]);
                //listBox.Items.Add(reader["Grade"]);
            }
            

        } ///  End of method selectJSONFILE   
        #endregion
        #region 
       
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
        ///
        ////       Event handler for about sub menu
        ///        sub menu found under Help menu
        ///        
        //////////////////////////////////////////////////////////////////////////////////////
        #region
        private void showDetail(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Course Work GUI\nVersion 1.1\n by \n Christian Lopez",
                            "Application Details");
 
        }  /// end  method showDetail
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        ///
        ///    Event handler for Exit sub menu
        ///    Sub menu under File menu
        ///    
        /////   A very very long and complicated method to write
        ///
        ////////////////////////////////////////////////////////////////////////////////////////
        #region
        private void closeMe(object sender, RoutedEventArgs e)
        {
            this.Close();
        }  // closeMe ends
        #endregion


        ////////////////////////////////////////////////////////////////////
        ///
        ///     Method will retrieve data from database
        ///     Will set data to 3 textboxes
        ///     Category Name textbox   = the Category
        ///     Assignment Name textbox = The name of the highlighed assignment
        ///     Grade text box  = The grade given for that particular Assignment
        ///     
        //////////////////////////////////////////////////////////////////////////////
        #region


        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string sql = "SELECT * FROM SubmissionsTable where AssignmentName = '" + listBox.SelectedItem.ToString() +"'";
            SqlCommand command = new SqlCommand(sql, sqlConn);

            // Retrieve the data from the database
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                

                assignmentTB.Text = (reader["AssignmentName"].ToString());
                categoryTB.Text = (reader["CategoryName"].ToString());
                gradeTB.Text = (reader["Grade"].ToString());
            }


        }
    }
    #endregion


}
#endregion
#endregion