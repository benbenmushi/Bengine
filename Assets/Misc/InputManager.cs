using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
	private static InputManager m_instance;
	public static InputManager Instance
	{
		get
		{
			if (m_instance)
				InitializeInstance();
			return m_instance;
		}
	}
	public static InputManager InitializeInstance()
	{
		if (m_instance == null)
		{
			m_instance = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;

			if (m_instance == null && Camera.main != null)
			{
				GameObject go = new GameObject("InputManager");

				m_instance = go.AddComponent<InputManager>();
#if UNITY_EDITOR
				m_instance.Init(inputManagerMode.keyboard | inputManagerMode.mouse | inputManagerMode.multiTouch);
				//m_instance.Init(inputManagerMode.multiTouch);
#elif UNITY_IPHONE || UNITY_ANDROID
            m_instance.Init(inputManagerMode.multiTouch);
#elif UNITY_STANDALONE
            m_instance.Init(inputManagerMode.keyboard | inputManagerMode.mouse);
#endif
				DontDestroyOnLoad(go);
			}
		}
		return m_instance;
	}
	public static InputManager InitializeInstance(inputManagerMode mode)
	{
		if (m_instance == null)
		{
			m_instance = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;

			if (m_instance == null && Camera.main != null)
			{
				GameObject go = new GameObject("InputManager");

				m_instance = go.AddComponent<InputManager>();
				m_instance.Init(mode);
				DontDestroyOnLoad(go);
			}
		}
		return m_instance;
	}

	[System.Flags, System.Serializable]
	public enum inputManagerMode
	{
		keyboard = 0x01,
		multiTouch = 0x02,
		mouse = 0x04
	}

	private bool initialized = false;
	[SerializeField]
	private inputManagerMode _mode;
	public inputManagerMode mode
	{
		get
		{
			return this._mode;
		}
	}

	/// <summary>
	/// Classe de gestion des Touch/Click
	/// </summary>
	[System.Serializable]
	public class imTouch
	{
		public delegate void touchMove(float x, float y);
		public delegate void touchUp(GameObject o);

		public imTouch(float x, float y)
		{
			this.position = new Vector3(x, y, 0);
		}
		public touchMove onMove = null;
		public touchUp onUp = null;
		//public touchUp onUp
		//{
		//	get
		//	{
		//		return this._onUp;
		//	}
		//	set
		//	{
		//		this._onUp = value;
		//	}
		//}
		public Vector3 position;
		public override string ToString()
		{
			return "" + this.position;
		}
	}
	public class imButtonClickUp
	{
		public float viewportDelta = 0.1f;

		private Vector3 beginPos;

		public imButtonClickUp(imTouch touch, imTouch.touchUp onClick, float viewportDelta = 0.1f)
		{
			this.viewportDelta = viewportDelta;
			this.beginPos = touch.position;
			bool canceled = false;
			touch.onUp += (g) =>
				{
					if (canceled)
						return;
					if ((this.beginPos - touch.position).magnitude / Screen.height < this.viewportDelta)
					{
						onClick(g);
						canceled = true;
					}
				};
			touch.onMove += (x, y) =>
			{
				if (canceled)
					return;
				if ((this.beginPos - touch.position).magnitude / Screen.height > this.viewportDelta)
				{
					onClick(null);
					canceled = true;
				}
			};
		}
	}
	// Patern
	class touchControl : InputManager.imTouch
	{
		public touchControl(InputManager.imTouch touch)
			: base(touch.position.x, touch.position.y)
		{
			this.onUp = touch.onUp;
			this.onMove = touch.onMove;
			this.moveDelta = Vector3.zero;
		}
		public Vector3 moveDelta;
		public float downTime;
		public int id;
		public bool toRemove = false;
	}

	/// <summary>
	/// Classe de gestion des entrees clavier
	/// </summary>
	public class imKey
	{
		public delegate void keyPressed(KeyCode key);

		public imKey(KeyCode _key)
		{
			this.key = _key;
		}
		public void doKeyDown()
		{
			if (this.onDown != null)
			{
				this.onDown(this.key);
				this._pressed = true;
			}
		}
		public void doKeyUp()
		{
			if (this._pressed && this.onUp != null)
			{
				this.onUp(this.key);
				this._pressed = false;
			}
		}
		public event keyPressed onDown;
		public event keyPressed onUp;
		public KeyCode key;
		private bool _pressed;
	}

	public delegate void OnClick(GameObject o, imTouch touch);
	public delegate void OnKey(imKey key);
	public delegate void OnPatern(imTouch[] touches);

	private static event OnClick _ClickDownEvent;
	public static event OnClick ClickDownEvent // Event de Touch/Click
	{
		add
		{
			InitializeInstance();
			_ClickDownEvent += value;
		}
		remove
		{
			_ClickDownEvent -= value;
		}
	}

	// Paterns
	public event OnPatern simpleTapPatern;
	public event OnPatern simpleDownPatern;
	public event OnPatern simpleTopPatern;
	public event OnPatern simpleLeftPatern;
	public event OnPatern simpleRightPatern;
	public event OnPatern doubleTapPatern;
	public event OnPatern doubleDownPatern;
	public event OnPatern doubleTopPatern;
	public event OnPatern doubleLeftPatern;
	public event OnPatern doubleRightPatern;
	public event OnPatern doubleInnerPatern;
	public event OnPatern doubleOuterPatern;
	public float tapDelay = 0.05f;
	public float moveDistance = 0.2f;
	public float minimalSlideSpeed = 0.2f;
	private int touchCount = 0;
	private List<touchControl> touchList = new List<touchControl>();
	private float pixelDistance;
	private float minimalDistanceToDo;

	[SerializeField]
	private Camera _cam;
	public Camera cam
	{
		get
		{
			if (this._cam == null)
			{
				InputManagerCamera camInput = GameObject.FindObjectOfType<InputManagerCamera>();

				if (camInput != null)
					this._cam = camInput.cam;
				else
					this._cam = Camera.main;
			}
			return this._cam;
		}
		set
		{
			this._cam = value;
		}
	}

	// Les touches actuellement actives sur l'ecran
	// Node: le tableau est range en suivant l'ordre des Input.touches[].fingerId
	[SerializeField]
	private imTouch[] myTouches = new imTouch[20];
	private imKey[] myKey;

	void OnLevelWasLoaded()
	{
		if (m_instance != null && m_instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
			m_instance = this;
		this.myTouches[0] = new imTouch(0, 0);
		DontDestroyOnLoad(this.gameObject);
	}

	public void Init(inputManagerMode initMode)
	{
		this._mode = initMode;
		this.initialized = true;
		//this.SetMoveDistance(this.moveDistance);
		//this.ClickDownEvent += this.onClickDown;
	}

	/// <summary>
	/// Attribue des callback a une entree clavier
	/// </summary>
	/// <param name="key">La touche a attribuer</param>
	/// <param name="onDown">onDown callback</param>
	/// <param name="onUp">onUp callback</param>
	public void ListenKey(KeyCode key, imKey.keyPressed onDown, imKey.keyPressed onUp)
	{
		if (this.myKey == null)
		{
			this.myKey = new imKey[1];
			this.myKey[0] = new imKey(key);
			if (onDown != null)
				this.myKey[0].onDown += onDown;
			if (onUp != null)
				this.myKey[0].onUp += onUp;
			return;
		}
		imKey oldKey = Array.Find<imKey>(this.myKey, element => element.key == key);

		if (oldKey == null)
		{
			Array.Resize<imKey>(ref this.myKey, this.myKey.Length + 1);
			imKey newKey = new imKey(key);

			if (onDown != null)
				newKey.onDown += onDown;
			if (onUp != null)
				newKey.onUp += onUp;
			this.myKey[this.myKey.Length - 1] = newKey;
		}
		else
		{
			if (onDown != null)
				oldKey.onDown += onDown;
			if (onUp != null)
				oldKey.onUp += onUp;
		}
	}
	/// <summary>
	/// Libere les callbacks d'une entree clavier
	/// </summary>
	/// <param name="key">La touche attribue</param>
	/// <param name="onDown">onDown callback attribue precedement</param>
	/// <param name="onUp">onUp callback attribue precedement</param>
	public void StopListenKey(KeyCode key, imKey.keyPressed onDown, imKey.keyPressed onUp)
	{
		if (this.myKey == null)
			return;
		imKey oldKey = Array.Find<imKey>(this.myKey, element => element.key == key);

		if (oldKey != null)
		{
			if (onDown != null)
				oldKey.onDown -= onDown;
			if (onUp != null)
				oldKey.onUp -= onUp;
		}
	}

	protected virtual void ClickDownHandler()
	{
		if (_ClickDownEvent == null)
			return;
		if (
#if !UNITY_EDITOR
            Input.multiTouchEnabled && 
#endif
(this._mode & inputManagerMode.multiTouch) != 0 && Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.touches[i].phase == TouchPhase.Began)
				{
					imTouch touch = new imTouch(Input.touches[i].position.x, Input.touches[i].position.y);
					Ray ray = this.cam.ScreenPointToRay(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, 0));
					ray.origin = new Vector3(ray.origin.x, ray.origin.y, -10);
					RaycastHit[] hit = Physics.RaycastAll(ray, 50);

					this.myTouches[Input.touches[i].fingerId] = touch;
					if (hit.Length > 0)
					{
						this.SortHitByDept(ref hit);
						for (int i2 = 0; i2 < hit.Length; i2++)
						{
							_ClickDownEvent(hit[i2].collider.gameObject, touch);
							if (!hit[i2].collider.isTrigger)
								break;
						}
					}
					else
						_ClickDownEvent(null, touch);
				}
			}
		}
#if !UNITY_EDITOR
        else
#endif
		if ((this._mode & inputManagerMode.mouse) != 0)
		{
			if (Input.GetMouseButtonDown(0) && Input.touchCount == 0)
			{
				this.myTouches[0] = new imTouch(Input.mousePosition.x, Input.mousePosition.y);
				Ray ray = this.cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
				RaycastHit[] hit = Physics.RaycastAll(ray, 50);

				if (hit.Length > 0)
				{
					this.SortHitByDept(ref hit);
					for (int i = 0; i < hit.Length; i++)
					{
						_ClickDownEvent(hit[i].collider.gameObject, this.myTouches[0]);
						if (!hit[i].collider.isTrigger)
							break;
					}
				}
				else
					_ClickDownEvent(null, this.myTouches[0]);
				/*
				if (Physics.Raycast(ray, out hit, 50))
					this.ClickDownEvent(hit.collider.gameObject, this.myTouches[0]);
				else
					this.ClickDownEvent(null, this.myTouches[0]);
				 */
			}
		}
	}
	protected virtual void ClickUpHandler()
	{
		if (
#if !UNITY_EDITOR
            Input.multiTouchEnabled && 
#endif
(this._mode & inputManagerMode.multiTouch) != 0 && Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; ++i)
			{
				if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
				{
					int id = Input.touches[i].fingerId;
					this.myTouches[id].onMove = null;
					if (this.myTouches[id].onUp != null)
					{
						Ray ray = this.cam.ScreenPointToRay(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, 0));
						RaycastHit[] hit = Physics.RaycastAll(ray, 50);

						if (hit.Length > 0)
						{
							this.SortHitByDept(ref hit);
							for (int i2 = 0; i2 < hit.Length; i2++)
							{
								this.myTouches[id].onUp(hit[i2].collider.gameObject);
								if (!hit[i2].collider.isTrigger)
									break;
							}
						}
						else
							this.myTouches[id].onUp(null);
						this.myTouches[id].onUp = null;
					}
				}
			}
		}
#if !UNITY_EDITOR
        else
#endif
		if ((this._mode & inputManagerMode.mouse) != 0)
		{
			if (Input.GetMouseButtonUp(0) && this.myTouches[0] != null)
			{
				this.myTouches[0].onMove = null;
				if (this.myTouches[0].onUp != null)
				{
					Ray ray = this.cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
					RaycastHit[] hit = Physics.RaycastAll(ray, 50);

					if (hit.Length > 0)
					{
						this.SortHitByDept(ref hit);
						for (int i = 0; i < hit.Length; i++)
						{
							this.myTouches[0].onUp(hit[i].collider.gameObject);
							if (!hit[i].collider.isTrigger || this.myTouches[0].onUp == null)
								break;
						}
					}
					else
						this.myTouches[0].onUp(null);
					this.myTouches[0].onUp = null;
				}
			}
		}
	}
	protected virtual void MotionHandler()
	{
		if (
#if !UNITY_EDITOR
            Input.multiTouchEnabled && 
#endif
(this._mode & inputManagerMode.multiTouch) != 0 && Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.touches[i].phase == TouchPhase.Moved && this.myTouches[Input.touches[i].fingerId].onMove != null)
				{
					int id = Input.touches[i].fingerId;
					this.myTouches[id].position = new Vector2(Input.touches[i].position.x, Input.touches[i].position.y);
					this.myTouches[id].onMove(Input.touches[i].deltaPosition.x, Input.touches[i].deltaPosition.y);
				}
			}
		}
#if !UNITY_EDITOR
        else 
#endif
		if ((this._mode & inputManagerMode.mouse) != 0 && this.myTouches[0] != null && this.myTouches[0].onMove != null && (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f))
		{
			this.myTouches[0].position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			this.myTouches[0].onMove(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		}
	}
	private void KeyBoardHandler()
	{
		if ((this._mode & inputManagerMode.keyboard) == 0 || this.myKey == null)
			return;
		for (int i = 0; i < this.myKey.Length; ++i)
		{
			if (Input.GetKeyUp(this.myKey[i].key))
				this.myKey[i].doKeyUp();
		}
		if (Input.anyKeyDown)
		{
			for (int i = 0; i < this.myKey.Length; ++i)
			{
				if (Input.GetKeyDown(this.myKey[i].key))
					this.myKey[i].doKeyDown();
			}
		}
	}

	public void Update()
	{
		if (!this.initialized)
#if UNITY_EDITOR
			this.Init(inputManagerMode.keyboard | inputManagerMode.mouse | inputManagerMode.multiTouch);
#elif UNITY_IPHONE || UNITY_ANDROID
            this.Init(inputManagerMode.multiTouch);
#elif UNITY_STANDALONE
            this.Init(inputManagerMode.keyboard | inputManagerMode.mouse);
#endif
		this.ClickDownHandler();
		this.ClickUpHandler();
		this.MotionHandler();
		this.KeyBoardHandler();
		// Paterns
		//this.UpdatePaterns();
	}

	// Patern Handler
	private void onClickDown(GameObject clickedObject, InputManager.imTouch touch)
	{
		if (this.touchCount > 1)
			return;
		++this.touchCount;
		touchControl touchC = new touchControl(touch);
		touchC.id = this.touchCount;
		this.touchList.Add(touchC);
		touchC.downTime = Time.time;
		touch.onUp += (o) => { --this.touchCount; touchC.toRemove = true; };
		touch.onMove += (x, y) => { touchC.moveDelta.x += x; touchC.moveDelta.y += y; };
	}
	void UpdatePaterns()
	{
		List<touchControl> tapTouchList = null;
		this.touchList.ForEach((e) =>
		{
			if (e.toRemove ||
				(Mathf.Clamp(Time.time - e.downTime, 0, 1) * this.minimalDistanceToDo > e.moveDelta.magnitude && e.downTime + this.tapDelay < Time.time) ||
				e.moveDelta.magnitude > this.pixelDistance)
			{
				this.touchList.Remove(e);
				e.onUp = (o) => --this.touchCount;
				if (tapTouchList == null)
					tapTouchList = new List<touchControl>();
				tapTouchList.Add(e);
			}
		});
		if (tapTouchList != null)
		{
			this.touchList.ForEach((e) => { this.touchList.Remove(e); e.onUp = (o) => --this.touchCount; tapTouchList.Add(e); });
			if (tapTouchList.Count == 1)
			{
				InputManager.imTouch[] touches = new InputManager.imTouch[1];

				touches[0] = tapTouchList[0];
				if (tapTouchList[0].moveDelta.magnitude > this.pixelDistance)
				{
					this.simpleSlideAction(tapTouchList[0], touches);
				}
				else
				{
					//this.debugString = "SIMPLE TAP";
					if (this.simpleTapPatern != null)
						this.simpleTapPatern(touches);
				}
			}
			else if (tapTouchList.Count == 2)
			{
				InputManager.imTouch[] touches = new InputManager.imTouch[2];

				if (tapTouchList[0].position.x > tapTouchList[1].position.x)
				{
					touches[0] = tapTouchList[1];
					touches[1] = tapTouchList[0];
				}
				else
				{
					touches[0] = tapTouchList[0];
					touches[1] = tapTouchList[1];
				}
				if (tapTouchList[0].moveDelta.magnitude > this.pixelDistance && tapTouchList[1].moveDelta.magnitude > this.pixelDistance * 0.5f)
				{
					this.doubleSlideAction(tapTouchList, touches);
				}
				else
				{
					if (this.doubleTapPatern != null)
						this.doubleTapPatern(touches);
				}
			}
		}
	}
	void doubleSlideAction(List<touchControl> list, InputManager.imTouch[] touches)
	{
		bool h1, h2, v1, v2;
		touchControl leftTouch, rightTouch;

		if (list[0] != touches[0])
		{
			leftTouch = list[1];
			rightTouch = list[0];
		}
		else
		{
			leftTouch = list[0];
			rightTouch = list[1];
		}
		h1 = leftTouch.moveDelta.x > 0;
		h2 = rightTouch.moveDelta.x > 0;
		v1 = leftTouch.moveDelta.y > 0;
		v2 = rightTouch.moveDelta.y > 0;
		if (Mathf.Abs(list[0].moveDelta.x) > Mathf.Abs(list[0].moveDelta.y))
		{
			if (h1)
			{
				if (h2)
				{
					//right slide
					if (this.doubleRightPatern != null)
						this.doubleRightPatern(touches);
				}
				else
				{
					// inner slide
					if (this.doubleInnerPatern != null)
						this.doubleInnerPatern(touches);
				}
			}
			else
			{
				if (h2)
				{
					// out slide
					if (this.doubleOuterPatern != null)
						this.doubleOuterPatern(touches);
				}
				else
				{
					//left slide
					if (this.doubleLeftPatern != null)
						this.doubleLeftPatern(touches);
				}
			}
		}
		else
		{
			if (v1)
			{
				if (v2)
				{
					// topSlide
					if (this.doubleTopPatern != null)
						this.doubleTopPatern(touches);
				}
				else
				{
					// inner slide
					if (this.doubleInnerPatern != null)
						this.doubleInnerPatern(touches);
				}
			}
			else
			{
				if (v2)
				{
					// out slide
					if (this.doubleOuterPatern != null)
						this.doubleOuterPatern(touches);
				}
				else
				{
					// down slide
					if (this.doubleDownPatern != null)
						this.doubleDownPatern(touches);
				}
			}
		}
	}
	void simpleSlideAction(touchControl touchC, InputManager.imTouch[] touches)
	{
		if (Mathf.Abs(touchC.moveDelta.x) > Mathf.Abs(touchC.moveDelta.y))
		{
			if (touchC.moveDelta.x > 0)
			{
				// right simple
				if (this.simpleRightPatern != null)
					this.simpleRightPatern(touches);
			}
			else
			{
				// left simple
				if (this.simpleLeftPatern != null)
					this.simpleLeftPatern(touches);
			}
		}
		else
		{
			if (touchC.moveDelta.y > 0)
			{
				// top simple
				if (this.simpleTopPatern != null)
					this.simpleTopPatern(touches);
			}
			else
			{
				//down simple
				if (this.simpleDownPatern != null)
					this.simpleDownPatern(touches);
			}
		}
	}

	private void SortHitByDept(ref RaycastHit[] hits)
	{
		System.Collections.Generic.List<RaycastHit> hitList = new System.Collections.Generic.List<RaycastHit>();
		float z = this.cam.transform.position.z;

		hitList.AddRange(hits);
		hitList.Sort((r, r2) => { return (z - r.transform.position.z > z - r2.transform.position.z ? -1 : 1); });
		hits = hitList.ToArray();
	}

#if UNITY_EDITOR
	void OnGUI()
	{
		if (Input.touchCount > 0) // Permet de visualiser les touches sur un device tactile
		{
			int p = 50;
			GUI.Label(new Rect(0, 0, 250, 50), "NB TOUCH: " + Input.touchCount);
			for (int i = 0; i < Input.touchCount; i++)
			{
				GUI.Label(new Rect(0, p, 250, 50), "Touches[" + i + "] (" + Input.touches[i].position.x + "; " + Input.touches[i].position.y + ") id: " + Input.touches[i].fingerId);
				p += 50;
			}
		}
	}
#endif
}
