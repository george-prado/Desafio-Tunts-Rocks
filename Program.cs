namespace Solution
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Google.Apis.Auth.OAuth2;
	using Google.Apis.Services;
	using Google.Apis.Sheets.v4;
	using Google.Apis.Sheets.v4.Data;
	using Students.ExtensionMethods;

	class Program
	{
		//Metadata
		static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static readonly string sheet = "engenharia_de_software";
		static readonly string SpreadsheetId = "1mws7Gr2MBrlBAYOoicBNQyqcmpXfwuxoVQKa_HfCNkU";
		static SheetsService service;

		static List<Student> studentList = new List<Student>();

		static void Main(string[] args)
		{
			Init();

			GetDataFromSheet(columnInit: 'A', columnEnd: 'H', rowInit: 4, rowEnd: 27);

			UpdateStatusColumn(initCell: 4, endCell: 27);

			UpdateNfaColumn(initCell: 4, endCell: 27);
		}

		//Needed to read credentials for OAuth 2.0 and call Google Sheet API
		static void Init()
		{
			GoogleCredential credential;
			
			using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				credential = GoogleCredential.FromStream(stream)
					.CreateScoped(Scopes);
			}

			service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "GeorgePrado",
			});
		}

		//Needed to collect data from Spreadsheet to Student objects
		static void GetDataFromSheet(char columnInit, char columnEnd, int rowInit, int rowEnd)
		{
			// Determine which columns and rows the method will read
			var range = $"{sheet}!{columnInit}{rowInit}:{columnEnd}{rowEnd}";
			SpreadsheetsResource.ValuesResource.GetRequest request =
					service.Spreadsheets.Values.Get(SpreadsheetId, range);

			var response = request.Execute();

			IList<IList<object>> values = response.Values;

			if (values != null && values.Count > 0)
			{
				//Pass values to Student properties
				foreach (var row in values)
				{
					//Table-based data fields
					Student student = new Student();
					student.Id = Convert.ToInt32(row[0]);
					student.Name = Convert.ToString(row[1]);
					student.Absences = Convert.ToInt32(row[2]);
					student.P1 = Convert.ToDouble(row[3]);
					student.P2 = Convert.ToDouble(row[4]);
					student.P3 = Convert.ToDouble(row[5]);

					//Method-based data fields
					student.Media = student.GetAvgScore();
					student.Status = student.GetStatus();
					student.Nfa = student.GetNfa();

					studentList.Add(student);
				}
			}
			else
			{
				Console.WriteLine("No data found.");
			}
		}

		//support methods for providing parameters to the main (UpdateColumn) method.
		static void UpdateStatusColumn(int initCell, int endCell)
		{
			UpdateColumn('G', initCell, endCell, (student) => student.Status);
		}
		static void UpdateNfaColumn(int initCell, int endCell)
		{
			UpdateColumn('H', initCell, endCell, (student) => student.Nfa.ToString());
		}

		//Update data entries on Spreadsheet based
		static void UpdateColumn(char columnLetter, int initCell, int endCell, Func<Student, string> valueProvider)
		{
			for (int i = initCell; i <= endCell; i++)
			{
				var cellRange = $"{sheet}!{columnLetter}{i}";
				var student = studentList[i - initCell];

				Console.WriteLine($"Coluna {columnLetter}, Célula {i}, Matrícula {student.Id} atualizada.");

				var valueRange = new ValueRange
				{
					Values = new List<IList<object>> { new List<object> { valueProvider(student) } }
				};

				var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, cellRange);
				updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

				try
				{
					var appendResponse = updateRequest.Execute();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Erro na atualização da célula: {ex.Message}");
				}
			}
		}
	}
}