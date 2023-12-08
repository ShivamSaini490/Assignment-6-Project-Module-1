using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/*Project Description:
 
The Student Management System is a dynamic and comprehensive C# Windows (Or Web) Form
Application designed to streamline the management of student-related information within an
educational institution. This project encompasses modules that cover key aspects of student data,
faculty details, attendance tracking, marks management, and financial records. As you progress through
the modules, you'll gain practical experience in C# programming, database interactions using SQL Server,
and the development of user-friendly interfaces.*/

/*Module 1: Setting up the Project
 1.Create Project Structure:
 • Create a new C# Windows (or Web) Forms Application using Visual Studio.*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static StudentManagementSystem.MainForm;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;
using System.Data;

namespace StudentManagementSystem
{
    public partial class MainForm : Form
    {
        private List<Student> students = new List<Student>();
        private List<Faculty> faculties = new List<Faculty>();
        private List<Attendance> attendanceRecords = new List<Attendance>();
        private List<Marks> marksRecords = new List<Marks>();
        private List<FinancialRecord> financialRecords = new List<FinancialRecord>();

        public MainForm()
        {
            InitializeComponent();
        }

                public class Student
        {
            public int StudentID { get; set; }
            public string Name { get; set; }
            
        }

        public class Faculty
        {
            public int FacultyID { get; set; }
            public string Name { get; set; }

        }

        public class Attendance
        {
            public int AttendanceID { get; set; }
            public int StudentID { get; set; }
            public DateTime Date { get; set; }
            public bool IsPresent { get; set; }

        }

        public class Marks
        {
            public int MarksID { get; set; }
            public int StudentID { get; set; }
            public string Subject { get; set; }
            public int Score { get; set; }

        }

        public class FinancialRecord
        {
            public int RecordID { get; set; }
            public int StudentID { get; set; }
            public decimal AmountPaid { get; set; }
            public DateTime PaymentDate { get; set; }

        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            Student newStudent = new Student();
            newStudent.StudentID = GenerateStudentID(); 
            newStudent.Name = txtStudentName.Text; 

            students.Add(newStudent);

        }

    }
}


/* 2.Database Setup:
• Design and create tables for Students, Marks, Subjects, Fees, and Faculty.
• Establish relationships between tables, e.g., linking students to their marks, subjects,
and fees.*/

/*--Students Table
CREATE TABLE Students (
    StudentID INT PRIMARY KEY,
    Name NVARCHAR(100),
    RollNumber NVARCHAR(20)
    --Add other student-related fields
);

//--Subjects Table
CREATE TABLE Subjects (
    SubjectID INT PRIMARY KEY,
    SubjectName NVARCHAR(100)
    --Add other subject-related fields
);

--Marks Table
CREATE TABLE Marks (
    MarkID INT PRIMARY KEY,
    StudentID INT,
    SubjectID INT,
    MarksObtained INT,
    -- Add other fields related to marks
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY(SubjectID) REFERENCES Subjects(SubjectID)
);

--Fees Table
CREATE TABLE Fees (
    FeeID INT PRIMARY KEY,
    StudentID INT,
    Amount DECIMAL(10, 2),
    --Add other fields related to fees
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

--Faculty Table
CREATE TABLE Faculty (
    FacultyID INT PRIMARY KEY,
    Name NVARCHAR(100),
    Department NVARCHAR(50)
    --Add other faculty-related fields
);*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Student
{
    [Key]
    public int StudentID { get; set; }
    public string Name { get; set; }
    public string RollNumber { get; set; }

    public virtual ICollection<Marks> Marks { get; set; }
    public virtual ICollection<Fees> Fees { get; set; }
}

public class Subject
{
    [Key]
    public int SubjectID { get; set; }
    public string SubjectName { get; set; }

    public virtual ICollection<Marks> Marks { get; set; }
}

public class Marks
{
    [Key]
    public int MarkID { get; set; }
    public int StudentID { get; set; }
    public int SubjectID { get; set; }
    public int MarksObtained { get; set; }

    [ForeignKey("StudentID")]
    public virtual Student Student { get; set; }

    [ForeignKey("SubjectID")]
    public virtual Subject Subject { get; set; }
}

public class Fees
{
    [Key]
    public int FeeID { get; set; }
    public int StudentID { get; set; }
    public decimal Amount { get; set; }

    [ForeignKey("StudentID")]
    public virtual Student Student { get; set; }
}

public class Faculty
{
    [Key]
    public int FacultyID { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
}

using System.Data.Entity;

public class SchoolContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Marks> Marks { get; set; }
    public DbSet<Fees> Fees { get; set; }
    public DbSet<Faculty> Faculties { get; set; }

    public SchoolContext() : base("name=SchoolContext")
    {
    }
}

/*3.Connect to Database:
• Implement a database connection class using ADO.NET to facilitate interactions with the
SQL Server database.*/

public class DatabaseConnection
{
    private readonly string connectionString;
    private SqlConnection connection;

    public DatabaseConnection(string server, string database, string username, string password)
    {
        connectionString = $"Server={server};Database={database};User Id={username};Password={password};";
    }

    private void OpenConnection()
    {
        try
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while opening connection: " + ex.Message);
        }
    }

    private void CloseConnection()
    {
        try
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while closing connection: " + ex.Message);
        }
    }

    public DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();
        try
        {
            OpenConnection();
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error executing query: " + ex.Message);
        }
        finally
        {
            CloseConnection();
        }
        return dataTable;
    }

    public int ExecuteNonQuery(string query)
    {
        int rowsAffected = 0;
        try
        {
            OpenConnection();
            SqlCommand command = new SqlCommand(query, connection);
            rowsAffected = command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error executing non-query: " + ex.Message);
        }
        finally
        {
            CloseConnection();
        }
        return rowsAffected;
    }
}

string server = "your_server_address";
string database = "your_database_name";
string username = "your_username";
string password = "your_password";

DatabaseConnection dbConnection = new DatabaseConnection(server, database, username, password);

string selectQuery = "SELECT * FROM Students";
DataTable result = dbConnection.ExecuteQuery(selectQuery);

string insertQuery = "INSERT INTO Students (Name, RollNumber) VALUES ('John Doe', 'A001')";
int rowsAffected = dbConnection.ExecuteNonQuery(insertQuery);
