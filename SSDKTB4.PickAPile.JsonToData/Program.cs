// See https://aka.ms/new-console-template for more information
using System.Data;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

 string jsonFile = "data.json";


var json = await File.ReadAllTextAsync(jsonFile);
var data = JsonConvert.DeserializeObject<PickAPileModel>(json);

string connectionString = "Server=.;Database=SSDKMiniPOS;User ID=sa;Password=sasa@123;TrustServerCertificate=True;";

using (IDbConnection db = new SqlConnection(connectionString))
{
	db.Open();

	foreach (var question in data.Questions)
	{
		string insertQuestionSql = @"
            INSERT INTO Questions (QuestionId, QuestionName, QuestionDesp)
            VALUES (@QuestionId, @QuestionName, @QuestionDesp);
        ";

		await db.ExecuteAsync(insertQuestionSql, question);
	}

	foreach (var answer in data.Answers)
	{
		string insertAnswerSql = @"
            INSERT INTO Answers (AnswerId, AnswerImageUrl, AnswerName, AnswerDesp, QuestionId)
            VALUES (@AnswerId, @AnswerImageUrl, @AnswerName, @AnswerDesp, @QuestionId);
        ";

		await db.ExecuteAsync(insertAnswerSql, answer);
	}
}

Console.WriteLine("Data inserted successfully!");
Console.ReadLine();
public class PickAPileModel
{
	public Question[] Questions { get; set; }
	public Answer[] Answers { get; set; }
}

public class Question
{
	public int QuestionId { get; set; }
	public string QuestionName { get; set; }
	public string QuestionDesp { get; set; }
	public List<Answer> Answers { get; set; } = new();
}

public class Answer
{
	public int AnswerId { get; set; }
	public string AnswerImageUrl { get; set; }
	public string AnswerName { get; set; }
	public string AnswerDesp { get; set; }
	public int QuestionId { get; set; }
}
;