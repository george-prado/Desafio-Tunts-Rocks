namespace Solution
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Absences { get; set; }
        public double P1 { get; set; }
        public double P2 { get; set; }
        public double P3 { get; set; }
        public string Status { get; set; }
        public double Media { get; set; }
        public double Nfa { get; set; }
		public static int NoOfClasses { get; } = 60;
	} 
}