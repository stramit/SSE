using UnityEditor;
using UnityEngine;
using StrumpyShaderEditor;

public class NodeEditorWindow : NodeEditor {
	[MenuItem("Shader Editor/Strumpy Shader Editor")]
	public static void Init ()
	{
		var window = GetWindow (typeof(NodeEditorWindow)) as NodeEditorWindow;
		window.wantsMouseMove = true;
		
		Object.DontDestroyOnLoad( window );
	}
	
	[MenuItem("Shader Editor/Donate")]
	public static void Donate ()
	{
		Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=59CS4BHGRLWDS&lc=AU&item_name=http%3a%2f%2fwww%2estrumpy%2enet&item_number=Strumpy%20Shader%20Editor&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHostedGuest");
	}
}
