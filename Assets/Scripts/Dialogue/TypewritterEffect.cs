using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    TMP_Text _textBox;
    string _currentText;

    Coroutine _typewriterCoroutine;

    WaitForSeconds _simpleDelay;
    WaitForSeconds _speedUpDelay;
    WaitForSeconds _interpunctuationDelay;

    [SerializeField] float charactersPerSecond = 20f;
    [SerializeField] float speedMult = 5f;
    [SerializeField] float interpunctuationDelay = 0.5f;

    int _currentVisibleCharacterIndex;

    bool _speedUpRequested = false;
    public bool TypewriterFinished { get; private set; } = false;

    public UnityEvent OnTypingFinished;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
        _textBox.maxVisibleCharacters = 0;

        _simpleDelay = new WaitForSeconds(1f / charactersPerSecond);
        _speedUpDelay = new WaitForSeconds(1f / (charactersPerSecond * speedMult));
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);

        // Start tracking text changes
        _currentText = _textBox.text;
        if (!string.IsNullOrEmpty(_currentText))
        {
            StartTyping(_currentText);
        }
    }

    private void Update()
    {
        // Detect text change automatically
        if (_textBox.text != _currentText)
        {
            StartTyping(_textBox.text);
        }

        // Handle input skipping/speedup
        if (Input.GetMouseButtonDown(0))
        {
            if (TypewriterFinished)
            {
                DialogueManager.Instance.OnFinishedTyping();
            }
            else if (_speedUpRequested)
            {
                _currentVisibleCharacterIndex = _currentText.Length;
                _textBox.maxVisibleCharacters = _currentVisibleCharacterIndex;
                _speedUpRequested = false;
            }
            else
            {
                _speedUpRequested = true;
            }
        }
    }

    public void StartTyping(string fullText)
    {
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }

        _currentText = fullText;
        _textBox.text = fullText;
        _textBox.maxVisibleCharacters = 0;
        _speedUpRequested = false;
        TypewriterFinished = false;

        if (!string.IsNullOrEmpty(_currentText))
        {
            _typewriterCoroutine = StartCoroutine(Typewriter());
        }
    }

    IEnumerator Typewriter()
    {
        _currentVisibleCharacterIndex = 0;
        string interpunctuationChars = "?,.:;!-";

        while (_currentVisibleCharacterIndex < _currentText.Length)
        {
            char character = _currentText[_currentVisibleCharacterIndex];
            _textBox.maxVisibleCharacters = _currentVisibleCharacterIndex + 1;

            if (interpunctuationChars.Contains(character))
            {
                yield return _interpunctuationDelay;
            }
            else
            {
                yield return _speedUpRequested ? _speedUpDelay : _simpleDelay;
            }

            _currentVisibleCharacterIndex++;
        }

        _speedUpRequested = false;
        TypewriterFinished = true;

        OnTypingFinished?.Invoke();
    }
}