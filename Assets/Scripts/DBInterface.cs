using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class DBInterface : MonoBehaviour
{
    private MySqlConnectionStringBuilder stringBuilder;

    public string Server;
    public string Database;
    public string UserID;
    public string Password;

    // Start is called before the first frame update
    void Start()
    {
        stringBuilder = new MySqlConnectionStringBuilder();
        stringBuilder.Server = Server;
        stringBuilder.Database = Database;
        stringBuilder.UserID = UserID;
        stringBuilder.Password = Password;
    }

    public void InsertHighscore(string playerName, int highscore)
    {
        using (MySqlConnection connection = new MySqlConnection(stringBuilder.ConnectionString))
        {
            try
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO highscores (playerName, highscore) VALUES (@playerName, @highscore)";
                command.Parameters.AddWithValue("@playerName", playerName);
                command.Parameters.AddWithValue("@highscore", highscore);

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("DBInterface: Could not insert the highscore! " + System.Environment.NewLine + ex.Message);
            }
        }
    }

    public List<System.Tuple<string,int>> RetrieveTopFiveHighscores()
    {
        List<System.Tuple<string, int>> topFive = new List<System.Tuple<string, int>>();

        using(MySqlConnection connection = new MySqlConnection(stringBuilder.ConnectionString))
        {
            try
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT playername, highscore FROM highscores ORDER BY highscore DESC LIMIT 5";
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var ordinal = reader.GetOrdinal("playername");
                    string playername = reader.GetString(ordinal);
                    ordinal = reader.GetOrdinal("highscore");
                    int highscore = reader.GetInt32(ordinal);
                    System.Tuple<string, int> entry = new System.Tuple<string, int>(playername, highscore);
                    topFive.Add(entry);
                }

                connection.Close();
            }
            catch(System.Exception ex)
            {
                Debug.LogError("DBInterface: Could not retrieve the top five highscores! " + System.Environment.NewLine + ex.Message);
            }
        }

        return topFive;
    }
}
