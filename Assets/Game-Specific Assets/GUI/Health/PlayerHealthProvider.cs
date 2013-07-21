using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class PlayerHealthProvider : MonoBehaviour 
{
	#region Variables / Properties
	
	public Texture2D FullHeart;
	public Texture2D HalfHeart;
	public Texture2D NoHeart;
	
	public AsvarduilImage UiWidget;
	
	private HealthSystem _playerHealthSystem;
	public Texture2D _fullUiWidget;
	private int _lastHP;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_playerHealthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>();
		_lastHP = _playerHealthSystem.HP;
		
		CalculateWidget();
	}
	
	public void OnGUI()
	{
		UiWidget.DrawMe();
	}
	
	public void FixedUpdate()
	{
		if(_lastHP != _playerHealthSystem.HP)
		{
			CalculateWidget();
			_lastHP = _playerHealthSystem.HP;
		}
		
		UiWidget.Tween();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void CalculateWidget()
	{
		int canvasWidth = FullHeart.width * (_playerHealthSystem.MaxHP / 2);
		int fullHearts = _playerHealthSystem.HP / 2;
		int halfHearts = _playerHealthSystem.HP % 2;
		int deadHearts = (_playerHealthSystem.MaxHP / 2) - (_playerHealthSystem.HP / 2);
		
		Texture2D tex = new Texture2D(canvasWidth, FullHeart.height);
		
		int i = 0;
		
		for(int counter = 0; counter < fullHearts; counter++)
		{
			tex.SetPixels(i, 0, FullHeart.width, FullHeart.height, FullHeart.GetPixels());
			i += FullHeart.width;
		}
		
		for(int counter = 0; counter < halfHearts; counter++)
		{
			tex.SetPixels(i, 0, FullHeart.width, FullHeart.height, HalfHeart.GetPixels());
			i += FullHeart.width;
		}
		
		for(int counter = 0; counter < deadHearts; counter++)
		{
			tex.SetPixels(i, 0, FullHeart.width, FullHeart.height, NoHeart.GetPixels());
			i += FullHeart.width;
		}
		
		tex.Apply();
		
		_fullUiWidget = tex;
		UiWidget.Image = _fullUiWidget;
	}
	
	#endregion Methods
}
