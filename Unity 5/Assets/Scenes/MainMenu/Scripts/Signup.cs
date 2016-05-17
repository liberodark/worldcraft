using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Net;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Globalization;
using System.Text.RegularExpressions;

public class Signup : AbstractMenu {

	private static float lastSignupFailed = 0;
	private static bool signupFailed = false;
	private static string failedMessage;
	private string CheckPassword = "";

	protected override void OnMenuGUI(){

		GUIStyle centeredStyle = new GUIStyle (GUI.skin.label);
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		centeredStyle.normal.textColor = Color.red;

		if (signupFailed && Time.time - lastSignupFailed < 3f) {
			GUILayout.Label (failedMessage,centeredStyle,GUILayout.Width(490));
		} else if (Time.time - lastSignupFailed >= 3f) {
			signupFailed = false;
		}

		GUILayout.BeginHorizontal (GUILayout.Width(490));
			GUILayout.Label ("Login: ",GUILayout.Width(245));
			GameStateManager.Login = GUILayout.TextField(GameStateManager.Login, GUILayout.Width(245));
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal (GUILayout.Width(490));
			GUILayout.Label ("Password: ",GUILayout.Width(245));
			GameStateManager.Password = GUILayout.PasswordField(GameStateManager.Password, '*', GUILayout.Width(245));
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal (GUILayout.Width(490));
		GUILayout.Label ("Check Password: ",GUILayout.Width(245));
		CheckPassword = GUILayout.PasswordField(CheckPassword, '*', GUILayout.Width(245));
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal (GUILayout.Width(490));
			GUILayout.Label ("Email: ",GUILayout.Width(245));
			GameStateManager.Email = GUILayout.TextField(GameStateManager.Email, GUILayout.Width(245));
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal (GUILayout.Width(490));
		if(GUILayout.Button("Back",GUILayout.ExpandWidth(true))){
			SwitchTo<LauncherGUI>();
		}

		if(GameStateManager.Password != "" && GameStateManager.Login != "" && GameStateManager.Email != "" && GUILayout.Button("Submit",GUILayout.Width(245))){
			RegexUtilities rg = new RegexUtilities();
			if(rg.IsValidEmail(GameStateManager.Email)){
					
					if(GameStateManager.Password == CheckPassword){
						using (WebClient wb = new WebClient())
						{
							NameValueCollection data = new NameValueCollection();
							
							string hashString = HashString(GameStateManager.Password);
							
							data["login"] = GameStateManager.Login;
							data["psw"] = hashString;
							data["mail"] = GameStateManager.Email;
							data["mode"] = "up";
							
							byte[] response = wb.UploadValues(MainMenu.url, "POST", data);
							using (MD5 md5Hash = MD5.Create())
							{
								if (!VerifyMd5Hash(md5Hash, "exist", System.Text.Encoding.UTF8.GetString(response)))
								{
									GameStateManager.connectionID = System.Text.Encoding.UTF8.GetString(response);
									GameStateManager.loggedIn = true;
									GameStateManager.Password = HashString(GameStateManager.Password);
									GameStateManager.startProcess = true;
									SwitchTo<MainMenu>();
								}
								else{
									signupFailed = true;
									failedMessage = "Your login or email address is already used";
									lastSignupFailed = Time.time;
								}
							}
						}
					}else{
						signupFailed = true;
						failedMessage = "Passwords doesn't match";
						lastSignupFailed = Time.time;
					}
				}
				else{
					signupFailed = true;
					failedMessage = "Invalid email address";
					lastSignupFailed = Time.time;
				}
		}
		GUILayout.EndHorizontal ();

	}


	public static string GetMd5Hash(MD5 md5Hash, string input)
	{
		
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
		
		StringBuilder sBuilder = new StringBuilder();
		
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		
		return sBuilder.ToString();
	}

	public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
	{
		string hashOfInput = GetMd5Hash(md5Hash, input);
		
		StringComparer comparer = StringComparer.OrdinalIgnoreCase;
		
		if (0 == comparer.Compare(hashOfInput, hash))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static string HashString(string s){
		byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
		SHA256Managed hashstring = new SHA256Managed();
		byte[] hash = hashstring.ComputeHash(bytes);
		string hashString = string.Empty;
		foreach (byte x in hash)
		{
			hashString += System.String.Format("{0:x2}", x);
		}
		return hashString;
	}
}

public class RegexUtilities
{
	bool invalid = false;
	
	public bool IsValidEmail(string strIn)
	{
		invalid = false;
		if (String.IsNullOrEmpty(strIn))
			return false;
		
		// Use IdnMapping class to convert Unicode domain names.
		try {
			strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
		}
		catch (Exception) {
			return false;
		}
		
		if (invalid) 
			return false;
		
		// Return true if strIn is in valid e-mail format.
		try {
			return Regex.IsMatch(strIn, 
			                     @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + 
			                     @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", 
			                     RegexOptions.IgnoreCase);
		}  
		catch (Exception) {
			return false;
		}
	}
	
	private string DomainMapper(Match match)
	{
		// IdnMapping class with default property values.
		IdnMapping idn = new IdnMapping();
		
		string domainName = match.Groups[2].Value;
		try {
			domainName = idn.GetAscii(domainName);
		}
		catch (ArgumentException) {
			invalid = true;      
		}      
		return match.Groups[1].Value + domainName;
	}
}
