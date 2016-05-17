using UnityEngine;
using System.Collections;

public class UploadDownloadTexture : MonoBehaviour 
{
	public enum Mode {Upload, Download};
	public Mode mode = Mode.Upload;
	public string url = "http://www.server.com/ES2.php";
	public string filename = "textureFile.txt";
	public string textureTag = "textureTag";
	public string webUsername = "ES2";
	public string webPassword = "65w84e4p994z3Oq";
	
	void Start () 
	{
		// Get texture, or throw error if texture doesn't exist.
		Texture2D texture = GetTexture ();
		if(texture == null)
			Debug.LogError ("There is no texture attached to this object.");
		
		if(mode == Mode.Upload)
			StartCoroutine(Upload(texture));
		else
			StartCoroutine(Download());
	}
	
	/* Uploads a texture to the server */
	private IEnumerator Upload(Texture2D texture)
	{
	    ES2Web web = new ES2Web(url, CreateSettings());
	      
	    // Start uploading our Texture and wait for it to finish.
	    yield return StartCoroutine(web.Upload(texture));
	      
	    if(web.isError)
	        Debug.LogError(web.errorCode + ":" + web.error);
		else
			Debug.Log ("Uploaded Successfully. Reload scene to load texture into blank object.");
	}
	
	/* Downloads a texture from the server */
	private IEnumerator Download()
	{
		 ES2Web web = new ES2Web(url, CreateSettings());
	      
	    // Start downloading our Texture and wait for it to finish.
	    yield return StartCoroutine(web.Download());
	      
	    if(web.isError)
		{
			// If there's no data to load, return.
			// Note: code "05" means that no data was found.
			if(web.errorCode == "05")
				yield return null;
				
	        Debug.LogError(web.errorCode + ":" + web.error);
		}
		
		// Load the Texture from the ES2Web object, using the correct tag.
		SetTexture( web.Load<Texture2D>(textureTag) );
		
		// Delete the data so our example works properly.
		yield return StartCoroutine (Delete());
		Debug.Log ("Texture successfully downloaded and applied to blank object.");
	}
	
	/* Deletes a texture from the server */
	private IEnumerator Delete()
	{
		 ES2Web web = new ES2Web(url, CreateSettings());
	      
	    // Delete our Texture and wait for confirmation.
	    yield return StartCoroutine(web.Delete());
	      
	    if(web.isError)
	        Debug.LogError(web.errorCode + ":" + web.error);
	}
	
	/* Creates an ES2Settings objects from the user
	 * defined settings */
	private ES2Settings CreateSettings()
	{
		ES2Settings settings = new ES2Settings();
		settings.webFilename = filename;
		settings.tag = textureTag;
		settings.webUsername = webUsername;
		settings.webPassword = webPassword;
		return settings;
	}
	
		/* Gets the Texture applied to this object, 
	 * or returns null if there's not one. */
	private Texture2D GetTexture()
	{
		if(renderer.material != null)
			if(renderer.material.mainTexture != null)
				return renderer.material.mainTexture as Texture2D;
		return null;	
	}
	
	/* Sets the texture to the one specified, or throws an error
	 * if there's no Material to apply the texture to. */
	private void SetTexture(Texture2D texture)
	{
		if(renderer.material != null)
				renderer.material.mainTexture = texture;
		else
			Debug.LogError ("There is no material attached to this object.");
	}
}
