﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoresEasy : MonoBehaviour
{
    const string privateCode = "OidSOP8n50qVVW_4zZCV_AvjMXaCZknkGMndr3bz77ew";  //Key to Upload New Info
    const string publicCode = "615156c88f40bb0e2873dd9e";   //Key to download
    const string webURL = "http://dreamlo.com/lb/"; //  Website the keys are for
    public static string trackuserName;
    public static int trackuserScore;
    public PlayerScore[] scoreList;
    DisplayHighscoresEasy myDisplay;

    static HighScoresEasy instance; //Required for STATIC usability
    void Awake()
    {
        instance = this; //Sets Static Instance
        myDisplay = GetComponent<DisplayHighscoresEasy>();
    }
    
    public static void UploadScore(string username, int score)  //CALLED when Uploading new Score to WEBSITE
    {//STATIC to call from other scripts easily
        instance.StartCoroutine(instance.DatabaseUpload(username,score)); //Calls Instance
    }

    public void sendScore()
    {
        UploadScore(trackuserName, trackuserScore);
        WinScreenTrigger.saveEasy = false;
    }
    IEnumerator DatabaseUpload(string userame, int score) //Called when sending new score to Website
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(userame) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
            DownloadScores();
        }
        else print("Error uploading" + www.error);
    }

    public void DownloadScores()
    {
        StartCoroutine("DatabaseDownload");
    }
    IEnumerator DatabaseDownload()
    {
        //WWW www = new WWW(webURL + publicCode + "/pipe/"); //Gets the whole list
        WWW www = new WWW(webURL + publicCode + "/pipe/0/10"); //Gets top 10
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            OrganizeInfo(www.text);
            myDisplay.SetScoresToMenu(scoreList);
        }
        else print("Error uploading" + www.error);
    }

    void OrganizeInfo(string rawData) //Divides Scoreboard info by new lines
    {
        string[] entries = rawData.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        scoreList = new PlayerScore[entries.Length];
        for (int i = 0; i < entries.Length; i ++) //For each entry in the string array
        {
            string[] entryInfo = entries[i].Split(new char[] {'|'});
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            scoreList[i] = new PlayerScore(username,score);
            print(scoreList[i].username + ": " + scoreList[i].score);
        }
    }
}

public struct PlayerScoreEasy //Creates place to store the variables for the name and score of each player
{
    public string username;
    public int score;

    public PlayerScoreEasy(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}