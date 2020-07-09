using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SteeringWheel : MonoBehaviour
{
    private static SteeringWheel _instance;
    public static SteeringWheel Instance { get { return _instance; } }

    private Graphic _UI_Element;
    private RectTransform _rectT;

    private Vector2 _centerPoint;

    private readonly float _maximumSteeringAngle = 200f;
    private readonly float _wheelReleasedSpeed = 200f;

    private float _wheelAngle = 0f;
    float _wheelPrevAngle = 0f;

    private bool _wheelBeingHeld = false;

    void Awake()
    {
        SetAsSingleton();
    }

    void Start()
    {
        _UI_Element = GameObject.FindWithTag("steeringWheel").GetComponent<Image>();
        _rectT = _UI_Element.rectTransform;

        InitEventsSystem();
        UpdateRect();

        MainController.Instance.SendUDPData(Constants.STEERING_PREFIX + Constants.DEFAULT_STEERING_ANGLE.ToString());
    }

    void Update()
    {
        if (!_wheelBeingHeld && !Mathf.Approximately(0f, _wheelAngle))
        {
            float deltaAngle = _wheelReleasedSpeed * Time.deltaTime;
            if (Mathf.Abs(deltaAngle) > Mathf.Abs(_wheelAngle))
            {
                _wheelAngle = 0f;
            }
            else if (_wheelAngle > 0f)
            {
                _wheelAngle -= deltaAngle;
                float interpolatedValue = Constants.DEFAULT_STEERING_ANGLE - _wheelAngle;

                MainController.Instance.SendUDPData(Constants.STEERING_PREFIX + ((int)interpolatedValue).ToString());

            }
            else
            {
                _wheelAngle += deltaAngle;
                float interpolatedValue = Constants.DEFAULT_STEERING_ANGLE + (-_wheelAngle);

                MainController.Instance.SendUDPData(Constants.STEERING_PREFIX + ((int)interpolatedValue).ToString());

            }
        }

        _rectT.localEulerAngles = Vector3.back * _wheelAngle;
    }

    #region PUBLIC_METHODS
    public float GetClampedValue()
    {
        return _wheelAngle / _maximumSteeringAngle;
    }

    public float GetAngle()
    {
        return _wheelAngle;
    }
    #endregion

    #region PRIVATE_METHODS
    private void InitEventsSystem()
    {
        EventTrigger _events = _UI_Element.gameObject.GetComponent<EventTrigger>();

        if (_events == null)
            _events = _UI_Element.gameObject.AddComponent<EventTrigger>();

        if (_events.triggers == null)
            _events.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        EventTrigger.Entry _entry = new EventTrigger.Entry();
        EventTrigger.TriggerEvent _callback = new EventTrigger.TriggerEvent();
        UnityAction<BaseEventData> _functionCall = new UnityAction<BaseEventData>(PressEvent);
        _callback.AddListener(_functionCall);
        _entry.eventID = EventTriggerType.PointerDown;
        _entry.callback = _callback;

        _events.triggers.Add(_entry);

        _entry = new EventTrigger.Entry();
        _callback = new EventTrigger.TriggerEvent();
        _functionCall = new UnityAction<BaseEventData>(DragEvent);
        _callback.AddListener(_functionCall);
        _entry.eventID = EventTriggerType.Drag;
        _entry.callback = _callback;

        _events.triggers.Add(_entry);

        _entry = new EventTrigger.Entry();
        _callback = new EventTrigger.TriggerEvent();
        _functionCall = new UnityAction<BaseEventData>(ReleaseEvent);//
        _callback.AddListener(_functionCall);
        _entry.eventID = EventTriggerType.PointerUp;
        _entry.callback = _callback;

        _events.triggers.Add(_entry);
    }

    private void UpdateRect()
    {
        Vector3[] _corners = new Vector3[4];
        _rectT.GetWorldCorners(_corners);

        for (int i = 0; i < 4; i++)
            _corners[i] = RectTransformUtility.WorldToScreenPoint(null, _corners[i]);

        Vector3 _bottomLeft = _corners[0];
        Vector3 _topRight = _corners[2];
        float _width = _topRight.x - _bottomLeft.x;
        float _height = _topRight.y - _bottomLeft.y;

        Rect _rect = new Rect(_bottomLeft.x, _topRight.y, _width, _height);
        _centerPoint = new Vector2(_rect.x + _rect.width * 0.5f, _rect.y - _rect.height * 0.5f);
    }

    private void PressEvent(BaseEventData eventData)
    {
        Vector2 _pointerPos = ((PointerEventData)eventData).position;

        _wheelBeingHeld = true;
        _wheelPrevAngle = Vector2.Angle(Vector2.up, _pointerPos - _centerPoint);
    }

    private void DragEvent(BaseEventData eventData)
    {
        Vector2 _pointerPos = ((PointerEventData)eventData).position;
        float _wheelNewAngle = Vector2.Angle(Vector2.up, _pointerPos - _centerPoint);

        if (Vector2.Distance(_pointerPos, _centerPoint) > 20f)
        {
            if (_pointerPos.x > _centerPoint.x)
                _wheelAngle += _wheelNewAngle - _wheelPrevAngle;
            else
                _wheelAngle -= _wheelNewAngle - _wheelPrevAngle;
        }

        _wheelAngle = Mathf.Clamp(_wheelAngle, -_maximumSteeringAngle, _maximumSteeringAngle);
        _wheelPrevAngle = _wheelNewAngle;
        float interpolatedValue = _wheelAngle < 0 ? Constants.DEFAULT_STEERING_ANGLE + (-_wheelAngle) : Constants.DEFAULT_STEERING_ANGLE - _wheelAngle;
        int response = (int)interpolatedValue;

        MainController.Instance.SendUDPData(Constants.STEERING_PREFIX + response.ToString());
    }

    private void ReleaseEvent(BaseEventData eventData)
    {
        DragEvent(eventData);
        _wheelBeingHeld = false;
    }

    private void SetAsSingleton()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
    #endregion
}