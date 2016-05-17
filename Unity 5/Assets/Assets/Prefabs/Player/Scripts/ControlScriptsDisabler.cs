using UnityEngine;
using System.Collections;

public class				ControlScriptsDisabler : MonoBehaviour
{
	#region Input Data
	public MonoBehaviour[]	_control_scripts;
	#endregion

	#region Unity
	void					OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if (!this.GetComponent<NetworkView>().isMine && this._control_scripts != null)
		{
			foreach (MonoBehaviour script in this._control_scripts)
				script.enabled = false;
		}
	}
	#endregion
}
