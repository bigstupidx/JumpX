using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class GoogleManager : GoogleSingleton<GoogleManager>
{


	[Serializable]
	public class TestScore
	{
		public int test;
	}

	TestScore TS = new TestScore();
	Text CloudText;


	RawImage testfriendImg;

	void Start()
	{
		//testfriendImg = GameObject.Find("testfriendImg").GetComponent<RawImage>();
		DontDestroyOnLoad(this);
	}

	public bool bLogin {
		get;
		set;
	}

	public void InitializeGPGS() //초기화.
	{
		TS.test = 100;
		bLogin = false;
	
	}

	public bool CheckLogin()//로그인 상태확인.
	{
		return Social.localUser.authenticated;
	}

	public void SaveToCloud()//클라우드 세이브.
	{
		if (!Social.localUser.authenticated) { //로그인 안되있으면 리턴.
			LoginGPGS();

			return;
		}

		CloudText.text = "첫번째 세이브";
		OpenSavedGame("SaveTest", true);
	}

	void OpenSavedGame(string filename, bool bSave)
	{
		
	}



	public void LoadFromCloud() //클라우드 로그시키기.
	{
		if (!Social.localUser.authenticated) { //로그인 안되있으면 리턴.

			LoginGPGS();

			return;
		}

		OpenSavedGame("SaveTest", false);
     
	}


	public void ReportScoreLeaderBoard(int score) //리더보드에 스코어 저장.
	{
		if (!Social.localUser.authenticated) {
			LoginGPGS();
			return;
		}
		Social.ReportScore(score, GPGS.LeaderBoardTest, (bool success) => {
			if (success) {
				Debug.Log("리더보드 성공");
			} else {
				Debug.Log("리더보드 실패");
			}
		});
	}

	public void LeaderBoardLoadScores() //리더보드 스코어 가져오는것.
	{
		if (!Social.localUser.authenticated) {
			LoginGPGS();
			return;
		}
		ILeaderboard lb = Social.CreateLeaderboard();
		lb.id = GPGS.LeaderBoardTest;
		lb.LoadScores(ok => {
			if (ok) {
				LoadUsersAndDisplay(lb);
			} else {
				Debug.Log("Error retrieving leaderboardi");
			}
		});
	}

	internal void LoadUsersAndDisplay(ILeaderboard lb) //리더보드 디스플레이
	{
		// get the user ids
		List<string> userIds = new List<string>();

       
		foreach (IScore score in lb.scores) {
			Debug.Log("lb.score === " + score.value);
			Debug.Log("Social.localUser ==== " + Social.localUser.id);
			Debug.Log("score.userId ==== " + score.userID);
			userIds.Add(score.userID);
		}
		// load the profiles and display (or in this case, log)
		Social.LoadUsers(userIds.ToArray(), (users) => {
			string status = "Leaderboard loading: " + lb.title + " count = " +
			                lb.scores.Length;
			foreach (IScore score in lb.scores) {
				IUserProfile user = FindUser(users, score.userID);
				status += "\n" + score.formattedValue + " by " +
				(string)(
				    (user != null) ? user.userName : "**unk_" + score.userID + "**");
			}

			Debug.Log(status);
		});
	}

	IUserProfile FindUser(IUserProfile[] users, string userID)
	{
		foreach (IUserProfile user in users)
			if (user.id == userID)
				return user;
		return null;
	}

	public void LoadingFriends() //친구들 불러오는곳
	{
		if (!Social.localUser.authenticated) {
			LoginGPGS();
			return;
		}
		/* 유저 email
        PlayGamesPlatform.Instance.GetUserEmail((status, email) =>
        {
            if (status == CommonStatusCodes.Success)
            {
                Debug.Log("주소는 : " + email);
            }
            else
            {
                Debug.Log("에러 메일 : " + status);
            }
        });*/

		Social.localUser.LoadFriends((ok) => {
			foreach (IUserProfile p in Social.localUser.friends) {
				//이름, 아이디 등등 가져올 수 있다.
				testfriendImg.texture = p.image;
			}
		});
	}

	public byte[] ObjectToByteArraySerialize(object obj) //모든 오브젝트를 바이트배열로 저장합니다. 클라우드 세이브.
	{
		using (var memoryStream = new MemoryStream()) {
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(memoryStream, obj);
			memoryStream.Flush();
			memoryStream.Position = 0;

			return memoryStream.ToArray();
		}
	}

	public T Deserialize<T>(byte[] byteData) //바이트형태를 T타입으로 리턴해줌.
	{
		using (var stream = new MemoryStream(byteData)) {
			var formatter = new BinaryFormatter();
			stream.Seek(0, SeekOrigin.Begin);
			return (T)formatter.Deserialize(stream);
		}
	}

	public void ShowLeaderboard() //리더보드 보여주기
	{
		if (!Social.localUser.authenticated)
			LoginGPGS();

		if (Social.localUser.authenticated) {
			Social.ShowLeaderboardUI();
		}
	}

	public void ShowAchievement() //업적 보여주기
	{
		if (!Social.localUser.authenticated)
			LoginGPGS();

		if (Social.localUser.authenticated) {
			Social.ShowAchievementsUI();
		}
	}

	public void LoginGPGS() //로그인
	{
		if (!Social.localUser.authenticated)
			Social.localUser.Authenticate(LoginCallBackGPGS);
	}

	public void LoginCallBackGPGS(bool result) //로그인
	{
		bLogin = result;
	}

	public void LogoutGPGS()//로그아웃
	{
	}

	public Texture2D GetImageGPGS() //내 이미지
	{
		if (Social.localUser.authenticated)
			return Social.localUser.image;
		else
			return null;
	}

	public string GetNameGPGS() // 내 이름
	{
		if (Social.localUser.authenticated)
			return Social.localUser.userName;
		else
			return null;
	}
}
