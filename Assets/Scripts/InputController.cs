using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Settings")]
    [SerializeField, Range(1, 20)] 
    private float radioDistance = 5;//the ratio of the circumference of the joystick
    [SerializeField, Range(0.01f, 1)] 
    private float SmoothTime = 0.5f;//return to default position speed
    [SerializeField, Range(0.5f, 4)] 
    private float OnPressScale = 1.5f;//return to default position speed
    public Color NormalColor = new Color(1, 1, 1, 1);
    public Color PressColor = new Color(1, 1, 1, 1);
    [SerializeField, Range(0.1f, 5)] 
    private float Duration = 1;

    [Header("Reference")]
    [SerializeField] private RectTransform stickRect;//The middle joystick UI
    [SerializeField] private RectTransform centerReference;

    private Vector3 deathArea;
    private Vector3 currentVelocity;
    private bool isFree = false;
    private int lastId = -2;
    private Canvas canvas;
    private Image stickImage;
    private Image backImage;
    private float diff;
    private Vector3 PressScaleVector;

    private BallController ballController;

    void Start()
    {
        ballController = FindObjectOfType<BallController>();

        canvas = transform.root.GetComponent<Canvas>();

        deathArea = centerReference.position;
        diff = centerReference.position.magnitude;

        PressScaleVector = new Vector3(OnPressScale, OnPressScale, OnPressScale);
        if (GetComponent<Image>() != null)
        {
            backImage = GetComponent<Image>();
            stickImage = stickRect.GetComponent<Image>();
            backImage.CrossFadeColor(NormalColor, 0.1f, true, true);
            stickImage.CrossFadeColor(NormalColor, 0.1f, true, true);
        }
    }

    void Update()
    {
        if (!isFree)
            return;

        //Return to default position with a smooth movement
        stickRect.position = Vector3.SmoothDamp(stickRect.position, deathArea, ref currentVelocity, 0.5f);

        //When is in default position, we not need continue update this
        if (Vector3.Distance(stickRect.position, deathArea) < .1f)
        {
            isFree = false;
            stickRect.position = deathArea;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        ballController.CreateProjectile();

        //Detect if is the default touchID
        if (lastId == -2)
        {
            //then get the current id of the current touch.
            //this for avoid that other touch can take effect in the drag position event.
            //we only need get the position of this touch
            lastId = data.pointerId;
            StopAllCoroutines();
            StartCoroutine(ScaleJoysctick(true));
            OnDrag(data);
            if (backImage != null)
            {
                backImage.CrossFadeColor(PressColor, Duration, true, true);
                stickImage.CrossFadeColor(PressColor, Duration, true, true);
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        //If this touch id is the first touch in the event
        if (data.pointerId == lastId)
        {
            isFree = false;
            //Get Position of current touch
            Vector3 position = bl_JoystickUtils.TouchPosition(canvas, GetTouchID);

			//Rotate into the area circumferential of joystick
			if (Vector2.Distance(deathArea, position) < newrRadio)
			{
				stickRect.position = position;
			}
			else
			{
				stickRect.position = deathArea + (position - deathArea).normalized * newrRadio;
			}
		}
    }

    public void OnPointerUp(PointerEventData data)
    {
        ballController.Shoot();

        isFree = true;
        currentVelocity = Vector3.zero;
        //leave the default id again
        if (data.pointerId == lastId)
        {
            //-2 due -1 is the first touch id
            lastId = -2;
            StopAllCoroutines();
            StartCoroutine(ScaleJoysctick(false));
            if (backImage != null)
            {
                backImage.CrossFadeColor(NormalColor, Duration, true, true);
                stickImage.CrossFadeColor(NormalColor, Duration, true, true);
            }
        }
    }

    IEnumerator ScaleJoysctick(bool increase)
    {
        float _time = 0;

        while (_time < Duration)
        {
            Vector3 v = stickRect.localScale;
            if (increase)
            {
                v = Vector3.Lerp(stickRect.localScale, PressScaleVector, (_time / Duration));
            }
            else
            {
                v = Vector3.Lerp(stickRect.localScale, Vector3.one, (_time / Duration));
            }
            stickRect.localScale = v;
            _time += Time.deltaTime;
            yield return null;
        }
    }

    private float newrRadio { get { return (radioDistance * 5 + Mathf.Abs((diff - centerReference.position.magnitude))); } }

    public int GetTouchID
    {
        get
        {
            //find in all touches
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == lastId)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public float Horizontal
    {
        get
        {
            return (stickRect.position.x - deathArea.x) / radioDistance;
        }
    }

    public float Vertical
    {
        get
        {
            return (stickRect.position.y - deathArea.y) / radioDistance;
        }
    }
}

public static class bl_JoystickUtils
{

    public static Vector3 TouchPosition(this Canvas _Canvas, int touchID)
    {
        Vector3 Return = Vector3.zero;

        if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
            Return = Input.GetTouch(touchID).position;
#else
            Return = Input.mousePosition;
#endif
        }
        else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 tempVector = Vector2.zero;
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
           Vector3 pos = Input.GetTouch(touchID).position;
#else
            Vector3 pos = Input.mousePosition;
#endif
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_Canvas.transform as RectTransform, pos, _Canvas.worldCamera, out tempVector);
            Return = _Canvas.transform.TransformPoint(tempVector);
        }

        return Return;
    }
}
