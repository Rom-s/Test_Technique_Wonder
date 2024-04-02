using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private Color _initialSymbolColor;
    private Image[] _buttonSymbols;
    private TMP_Text[] _buttonTexts;

    // Start is called before the first frame update
    void Start()
    {
        // Setting _timeTransiton to transitionDuration will prevent view transition 
        _timeTransition = transitionDuration;
        _currentView = View.Front;
        helmetTransform.eulerAngles = _viewRotations[(int)View.Front];

        _buttonSymbols = new Image[buttons.Length];
        _buttonTexts = new TMP_Text[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            int v = i;
            buttons[i].onClick.AddListener(() => RotateTo((View)v));
            _buttonSymbols[i] = buttons[i].GetComponentsInChildren<Image>()[1];
            _buttonTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
            _buttonTexts[i].gameObject.SetActive(i == (int)View.Front);
            
        }
        _initialSymbolColor = _buttonSymbols[0].color;
        _buttonSymbols[(int)View.Front].color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        // Make transition between views: transition concerns the helmet rotation, button's texts, button's symbols and the purple sprite position
        if (_timeTransition < transitionDuration)
        {
            _timeTransition = Mathf.Clamp(_timeTransition + Time.deltaTime, 0f, transitionDuration);
            
            float lerpFactor = _timeTransition / transitionDuration;
            helmetTransform.eulerAngles = Vector3.Lerp(_viewRotations[(int)_currentView], _viewRotations[(int)_destinationView], lerpFactor);
            selectedSpriteTransform.localPosition = Vector3.Lerp(buttons[(int)_currentView].transform.localPosition, buttons[(int)_destinationView].transform.localPosition, lerpFactor);
            _buttonSymbols[(int)_currentView].color = Color.Lerp(Color.white, _initialSymbolColor, lerpFactor);
            _buttonSymbols[(int)_destinationView].color = Color.Lerp(_initialSymbolColor, Color.white, lerpFactor);

            if (_timeTransition == transitionDuration)
            {
                _buttonTexts[(int)_currentView].gameObject.SetActive(false);
                _currentView = _destinationView;
            }
        }
    }

    /// <summary>
    /// Rotate the helmet to the indicated view
    /// </summary>
    /// <param name="view"></param>
    private void RotateTo(View view)
    {
        // Transition is prevented if it is already in progress or if the requested view is the current view 
        if (_timeTransition < transitionDuration || view == _currentView)
        {
            return;
        }
        buttons[0].GetComponentsInParent<HorizontalLayoutGroup>()[1].enabled = false;

        _buttonTexts[(int)view].gameObject.SetActive(true);

        _destinationView = view;

        // Setting _timeTransiton to 0 will trigger view transition 
        _timeTransition = 0f;
    }
}
