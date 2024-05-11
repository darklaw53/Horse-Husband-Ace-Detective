using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    TMP_Text _textBox;
    string _previousText;

    Coroutine _typewriterCoroutine;

    WaitForSeconds _simpleDelay;
    WaitForSeconds _speedUpDelay;
    WaitForSeconds _interpunctuationDelay;

    [SerializeField] float charactersPerSecond = 20;
    [SerializeField] float speedMult = 5;
    [SerializeField] float interpunctuationDelay = .5f;

    int _currentVisibleCharacterIndex;

    public bool _speedUpRequested = false;
    public bool _typewriterFinished = false;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
        _textBox.maxVisibleCharacters = 0;

        _simpleDelay = new WaitForSeconds(1 / charactersPerSecond);
        _speedUpDelay = new WaitForSeconds(1 / (charactersPerSecond * speedMult));
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);
    }

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void Update()
    {
        // Check for mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            if (_typewriterFinished)
            {
                // If typewriter finished, execute next line
                DialogueManager.Instance.NextLine();
            }
            else if (_speedUpRequested)
            {
                // If speed-up already requested, instantly finish the process
                _currentVisibleCharacterIndex = _textBox.text.Length;
                _textBox.maxVisibleCharacters = _currentVisibleCharacterIndex + 1;
                _speedUpRequested = false; // Reset the flag
            }
            else
            {
                // If speed-up not requested, request it
                _speedUpRequested = true;
            }
        }
    }

    private void OnTextChanged(UnityEngine.Object obj)
    {
        if (_textBox.text != _previousText)
        {
            _previousText = _textBox.text;
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
            }
            _typewriterCoroutine = StartCoroutine(Typewriter());
        }
    }

    IEnumerator Typewriter()
    {
        _typewriterFinished = false;
        _currentVisibleCharacterIndex = 0;

        while (_currentVisibleCharacterIndex < _textBox.text.Length)
        {
            char character = _textBox.text[_currentVisibleCharacterIndex];
            _textBox.maxVisibleCharacters = _currentVisibleCharacterIndex + 1;

            if (character == '?' || character == '.' || character == ',' || character == ':' ||
                character == ';' || character == '!' || character == '-')
            {
                yield return _interpunctuationDelay;
            }
            else
            {
                // Check if speed-up requested, if not use normal delay
                yield return _speedUpRequested ? _speedUpDelay : _simpleDelay; // Adjust the speed multiplier as needed
            }

            _currentVisibleCharacterIndex++;
        }

        // Typewriter finished
        _speedUpRequested = false;
        _typewriterFinished = true;

        // Check if the line is marked as quick, if true, execute next line immediately
        if (DialogueManager.Instance.currentDialogueNode.lines[DialogueManager.Instance.currentLine].quick)
        {
            DialogueManager.Instance.NextLine();
        }
    }
}