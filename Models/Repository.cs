using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;

namespace cvicenie_mvc.Models
{
    public class Repository
    {
        public static List<StudentModel> students = new List<StudentModel>();
        String connectionString = "Server=tcp:cv2serv.database.windows.net,1433;Initial Catalog=cv2db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default";
        public StudentModel GetJsonStudent(int studentId)
        {
            return students.Where(e => e.Id == studentId).First();
        }
        public List<StudentModel> GetAllStudentsFromJson(HttpResponseMessage getData)
        {
            Clear();
            string results = getData.Content.ReadAsStringAsync().Result;

            // Handle null or empty results safely
            if (string.IsNullOrEmpty(results))
            {
                return new List<StudentModel>(); // Return an empty list if no data was retrieved
            }

            // Safely deserialize the JSON
            var deserializedStudents = JsonConvert.DeserializeObject<List<StudentModel>>(results);

            // If deserialization failed, return an empty list
            if (deserializedStudents == null)
            {
                return new List<StudentModel>();
            }
            List<StudentModel> JsonStudents = deserializedStudents;

            foreach (var student in JsonStudents)
            {
                students.Add(student);
            }
            return students;
        }

        public String Prediction(HttpResponseMessage getData) {
            string results = getData.Content.ReadAsStringAsync().Result;
            return results.ToString();
        }


        public void Clear() { students.Clear(); }
        public List<StudentModel> GetAllStudents()
        {
            Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT * FROM Students";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentModel student = new StudentModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Gender = reader.GetString(2),
                                City = reader.GetString(3),
                                Graduated = reader.GetBoolean(4)
                            };
                            students.Add(student);
                        }
                    }
                }
            }
            return students;
        }

        public void Create(StudentModel student)
        {
            students.Add(student);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Включаємо можливість вставляти власний Id
                using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Students ON", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Вставка запису (твій старий варіант)
                string sql = "INSERT INTO Students (Id, Name, Gender, City, Graduated) " +
                             "VALUES (" + student.Id + ", '" + student.Name + "', '" + student.Gender + "', '" + student.City + "', '" + student.Graduated + "')";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Вимикаємо вставку Id після вставки
                using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Students OFF", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public void Update(StudentModel student, int studentId)
        {
            GetAllStudents();
            students.Where(e => e.Id == studentId).First().Name = student.Name;
            students.Where(e => e.Id == studentId).First().Gender = student.Gender;
            students.Where(e => e.Id == studentId).First().City = student.City;
            students.Where(e => e.Id == studentId).First().Graduated = student.Graduated;


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE Students SET Name ='" + student.Name + "', Gender ='" + student.Gender + "', City ='" + student.City + "', Graduated ='" + student.Graduated + "' WHERE Id=" + studentId + ";";
                    using (SqlCommand command = new SqlCommand(sql, connection)) command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
        public void Delete(int studentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "DELETE FROM Students WHERE Id =" + studentId + ";";
                    using (SqlCommand command = new SqlCommand(sql, connection)) command.ExecuteNonQuery();
                }

                StudentModel student = students.Where(e => e.Id == studentId).First();
                students.Remove(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
