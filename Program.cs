// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Data.SQLite;

// Class to represent a student
class Student
{
    public string StudentID { get; set; }
    public string Name { get; set; }
    public Dictionary<string, double> Units { get; set; }

    public Student(string studentID, string name)
    {
        StudentID = studentID;
        Name = name;
        Units = new Dictionary<string, double>();
    }

    public void AddSubject(string unit, double marks)
    {
        Units[unit] = marks;
    }

    public double GetTotalMarks()
    {
        double total = 0;
        foreach (var marks in Units.Values)
        {
            total += marks;
        }
        return total;
    }

    public double GetAverageMarks()
    {
        return Units.Count == 0 ? 0 : GetTotalMarks() / Units.Count;
    }

    public string GetGrade(double marks)
    {
        if (marks >= 70) return "A";
        if (marks >= 60) return "B";
        if (marks >= 50) return "C";
        if (marks >= 40) return "D";
        return "F";
    }

    public void PrintResultSlip()
    {
        Console.WriteLine("---------------------------------");
        Console.WriteLine($"Student ID:\t\t{StudentID}");
        Console.WriteLine($"Name:\t\t\t{Name}");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("Units\t\t\tMarks\tGrade");
        foreach (var subject in Units)
        {
            Console.WriteLine($"{subject.Key.PadRight(16)}\t{subject.Value}\t{GetGrade(subject.Value)}");
        }
        Console.WriteLine($"Total Marks:\t\t{GetTotalMarks()}");
        Console.WriteLine($"Average Marks:\t\t{GetAverageMarks():F2}\t{GetGrade(GetAverageMarks())}");
        Console.WriteLine("---------------------------------");
    }
}

// Database management class
class DatabaseManager
{
    private const string ConnectionString = "Data Source=students.db;Version=3;";

    public static void InitializeDatabase()
    {
        using (var conn = new SQLiteConnection(ConnectionString))
        {
            conn.Open();
            string createTableQuery = "CREATE TABLE IF NOT EXISTS Students (StudentID TEXT PRIMARY KEY, Name TEXT)";
            string createUnitsTableQuery = "CREATE TABLE IF NOT EXISTS Units (StudentID TEXT, UnitName TEXT, Marks REAL, PRIMARY KEY(StudentID, UnitName), FOREIGN KEY(StudentID) REFERENCES Students(StudentID))";
            using (var cmd = new SQLiteCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand(createUnitsTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void InsertOrUpdateStudent(Student student)
    {
        using (var conn = new SQLiteConnection(ConnectionString))
        {
            conn.Open();
            using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Students (StudentID, Name) VALUES (@StudentID, @Name)", conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.ExecuteNonQuery();
            }

            foreach (var unit in student.Units)
            {
                using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Units (StudentID, UnitName, Marks) VALUES (@StudentID, @UnitName, @Marks)", conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
                    cmd.Parameters.AddWithValue("@UnitName", unit.Key);
                    cmd.Parameters.AddWithValue("@Marks", unit.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public static Student? GetStudent(string studentID)
    {
        using (var conn = new SQLiteConnection(ConnectionString))
        {
            conn.Open();
            using (var cmd = new SQLiteCommand("SELECT Name FROM Students WHERE StudentID = @StudentID", conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader.GetString(0);
                        Student student = new Student(studentID, name);

                        using (var unitCmd = new SQLiteCommand("SELECT UnitName, Marks FROM Units WHERE StudentID = @StudentID", conn))
                        {
                            unitCmd.Parameters.AddWithValue("@StudentID", studentID);
                            using (var unitReader = unitCmd.ExecuteReader())
                            {
                                while (unitReader.Read())
                                {
                                    string unitName = unitReader.GetString(0);
                                    double marks = unitReader.GetDouble(1);
                                    student.AddSubject(unitName, marks);
                                }
                            }
                        }
                        return student;
                    }
                }
            }
        }
        return null;
    }

    public static void DeleteStudent(string studentID)
    {
        Console.Write("Are you sure you want to delete this student? (yes/no): ");
        string? input = Console.ReadLine();
        if (!string.Equals(input, "yes", StringComparison.OrdinalIgnoreCase)) return;

        using (var conn = new SQLiteConnection(ConnectionString))
        {
            conn.Open();
            using (var cmd = new SQLiteCommand("DELETE FROM Units WHERE StudentID = @StudentID", conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand("DELETE FROM Students WHERE StudentID = @StudentID", conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.ExecuteNonQuery();
            }
        }
        Console.WriteLine("Student record deleted successfully.");
    }
}

// Main program
class Program
{
    static void Main()
    {
        DatabaseManager.InitializeDatabase();

        Console.Write("Enter Student ID: "); // Add this line to prompt the user
        string studentID = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(studentID))
        {
            Student? student = DatabaseManager.GetStudent(studentID);

            if (student != null)
            {
                student.PrintResultSlip();
                Console.Write("Update marks? (yes/no): ");
                if (Console.ReadLine()?.ToLower() == "yes")
                {
                    Console.Write("Unit name: ");
                    string subject = Console.ReadLine() ?? "";
                    Console.Write("Marks: ");
                    if (double.TryParse(Console.ReadLine(), out double marks))
                    {
                        student.AddSubject(subject, marks);
                        DatabaseManager.InsertOrUpdateStudent(student);
                        Console.WriteLine("Marks updated.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                }

                Console.Write("Delete student record? (yes/no): ");
                if (Console.ReadLine()?.ToLower() == "yes")
                {
                    DatabaseManager.DeleteStudent(studentID);
                }
            }
            else
            {
                Console.Write("Student not found. Enter name to add: ");
                string name = Console.ReadLine() ?? "";
                Student newStudent = new Student(studentID, name);

                Console.Write("Unit name: ");
                string subject = Console.ReadLine() ?? "";
                Console.Write("Marks: ");
                if (double.TryParse(Console.ReadLine(), out double marks))
                {
                    newStudent.AddSubject(subject, marks);
                    DatabaseManager.InsertOrUpdateStudent(newStudent);
                    Console.WriteLine("Student record added.");
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
        }
    }
}


