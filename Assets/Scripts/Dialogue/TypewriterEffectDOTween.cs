using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class TypewriterEffectDOTween : MonoBehaviour
{
    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    private TMP_Text textComponent;
    private string fullText;
    private bool isTyping = false;
    private List<EffectWord> effectWords = new List<EffectWord>();

    [Header("Typing Sound")]
    [SerializeField] private AudioClip typingSound;
    private AudioSource audioSource;

    [SerializeField] private float minSoundDelay = 0.05f;
    [SerializeField] private float maxSoundDelay = 0.1f;
    private float lastSoundTime;

    [Header("Pitch Variation")]
    [SerializeField] private float minPitch = 0.4f;
    [SerializeField] private float maxPitch = 0.8f;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        audioSource = GetComponentInParent<AudioSource>();
        audioSource.pitch = 1f;
    }

    // Starts the typewriter effect on a string of text
    public void StartTyping(string text)
    {
        lastSoundTime = Time.time;
        effectWords.Clear();
        fullText = ParseText(text);
        textComponent.text = "";
        isTyping = true;
        StartCoroutine(TypeTextWithEffects());
    }

    // Parses effect tags like $w( ... ) or $s( ... ) and stores their positions
    private string ParseText(string text)
    {
        var effectStack = new List<(TextEffect, int)>();
        string cleanedText = "";
        int charIndex = 0;
        bool insideEffect = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '$' && i + 2 < text.Length)
            {
                if (text[i + 1] == 'w' && text[i + 2] == '(')
                {
                    effectStack.Add((TextEffect.Wiggle, charIndex));
                    i += 2;
                    insideEffect = true;
                    continue;
                }
                else if (text[i + 1] == 's' && text[i + 2] == '(')
                {
                    effectStack.Add((TextEffect.Shake, charIndex));
                    i += 2;
                    insideEffect = true;
                    continue;
                }
            }
            else if (text[i] == ')' && insideEffect)
            {
                if (effectStack.Count > 0)
                {
                    var last = effectStack[^1];
                    effectWords.Add(new EffectWord(last.Item1, last.Item2, charIndex - last.Item2));
                    effectStack.RemoveAt(effectStack.Count - 1);
                }
                insideEffect = effectStack.Count > 0;
                continue;
            }

            cleanedText += text[i];
            charIndex++;
        }

        return cleanedText;
    }

    // Handles showing the text letter by letter and applying effects and sounds
    private IEnumerator TypeTextWithEffects()
    {
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.maxVisibleCharacters++;

            foreach (var word in effectWords)
            {
                if (i >= word.startIndex && i < word.startIndex + word.length)
                {
                    StartCoroutine(ApplyWordEffect(word));
                }
            }

            if (audioSource && typingSound && Time.time - lastSoundTime >= Random.Range(minSoundDelay, maxSoundDelay))
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(typingSound);
                lastSoundTime = Time.time;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // Applies the effect on a word repeatedly (wiggle or shake)
    private IEnumerator ApplyWordEffect(EffectWord word)
    {
        while (true)
        {
            if (word.effect == TextEffect.Wiggle) WiggleWord(word.startIndex, word.length);
            if (word.effect == TextEffect.Shake) ShakeWord(word.startIndex, word.length);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Makes the word bounce up and down like a wave
    private void WiggleWord(int startIndex, int length)
    {
        TMP_TextInfo info = textComponent.textInfo;
        textComponent.ForceMeshUpdate();

        for (int i = startIndex; i < startIndex + length; i++)
        {
            if (!info.characterInfo[i].isVisible) continue;

            int v = info.characterInfo[i].vertexIndex;
            Vector3[] verts = info.meshInfo[info.characterInfo[i].materialReferenceIndex].vertices;

            float wave = Mathf.Sin(Time.time * 10f + i) * 2f;
            for (int j = 0; j < 4; j++) verts[v + j] += new Vector3(0, wave, 0);
        }

        textComponent.UpdateVertexData();
    }

    // Shakes the word randomly in 2D space
    private void ShakeWord(int startIndex, int length)
    {
        TMP_TextInfo info = textComponent.textInfo;
        textComponent.ForceMeshUpdate();

        for (int i = startIndex; i < startIndex + length; i++)
        {
            if (!info.characterInfo[i].isVisible) continue;

            int v = info.characterInfo[i].vertexIndex;
            Vector3[] verts = info.meshInfo[info.characterInfo[i].materialReferenceIndex].vertices;

            float shake = Random.Range(-2f, 2f);
            for (int j = 0; j < 4; j++) verts[v + j] += new Vector3(shake, shake, 0);
        }

        textComponent.UpdateVertexData();
    }

    // Skips the typing animation and reveals all text instantly
    public void SkipTyping()
    {
        StopAllCoroutines();
        isTyping = false;
        textComponent.maxVisibleCharacters = fullText.Length;

        foreach (var word in effectWords)
        {
            StartCoroutine(ApplyWordEffect(word));
        }
    }

    // Returns whether text is still typing
    public bool IsTyping()
    {
        return isTyping;
    }
}

// Defines which visual effect to use
public enum TextEffect
{
    None,
    Wiggle,
    Shake
}

// Represents a section of text that should have an effect applied
public class EffectWord
{
    public TextEffect effect;
    public int startIndex;
    public int length;

    public EffectWord(TextEffect effect, int startIndex, int length)
    {
        this.effect = effect;
        this.startIndex = startIndex;
        this.length = length;
    }
}
