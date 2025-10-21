namespace cvicenie_mvc.Models
{
    public class StudentModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public bool? Graduated { get; set; } = false;
        public string? Scores { get; set; }
        

    }
}
