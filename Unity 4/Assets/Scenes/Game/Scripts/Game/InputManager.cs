using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

[SerializableAttribute]
public class InputManager
{
	private SortedList<string,KeyCode> _dictionnary{ get; set;}
	private static InputManager _inputManager = null;
	private static float _inventoryExiting = 0;
	private static bool loading = false;
	
	public Paire[] p {
		get {
			return Paire.SortedToPaire (dictionnary);
		}
		set{
			dictionnary = Paire.PaireToSorted(value);
		}
	}

	public float sensitivity{ get; set;}
	public float soundLevel{ get; set; }

	public static InputManager inputManager (){
		return ((_inputManager == null) ? new InputManager() : _inputManager);
	}

	public static SortedList<string,KeyCode> dictionnary{
		get{
			return _inputManager._dictionnary;
		}
		set{
			_inputManager._dictionnary = value;
		}
	}

	private InputManager(){

		_inputManager = this;

		if (!LoadInputs ()) {

			dictionnary = new SortedList<string,KeyCode> ();

			dictionnary.Add ("Hide HUD", KeyCode.F1);
			dictionnary.Add ("Screenshot", KeyCode.F2);
			dictionnary.Add ("Stats", KeyCode.F3);

			dictionnary.Add ("Fly", KeyCode.F);
			dictionnary.Add ("Left", KeyCode.A);
			dictionnary.Add ("Right", KeyCode.D);
			dictionnary.Add ("Forward", KeyCode.W);
			dictionnary.Add ("Backward", KeyCode.S);
			dictionnary.Add ("Inventory", KeyCode.E);
			dictionnary.Add ("Jump", KeyCode.Space);

			dictionnary.Add ("Go down", KeyCode.LeftShift);
			dictionnary.Add ("Spell", KeyCode.Q);
			dictionnary.Add ("Activate Item", KeyCode.X);

			sensitivity = 5f;
			soundLevel = 1.0f;
		}
	}

	public KeyCode getKey(string name){
		return dictionnary[name];
	}

	public bool isSpellLaunched {
		get {
			return Input.GetKeyDown(getKey("Spell"));
		}
	}

	public bool isItemActivated {
		get {
			return Input.GetKeyDown(getKey("Activate Item"));
		}
	}

	public bool noHUD{
		get{
			return Input.GetKeyDown(getKey("Hide HUD")); 
		}
	}

	public bool inventory {
		get {
			return Input.GetKeyDown(getKey ("Inventory"));
		}
	}

	public void inventoryExiting(){
		_inventoryExiting = Time.time;
	}

	public bool canBreakBlocks{
		get{
			return (Time.time - _inventoryExiting > 0.1f);
		}
	}

	public bool isLeftClickHold{
		get{
			return Input.GetMouseButton(0);
		}
	}

	public bool isRightClickHold{
		get{
			return Input.GetMouseButton(1);
		}
	}

	public bool LeftClick{
		get{
			return Input.GetMouseButtonDown(0);
		}
	}
	
	public bool RightClick{
		get{
			return Input.GetMouseButtonDown(1);
		}
	}

	public bool returnInput {
		get {
			return Input.GetKeyDown(KeyCode.Return);
		}
	}

	public bool respondInput {
		get {
			return Input.GetKeyDown(KeyCode.R);
		}
	}

	public bool escapeInput {
		get{
			return (Event.current.keyCode == KeyCode.Escape);
		}
	}

	public bool tabInput {
		get{
			return (Event.current.keyCode == KeyCode.Tab);
		}
	}

	public bool isHorizontalInputHold{
		get{
			return Input.GetKey(getKey("Right")) || Input.GetKey(getKey("Left"));
		}
	}

	public bool isVerticalInputHold{
		get{
			return Input.GetKey(getKey("Forward")) || Input.GetKey(getKey("Backward"));
		}
	}

	public bool isHorizontalInput{
		get{
			return Input.GetKeyDown(getKey("Right")) || Input.GetKeyDown(getKey("Left"));
		}
	}
	
	public bool isVerticalInput{
		get{
			return Input.GetKeyDown(getKey("Forward")) || Input.GetKeyDown(getKey("Backward"));
		}
	}

	public bool statsInput {
		get {
			return Input.GetKeyDown(getKey("Stats"));
		}
	}

	public bool flyInput{
		get {
			return Input.GetKeyDown(getKey("Fly"));
		}
	}

	public bool isJumpInputHold{
		get {
			return Input.GetKey(getKey("Jump"));
		}
	}

	public bool screenshotInput{
		get {
			return Input.GetKeyDown(getKey("Screenshot"));
		}
	}

	public bool jumpInput{
		get {
			return Input.GetKeyDown(getKey("Jump"));
		}
	}

	public bool isGoDownInputHold{
		get {
			return Input.GetKey (getKey ("Go down"));
		}
	}
	
	public bool goDownInput{
		get {
			return Input.GetKeyDown (getKey ("Go down"));
		}
	}

	public bool isKeyPressed {
		get{
			return (Event.current.type == EventType.keyDown);
		}
	}

	public Vector2 getMousePosition(){
		return Event.current.mousePosition;
	}

	public float getHorizontalAxis(){
		if (Input.GetKey (getKey ("Right"))) {
			return 1f;
		} else if (Input.GetKey (getKey ("Left"))) {
			return -1f;
		}
		return 0f;
	}

	public float getVerticalAxis(){
		if (Input.GetKey (getKey ("Forward"))) {
			return 1f;
		} else if (Input.GetKey (getKey ("Backward"))) {
			return -1f;
		}
		return 0f;
	}

	public bool IsKeyDown(KeyCode key)
	{
		return (Event.current.type == EventType.keyDown && Event.current.keyCode == key);
	}

	public void SerializeInputs(){
		XmlSerializer serializer = new XmlSerializer(typeof(InputManager));
		System.IO.Directory.CreateDirectory(Application.dataPath+"/Options/");
		using (StreamWriter wr = new StreamWriter(Application.dataPath+"/Options/inputs.xml")) {
			serializer.Serialize (wr, _inputManager);
		}
	}

	public bool LoadInputs(){
		if (loading)
			return true;

		loading = true;
		try{
			XmlSerializer serializer = new XmlSerializer(typeof(InputManager));
			using (StreamReader rd = new StreamReader(Application.dataPath+"/Options/inputs.xml"))
			{
				_inputManager = serializer.Deserialize(rd) as InputManager;
			}
		}
		catch(Exception){
			loading = false;
			return false;
		}
		loading = false;
		return true;
	}
}

[SerializableAttribute]
public class Paire{
	public KeyCode code{ get; set;}
	public string name{ get; set;}

	public Paire(){
		code = new KeyCode();
		name = "null";
	}

	public Paire(KeyCode k, string n){
		code = k;
		name = n;
	}

	public static Paire[] SortedToPaire(SortedList<string,KeyCode> list){
		Paire[] p = new Paire[list.Count];
		int i = 0;

		foreach (string name in list.Keys) {
			p[i] = new Paire(list[name],name);
			i++;
		}
		return p;
	}

	public static SortedList<string,KeyCode> PaireToSorted(Paire[] p){
		SortedList<string,KeyCode> list = new SortedList<string, KeyCode> ();

		foreach (Paire u in p) {
			list.Add(u.name,u.code);
		}
		return list;
	}
}

