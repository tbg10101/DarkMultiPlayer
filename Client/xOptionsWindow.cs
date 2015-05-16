using System;
using UnityEngine;

namespace DarkMultiPlayer {
	public class xOptionsWindow {
		public static xOptionsWindow instance;
		
		private static Rect windowRect = new Rect (Screen.width / 2f + 300 / 2f, Screen.height / 2f - 543 / 2f, 300, 543);
		private int descWidth = 75;
		private int sepWidth = 5;
		
		private static bool bringToFront = false;
		
		private GUIStyle sectionHeaderStyle = new GUIStyle();
		private GUIStyle descriptorStyle = new GUIStyle();
		private GUIStyle playerNameStyle = new GUIStyle(GUI.skin.textField);
		private GUIStyle noteStyle = new GUIStyle();
		private GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		
		private long lastSecondSentColor = 0;
		
		private bool settingChat = false;
		private bool settingScreenshot = false;
		private string settingKeyMessage = "cancel";
		
		private xOptionsWindow () {
			instance = this;
			
			lock (Client.eventLock) {
				Client.drawEvent.Add(this.Draw);
			}
			
			Texture2D sectionHeader = new Texture2D(1, 1);
			sectionHeader.SetPixel(0, 0, new Color(0, 0, 0, 0.75f));
			sectionHeader.Apply();
			sectionHeaderStyle.normal.background = sectionHeader;
			sectionHeaderStyle.normal.textColor = Color.white;
			sectionHeaderStyle.padding = new RectOffset(4, 4, 2, 2);
			sectionHeaderStyle.alignment = TextAnchor.MiddleCenter;
			sectionHeaderStyle.fontStyle = FontStyle.Bold;
			
			descriptorStyle.normal.textColor = Color.white;
			descriptorStyle.padding = new RectOffset(4, 4, 2, 2);
			descriptorStyle.alignment = TextAnchor.MiddleRight;
			
			playerNameStyle.normal.textColor = Settings.fetch.playerColor;
			playerNameStyle.padding = new RectOffset(4, 4, 2, 2);
			playerNameStyle.alignment = TextAnchor.MiddleLeft;
			
			noteStyle.normal.textColor = new Color(1, 1, 1, 0.75f);
			noteStyle.fontSize = 12;
			noteStyle.padding = new RectOffset(4, 4, 2, 2);
			noteStyle.alignment = TextAnchor.UpperLeft;
			noteStyle.wordWrap = true;
			
			buttonStyle.padding = new RectOffset(4, 4, 2, 2);
		}
		
		public static void show() {
			if (instance == null) {
				instance = new xOptionsWindow();
			}
			
			bringToFront = true;
		}
		
		public static void close() {
			Client.drawEvent.Remove(instance.Draw);
			instance = null;
		}
		
		private void Draw () {
			windowRect = GUI.Window (6711 + Client.WINDOW_OFFSET, windowRect, DrawContent, "Options");
		}
		
		private void DrawContent (int windowID) {
			//Boilerplate Window (lots could be abstracted out)
			if (GUI.Button (new Rect (windowRect.width-16, 3, 13, 13), "")) {
				close();
			}
			
			GUI.DragWindow(new Rect(0, 0, windowRect.width, 17));
			
			if (bringToFront) {
				GUI.BringWindowToFront(windowID);
				GUI.FocusWindow(windowID);
				bringToFront = false;
			}
			
			int groupY;
			
			int windowY = 17;
			
			//Player
			GUI.DragWindow(new Rect(0, windowY, windowRect.width, 20));
			GUI.Box(new Rect(2, windowY, windowRect.width-4, 20), "Player", sectionHeaderStyle);
			
			windowY += 20 + 2;
			
			GUI.BeginGroup(new Rect(10, windowY, windowRect.width - 20, 106));
			groupY = 0; 
			
			//Player Name
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Name:", descriptorStyle);
			playerNameStyle.normal.textColor = Settings.fetch.playerColor;
			if (NetworkWorker.fetch.state == DarkMultiPlayerCommon.ClientState.RUNNING) {
				GUI.Label(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), Settings.fetch.playerName, playerNameStyle);
			} else {
				string newName = GUI.TextField(new Rect(descWidth + sepWidth, 0, windowRect.width - (descWidth + sepWidth) - 20, 20), Settings.fetch.playerName, playerNameStyle);
				
				if (! newName.Equals(Settings.fetch.playerName)) {
					Settings.fetch.playerName = newName;
					Settings.fetch.SaveSettings();
				}
			}
			groupY += 20 + 4;
			
			//Player Color
			Color newcolor = Settings.fetch.playerColor;
			
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Red:", descriptorStyle);
			newcolor.r = GUI.HorizontalSlider(new Rect(descWidth + sepWidth, groupY + 5, windowRect.width - (descWidth + sepWidth) - 20, 12), Settings.fetch.playerColor.r, 0, 1);
			groupY += 20;
			
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Green:", descriptorStyle);
			newcolor.g = GUI.HorizontalSlider(new Rect(descWidth + sepWidth, groupY + 5, windowRect.width - (descWidth + sepWidth) - 20, 12), Settings.fetch.playerColor.g, 0, 1);
			groupY += 20;
			
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Blue:", descriptorStyle);
			newcolor.b = GUI.HorizontalSlider(new Rect(descWidth + sepWidth, groupY + 5, windowRect.width - (descWidth + sepWidth) - 20, 12), Settings.fetch.playerColor.b, 0, 1);
			groupY += 22;
			
			if (GUI.Button(new Rect(0, groupY, windowRect.width - 20, 20), "Randomize Color", buttonStyle)) {
				newcolor = PlayerColorWorker.GenerateRandomColor();
			}
			
			if (! newcolor.Equals(Settings.fetch.playerColor)) {
				Settings.fetch.playerColor = newcolor;
				Settings.fetch.SaveSettings();
			}
			
			if (NetworkWorker.fetch.state == DarkMultiPlayerCommon.ClientState.RUNNING && lastSecondSentColor != DateTime.Now.Second) {
				PlayerColorWorker.fetch.SendPlayerColorToServer();
				
				lastSecondSentColor = DateTime.Now.Second;
			}
			
			GUI.EndGroup();
			windowY += 106 + 5;
			
			//Key Bindings
			GUI.DragWindow(new Rect(0, windowY, windowRect.width, 20));
			GUI.Box(new Rect(2, windowY, windowRect.width-4, 20), "Key Bindings", sectionHeaderStyle);
			
			windowY += 20 + 2;
			
			GUI.BeginGroup(new Rect(10, windowY, windowRect.width - 20, 90));
			groupY = 0;
			
			//Info
			GUI.Label(new Rect(0, groupY, windowRect.width - 20, 48), "Click a button below to select the action you want to change. Then press a key to set the binding. To cancel, click the button again or press Escape.", noteStyle);
			groupY += 48;
			
			//Chat
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Chat:", descriptorStyle);
			
			string chatDescription = Settings.fetch.chatKey.ToString();
			if (settingChat) {
				chatDescription = settingKeyMessage;
				if (Event.current.isKey) {
					if (Event.current.keyCode != KeyCode.Escape) {
						Settings.fetch.chatKey = Event.current.keyCode;
						Settings.fetch.SaveSettings();
					}
					
					settingChat = false;
				}
			}
			
			if (GUI.Button(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), chatDescription, buttonStyle)) {
				settingScreenshot = false;
				
				settingChat = !settingChat;
			}
			groupY += 22;
			
			//Screenshot
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Screenshot:", descriptorStyle);
			
			string screenshotDescription = Settings.fetch.screenshotKey.ToString();
			if (settingScreenshot) {
				screenshotDescription = settingKeyMessage;
				if (Event.current.isKey) {
					if (Event.current.keyCode != KeyCode.Escape) {
						Settings.fetch.screenshotKey = Event.current.keyCode;
						Settings.fetch.SaveSettings();
					}
					
					settingScreenshot = false;
				}
			}
			
			if (GUI.Button(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), screenshotDescription, buttonStyle)) {
				settingChat = false;
				
				settingScreenshot = !settingScreenshot;
			}
			
			GUI.EndGroup();
			windowY += 90 + 5;
			
			//Cache Size
			GUI.DragWindow(new Rect(0, windowY, windowRect.width, 20));
			GUI.Box(new Rect(2, windowY, windowRect.width-4, 20), "Cache", sectionHeaderStyle);
			
			windowY += 20 + 2;
			
			GUI.BeginGroup(new Rect(10, windowY, windowRect.width - 20, 84));
			groupY = 0;
			
			//Current Size
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Current:", descriptorStyle);
			GUI.Label(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20 - 22, 20), Mathf.Round(UniverseSyncCache.fetch.currentCacheSize / 1024).ToString());
			GUI.Label(new Rect(windowRect.width - 40, groupY, 20, 20), "KB");
			groupY += 20;
			
			//Set Maximum
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Maximum:", descriptorStyle);
			string newSizeStr = GUI.TextField(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20 - 22, 20), (Settings.fetch.cacheSize / 1024).ToString());
			GUI.Label(new Rect(windowRect.width - 40, groupY, 20, 20), "KB");
			
			int newSize;
			if (newSizeStr == "") {
				newSize = 1;
			} else {
				if (int.TryParse(newSizeStr, out newSize)) {
					if (newSize < 1) {
						newSize = 1;
					} else if (newSize > 1000000) {
						newSize = 1000000;
					}
				} else {
					newSize = 100000;
				}
			}
			
			if (newSize != Settings.fetch.cacheSize) {
				Settings.fetch.cacheSize = newSize * 1024;
				Settings.fetch.SaveSettings();
			}
			groupY += 22;
			
			//Management
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Manage:", descriptorStyle);
			if (GUI.Button(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), "Expire")) {
				UniverseSyncCache.fetch.ExpireCache();
			}
			groupY += 22;
			
			if (GUI.Button(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), "Delete")) {
				UniverseSyncCache.fetch.DeleteCache();
			}
			
			GUI.EndGroup();
			windowY += 84 + 5;
			
			//DMPModControl.txt
			GUI.DragWindow(new Rect(0, windowY, windowRect.width, 20));
			GUI.Box(new Rect(2, windowY, windowRect.width-4, 20), "DMPModControl.txt", sectionHeaderStyle);
			
			windowY += 20 + 2;
			
			GUI.BeginGroup(new Rect(10, windowY, windowRect.width - 20, 42));
			groupY = 0;
			
			//Mod Control File Generation
			GUI.Label(new Rect(0, groupY, descWidth, 20), "Generate:", descriptorStyle);
			if (GUI.Button(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), "Whitelist")) {
				ModWorker.fetch.GenerateModControlFile(true);
			}
			groupY += 22;
			
			if (GUI.Button(new Rect(descWidth + sepWidth, groupY, windowRect.width - (descWidth + sepWidth) - 20, 20), "Blacklist")) {
				ModWorker.fetch.GenerateModControlFile(false);
			}
			
			GUI.EndGroup();
			windowY += 42 + 5;
			
			//Other
			GUI.DragWindow(new Rect(0, windowY, windowRect.width, 20));
			GUI.Box(new Rect(2, windowY, windowRect.width-4, 20), "Other", sectionHeaderStyle);
			
			windowY += 20 + 2;
			
			GUI.BeginGroup(new Rect(10, windowY, windowRect.width - 20, 64));
			groupY = 0;
			
			bool newCompress = GUI.Toggle(new Rect(0, groupY, windowRect.width - 20, 20), Settings.fetch.compressionEnabled, " Compress Network Traffic");
			if (newCompress != Settings.fetch.compressionEnabled) {
				Settings.fetch.compressionEnabled = newCompress;
				Settings.fetch.SaveSettings();
			}
			groupY += 22;
			
			UniverseConverterWindow.fetch.display = GUI.Toggle(new Rect(0, groupY, windowRect.width - 20, 20), UniverseConverterWindow.fetch.display, "Generate DMP universe from saved game...", buttonStyle);
			groupY += 22;
			
			if (GUI.Button(new Rect(0, groupY, windowRect.width - 20, 20), "Reset Disclaimer", buttonStyle)) {
				Settings.fetch.disclaimerAccepted = 0;
				Settings.fetch.SaveSettings();
			}
			
			GUI.EndGroup();
		}
	}
}