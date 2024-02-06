namespace Solution
{
	namespace Students.ExtensionMethods
	{
		public static class StudentsExtension
		{
			public static double GetAvgScore(this Student student)
			{
				double avg = Math.Round((student.P1 + student.P2 + student.P3) / 3);

				return avg;
			}
			public static string GetStatus(this Student student)
			{
				const double AbsenceThresholdPercent = 25.0;
				double absencePercent = Math.Round(Convert.ToDouble((student.Absences) * 100 / Student.NoOfClasses));
				string status = "";

				status = student.Media switch
				{
					var m when m < 50 => "Reprovado por Nota",
					var m when m < 70 => "Exame Final",
					_ => "Aprovado",
				};
				if (absencePercent >= AbsenceThresholdPercent)
				{
					status = "Reprovado por Falta";
				}

				return status;
			}
			public static double GetNfa(this Student student)
			{
				if (student.Status == "Exame Final")
					return 100 - student.Media;

				return 0;
			}
		}
	}
}