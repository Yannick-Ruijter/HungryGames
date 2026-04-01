using System.Collections;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    [SerializeField] private TMP_Text _countDownText;
    [SerializeField] private GameState _gameState;
    [SerializeField] private float _textSize = 50;

    private float _bounceVal = 0f;

    bool _bounce = false;
    bool _toChange = false;
    string _text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState.onCountDownTextUpdate.AddListener(SwapNumber);

    }

    // Update is called once per frame
    void Update()
    {
        if(_bounce)
        {
            _bounceVal += Time.deltaTime * 4;
            float scale = Mathf.Clamp( Mathf.Sin(_bounceVal), 0, 1) * _textSize;
            
            _countDownText.transform.localScale = new Vector3(scale, scale, scale);

            if(_bounceVal > Mathf.PI)
            {
                //_bounceVal = -Mathf.PI / 4;
                _bounce = false;
            }
        }
    }

    void SwapNumber(string text)
    {
        _bounceVal = 0;
        _countDownText.SetText(text);
        // we want it to first become small, then set text
        _bounce = true;
        // then make text big
        Debug.Log(text);
    }


}
