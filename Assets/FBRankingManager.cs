using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System;
using System.Text;

public class FBRankingManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;

    //User Data variables
    [Header("Ranking Variables")]
    public GameObject scoreElement;
    public Transform scoreboardContent;

    //Register variables
    [Header("SendData")]
    public TMP_InputField nicknameField;
    public TMP_InputField timeField;


    private void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    

    public void SendData()
    {
        var id = GenerateRandomId();

        if (nicknameField.text != "" && timeField.text != "")
        {
            var nick = nicknameField.text;
            var time = int.Parse(timeField.text);

            UploadDatapackageToFirebase(nick + "-" +id, nick, time);

            nicknameField.text = "";
            timeField.text = "";

            ScoreboardButton();
        }
    }

    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData()); 
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase");
        //Set the authentication instance object
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;        
    }

    private IEnumerator UpdateUsernameDatabase(string id, string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("ranking").Child(id).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateTime(string id, int _time)
    {
        //Set the currently logged in user time
        var DBTask = DBreference.Child("ranking").Child(id).Child("time").SetValueAsync(_time);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Time is now updated
        }
    }

    private IEnumerator UpdatePair(string id, string key, string value)
    {
        var DBTask = DBreference.Child("ranking").Child(id).Child(key).SetValueAsync(value);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Pait are now updated
        }
    }

    private void UploadDatapackageToFirebase(string _id, string _nick, int _time)
    {
        StartCoroutine(UpdatePair( _id, "username", _nick));
        StartCoroutine(UpdateTime(_id, _time));
    }

    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by time amount
        var DBTask = DBreference.Child("ranking").OrderByChild("time").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            var ordinal = 1;
            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                var username = childSnapshot.Child("username").Value.ToString();
                int time = int.Parse(childSnapshot.Child("time").Value.ToString());
               
                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(
                    "#" + ordinal.ToString(),
                    username, GetHourMinuteSeconds(time), 
                    0);
               
                ordinal++;

                if (ordinal > 10)
                    break;


            }

            //Go to scoareboard screen
            //UIManager.instance.RankingScreen();
        }
    }

    private string GetHourMinuteSeconds(int n)
    {
        var minutes = (n / 60).ToString();
        var seconds = (n % 60).ToString();

        if (seconds.Length == 1)
            seconds = "0" + seconds;

        return minutes + ":" + seconds;
    }

    private string GenerateRandomId()
    {
        StringBuilder builder = new StringBuilder();
        Enumerable
           .Range(65, 26)
            .Select(e => ((char)e).ToString())
            .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            .OrderBy(e => Guid.NewGuid())
            .Take(10)
            .ToList().ForEach(e => builder.Append(e));
        return builder.ToString();
    }
}
