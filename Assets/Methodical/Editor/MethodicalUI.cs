//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;

namespace Linkage.Methodical{
	public class MethodicalUI : EditorWindow {
		private string command = "";
		public static Dictionary<string,string> methodicalProps = null;
		private Texture2D clearTexture;
		private Texture twitterNormal;
		private bool staticMode = false;
		private MonoBehaviour go;
		private string staticVariable = "";
		private Vector2 scrollPosition = Vector2.zero;
		private List<MethodicalMessage> display = new List<MethodicalMessage>();//commands and messages. What is actually displayed in window.
		private List<string> commands = new List<string>();//commands only
		private bool commandCalled = false;//!> allows me to set the scrollview to the end. stupid hacky stuff
		//private float largestCommandWidth = 1f;//!>largest command width in pixels. used for horizontal scroll bar;
		private float lastWindowHeight = 20f;//!> the last retrieved window height. Lets me see if it has changed, will reset scroll view if necessary
		//A few variables for making up and down arrows work
		private int commandIndex = -1;//!>Index for keeping track of previous commands, allows using up and down arrows to traverse commands. -1 is cached command
		private string cachedCommand = "";//!> last typed text cached for traversel previous commands. Lets me come back later if necessary 
		private float lastConsoleContentHeight = 0;


		[MenuItem ("Window/Methodical")]
		public static void ShowWindow () {
			EditorWindow window =  EditorWindow.GetWindow (typeof( MethodicalUI));
			float windowRatio = 16f / 9f;
			float width = 1920f/2f;
			window.position = new Rect(100f,100f,width,width * (1f/windowRatio));
			window.title = "Methodical";
		}

		public void OnEnable(){
			clearTexture = MakeTex(new Color(0f,0f,0f,0f));

			twitterNormal = (Texture)Resources.Load("methodicaltwitternormal");
			GetProps ();
		}

		public Object testObject;
		public string testString = "test";

		public void OnGUI () {
			
			//Drawing a scrolling window with previous commands
			//float scrollWindowHeight = 20f * commands.Count;
			float clearButtonOffset = 15;
			float consoleContentHeight = 0;
			//Debug.Log (-lastConsoleContentHeight + ' ' + position.height);
			if (-lastConsoleContentHeight < position.height - 20) {
				lastConsoleContentHeight = -(position.height - 20);
				clearButtonOffset = 0;
			}
			GUIStyle boxStyle = new GUIStyle (GUI.skin.box);
			boxStyle.normal.background = MakeTex(new Color(.883f,.883f,.883f));
			GUI.Box (new Rect(0,0,position.width,position.height),"",boxStyle);

			scrollPosition = GUI.BeginScrollView(new Rect(0f,0f,position.width,position.height-20f),scrollPosition,new Rect(0f,0f,position.width-20,-lastConsoleContentHeight));

			if (display.Count == 0) {
					//Drawing the version number, support links and twitter
					GUIStyle versionNumStyle = SetLabelColor (new Color (.635f, .635f, .635f));
					float versionNumWidth = versionNumStyle.CalcSize (new GUIContent ("v." + MethodicalUI.methodicalProps ["VersionNum"])).x;
					float supportWidth = versionNumStyle.CalcSize (new GUIContent ("Support")).x;
					float logoWidth = versionNumStyle.CalcSize(new GUIContent("ABXY")).x;
					//GUI.Label (new Rect(10,position.height - 40,versionNumWidth,20),"v."+MethodicalUI.methodicalProps["VersionNum"],versionNumStyle);
					if (GUI.Button (new Rect (35, position.height - 40, versionNumWidth, 20), "v." + MethodicalUI.methodicalProps ["VersionNum"], SetButtonStyle (new Color (0.635f, 0.635f, 0.635f), new Color (0.396f, 0.396f, 0.396f)))) {
							Application.OpenURL (MethodicalUI.methodicalProps ["UpdateCheckURL"] + "?vnum=" + MethodicalUI.methodicalProps ["VersionNum"]);
					}
					if (GUI.Button (new Rect (45 + versionNumWidth, position.height - 40, supportWidth, 20), "Support", SetButtonStyle (new Color (0.635f, 0.635f, 0.635f), new Color (0.396f, 0.396f, 0.396f)))) {
							Application.OpenURL (MethodicalUI.methodicalProps ["SupportLink"]);
					}
					if (GUI.Button (new Rect (10, position.height - 40, 15, 15), twitterNormal, SetButtonStyle (Color.white, Color.white))) {
							Application.OpenURL (MethodicalUI.methodicalProps ["TwitterHandle"]);
					}
					if (GUI.Button (new Rect (position.width - logoWidth - 10, position.height - 40, logoWidth, 20), "ABXY", SetButtonStyle (new Color (0.635f, 0.635f, 0.635f), new Color (0.396f, 0.396f, 0.396f)))) {
							Application.OpenURL (MethodicalUI.methodicalProps ["LinkageSite"]);
					}

			}
			foreach (MethodicalMessage cmd in display){
				consoleContentHeight = Print(cmd,consoleContentHeight);
				

			}
			
			//going to end of command list if new command issued
			if (commandCalled){
				//Debug.Log("scrolling to");
				scrollPosition =  new Vector2(0,-consoleContentHeight);// go to end if command called	
			}
			commandCalled = false;//resetting
			
			GUI.EndScrollView();
			
			//checking for enter
			if (Event.current.Equals(Event.KeyboardEvent("return")))
				CallCommand();
			//check for up arrow
			if (Event.current.Equals(Event.KeyboardEvent("up")))
				CallUpArrow();
			//check for down arrow
			if (Event.current.Equals(Event.KeyboardEvent("down")))
				CallDownArrow();
			
			//setting the min size so I can shrink it down real small
			minSize = new Vector2(500f,20f);	




			//drawing bars
			//go = (MonoBehaviour)EditorGUI.ObjectField(new Rect(0f,position.height - 20f,position.width*.20f,20f),new GUIContent("","hello"),go,typeof(MonoBehaviour),true);
			DoSplitObjectSelector(position);
			
			GUI.SetNextControlName ("MethodicalCommandField");
			command = GUI.TextField(new Rect(position.width*.22f,position.height - 20f,(position.width * .78f) - 40f,20f),command);
			if(GUI.Button(new Rect(position.width -40f, position.height - 20f,40f,20f),"->")){
				CallCommand();
			}
			
			//clear button
			if ((commands.Count !=0 || display.Count != 0) && position.height > 40f && GUI.Button(new Rect(position.width - 50f-clearButtonOffset,0f,50f,20f),"Clear")){
				Clear();	
			}
			//resetting the scroll window if the height of the editor has changed. Fixes some grossness
			if (lastWindowHeight != position.height){
				scrollPosition =  new Vector2(0,-consoleContentHeight);
				lastWindowHeight = position.height;
			}
			lastConsoleContentHeight = consoleContentHeight;

			

		}
		
		/*!
		 * Calls commands to the selected monobehaviour using invoke
		 * 
		 * */
		private void CallCommand(){
			if (go == null){
				//display.Insert(0,new MethodicalMessage("No monobehavior selected!",MethodicalMessage.MessageTypes.Error));
				//return;
			}
			if (command.Equals("")){
				
				display.Insert(0,new MethodicalMessage("No command entered!",MethodicalMessage.MessageTypes.Error));
				return;
			}
			commandCalled = true;
			MethodicalExecutor.isStatic = staticMode;
			MethodicalExecutor.root = go;
			MethodicalExecutor.staticRoot = staticVariable;

			MethodicalObject chain = MethodicalInterpreter.MakeChain(go,command);

			
			
			object returnValue = MethodicalExecutor.execute(chain);
			
			commands.Insert(0,command);
			display.Insert(0,command);
			if (chain.GetType ().Equals (typeof(MethodicalErrorObject))) {
				display.Insert(0,new MethodicalMessage(chain.ToString(),MethodicalMessage.MessageTypes.Error));	
			}else if (returnValue!=null && returnValue.GetType ().Equals (typeof(MethodicalErrorObject))) {
				display.Insert(0,new MethodicalMessage(returnValue.ToString(),MethodicalMessage.MessageTypes.Error));	
			}
			else if (returnValue!=null){
				display.Insert(0,new MethodicalMessage("-     " + returnValue,MethodicalMessage.MessageTypes.Return));	
			}
			command = "";
			commandIndex = -1;
			cachedCommand = "";
			EditorGUI.FocusTextInControl ("MethodicalCommandField");
		}
		
		/*!
		 * handles traversal of previous commands
		 * */
		private void CallUpArrow(){
			//caching current text
			if (commandIndex == -1)
				cachedCommand = command;
			
			if (commandIndex + 1 < commands.Count)
				commandIndex++;
			
			//setting current command
			if (commandIndex == -1 )
				command = cachedCommand;
			else{
				command = commands[commandIndex];	
			}
			//Debug.Log(commandIndex);
		}
		
		/*!
		 * handles traversal of previous commands
		 * */
		private void CallDownArrow(){
			//caching current text
			if (commandIndex == -1)
				cachedCommand = command;
			
			if (commandIndex - 1 >= -1)
				commandIndex--;
			//setting current command
			if (commandIndex == -1 )
				command = cachedCommand;
			else{
				command = commands[commandIndex];	
			}
			//Debug.Log(commandIndex);
				
		}
		
		/*!
		 * clears the console
		 * */
		private void Clear(){
			commands = new List<string>();
			display = new List<MethodicalMessage>();
			command = "";

			commandIndex = -1;
			cachedCommand = "";
		}

		/*!
		 * Returns a GUIStyle for changing label color
		 * */
		private GUIStyle SetLabelColor(Color color){
			GUIStyle localStyle = new GUIStyle(EditorStyles.label);
			localStyle.normal.textColor = color;
			return localStyle;
		}

		private GUIStyle SetButtonStyle(Color normal, Color highlight){
			GUIStyle localStyle = new GUIStyle ();
			localStyle.normal.background = clearTexture;
			localStyle.hover.background =clearTexture;
			localStyle.normal.textColor = normal;
			localStyle.hover.textColor = highlight;
			
			return localStyle;
		}

		//Returns new line hight
		private float Print(MethodicalMessage cmd, float consoleContentHeight){
			GUIStyle labelStyle = new GUIStyle (EditorStyles.label);
			GUIStyle boxStyle = new GUIStyle (GUI.skin.box);
			labelStyle.active.textColor = Color.black;
			labelStyle.focused.textColor = Color.black;
			boxStyle.border = new RectOffset (0,0,0,0);
			boxStyle.normal.background = MakeTex(new Color(.883f,.883f,.883f));
			
			labelStyle.wordWrap = true;
			float leftMargin = 5f;
			float textHeight = labelStyle.CalcHeight(new GUIContent(cmd),position.width - leftMargin);

			
			
			
			
			if (cmd.MessageType.Equals (MethodicalMessage.MessageTypes.Return)) {
				//labelStyle.normal.textColor = new Color (.2f, .2f, .2f);
				leftMargin = 20;
				boxStyle.normal.background = MakeTex (new Color (.92f, .92f, .92f));
			} else if (cmd.MessageType.Equals (MethodicalMessage.MessageTypes.Error)) {
				//labelStyle.normal.textColor = new Color (.2f, .2f, .2f);
				leftMargin = 20;
				boxStyle.normal.background = MakeTex (new Color (.8f, .56f, .56f));
			}


			
			consoleContentHeight = consoleContentHeight - textHeight;
			GUI.Box (new Rect(0,-lastConsoleContentHeight+consoleContentHeight,position.width,textHeight),"",boxStyle);
			//if (consoleContentHeight > position.height){
				//Debug.Log("should have bars right now");
				EditorGUI.SelectableLabel(new Rect(leftMargin,-lastConsoleContentHeight+consoleContentHeight, position.width - leftMargin,textHeight),cmd,labelStyle);
			//}
			//else{
				//Debug.Log("No Bars");
				//GUI.Label(new Rect(leftMargin,-lastConsoleContentHeight+consoleContentHeight, position.width-leftMargin,textHeight),cmd,labelStyle);
			//}
			return consoleContentHeight;
		}

		private Texture2D MakeTex(Color color){
			Texture2D texture = new Texture2D (1, 1);
			texture.SetPixel (0, 0, color);
			texture.Apply ();
			return texture;
		}

		private Dictionary<string,string> GetProps(){
			Dictionary<string,string> props = new Dictionary<string,string>();
			XmlDocument xDoc = new XmlDocument ();
			xDoc.Load ("assets/Methodical/props.xml");
			string versionNum = xDoc.GetElementsByTagName ("VersionNum")[0].InnerText;
			string supportLink = xDoc.GetElementsByTagName ("SupportLink") [0].InnerText;
			string twitterHandle = xDoc.GetElementsByTagName ("TwitterHandle") [0].InnerText;
			string updateCheckURL = xDoc.GetElementsByTagName ("UpdateCheckURL") [0].InnerText;
			string linkageSite = xDoc.GetElementsByTagName ("LinkageSite")[0].InnerText;
			props.Add("VersionNum", versionNum);
			props.Add("SupportLink", supportLink);
			props.Add("TwitterHandle", twitterHandle);
			props.Add ("UpdateCheckURL", updateCheckURL);
			props.Add ("LinkageSite",linkageSite);
			methodicalProps = props;
			return props;
		}

		// If you could represent regret and bad decisions in code form, it would look like this method...
		private void DoSplitObjectSelector(Rect position) {

			//staticMode = EditorGUI.Toggle(new Rect(0f, position.height - 20f, 30f, 20f), "S", staticMode, "Button");
			staticMode = GUI.Toggle(new Rect(0f, position.height - 20f, 30f, 20f),staticMode,"s");
			if (staticMode) {
				staticVariable = (EditorGUI.TextField(new Rect(30f, position.height - 20f, (position.width * .20f) - 20f, 20f),staticVariable));
			}
			else {
				go = (MonoBehaviour)EditorGUI.ObjectField(new Rect(30f, position.height - 20f, (position.width * .20f) - 20f, 20f), new GUIContent("", "hello"), go, typeof(MonoBehaviour), true);
			}



			
		}
	}
}
