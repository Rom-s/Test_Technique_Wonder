using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    public enum View { Front, Left, Right };

    [SerializeField] private Transform helmetTransform;
    [SerializeField] [Min(0)] private float transitionDuration = 1f;
    [SerializeField] private Button[] buttons = new Button[3];
    [SerializeField] private RectTransform selectedSpriteTransform;

    private View _currentView;
    private View _destinationView;
    private float _timeTransition;
    private Vector3[] _viewRotations = new Vector3[3] { new(90, 0, 0), new(90, -90, 0), new(90, 90, 0) };

    // Start is called before the first frame update
    void Start()
    {
        _timeTransition = transitionDuration;
        _currentView = View.Front;
        helmetTransform.eulerAngles = _viewRotations[(int)View.Front];

        for (int i = 0; i < buttons.Length; i++)
        {
            int v = i;
            buttons[i].onClick.AddListener(() => RotateTo((View)v));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeTransition < transitionDuration)
        {
            helmetTransform.eulerAngles = Vector3.Lerp(_viewRotations[(int)_currentView], _viewRotations[(int)_destinationView], _timeTransition / transitionDuration);
            selectedSpriteTransform.localPosition = Vector3.Lerp(buttons[(int)_currentView].transform.localPosition, buttons[(int)_destinationView].transform.localPosition, _timeTransition / transitionDuration);
            _timeTransition = Mathf.Clamp(_timeTransition + Time.deltaTime, 0f, transitionDuration);
            if (_timeTransition == transitionDuration)
            {
                _currentView = _destinationView;
            }
        }
    }

    private void RotateTo(View view)
    {
        if (_timeTransition < transitionDuration || view == _currentView)
        {
            return;
        }
        _destinationView = view;
        _timeTransition = 0f;
    }
}
