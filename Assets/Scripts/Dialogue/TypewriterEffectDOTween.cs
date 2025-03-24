using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypewriterEffectDOTween : MonoBehaviour
{
    [Header("Scrolling Text")]
    public float typingSpeed = 0.03f;
    private TMP_Text textComponent;
    private string fullText;
    private bool isTyping = false;
    private List<EffectWord> effectWords = new List<EffectWord>();

    [Header("Sound Clips")]
    [SerializeField]
    private AudioClip typingSound;
    private AudioSource audioSource;

    [SerializeField] private float minSoundDelay = 0.1f;
    [SerializeField] private float maxSoundDelay = 0.3f;

    private float lastSoundTime;

    [Header("Pitch Variation")]
    [SerializeField] private float minPitch = 0.8f;  // Minimum pitch
    [SerializeField] private float maxPitch = 1.2f;  // Maximum pitch


    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        audioSource = GetComponentInParent<AudioSource>();
        audioSource.pitch = 1f;
    }

    public void StartTyping(string text)
    {
        lastSoundTime = Time.time;
        effectWords.Clear();
        fullText = ParseText(text);  // Clean text and store effect positions
        textComponent.text = "";
        isTyping = true;
        StartCoroutine(TypeTextWithEffects());
    }

    private string ParseText(string text)
    {
        List<(TextEffect effect, int startIndex)> effectStack = new List<(TextEffect, int)>();
        string cleanedText = "";
        int charIndex = 0;
        bool insideEffect = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '$' && i + 1 < text.Length)
            {
                if (text[i + 1] == 'w' && i + 2 < text.Length && text[i + 2] == '(')
                {
                    effectStack.Add((TextEffect.Wiggle, charIndex));
                    i += 2; // Skip `$w(`
                    insideEffect = true;
                    continue;
                }
                else if (text[i + 1] == 's' && i + 2 < text.Length && text[i + 2] == '(')
                {
                    effectStack.Add((TextEffect.Shake, charIndex));
                    i += 2; // Skip `$s(`
                    insideEffect = true;
                    continue;
                }
            }
            else if (text[i] == ')' && insideEffect)
            {
                if (effectStack.Count > 0)
                {
                    var lastEffect = effectStack[effectStack.Count - 1];
                    effectWords.Add(new EffectWord(lastEffect.effect, lastEffect.startIndex, charIndex - lastEffect.startIndex));
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

    private IEnumerator TypeTextWithEffects()
    {
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.maxVisibleCharacters++;

            foreach (var effectWord in effectWords)
            {
                if (i >= effectWord.startIndex && i < effectWord.startIndex + effectWord.length)
                {
                    StartCoroutine(ApplyWordEffect(effectWord));
                }
            }

            if (audioSource && typingSound)
            {
                if (Time.time - lastSoundTime >= Random.Range(minSoundDelay, maxSoundDelay))
                {
                    audioSource.pitch = Random.Range(minPitch, maxPitch);

                    audioSource.PlayOneShot(typingSound);
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private IEnumerator ApplyWordEffect(EffectWord effectWord)
    {
        while (true)
        {
            switch (effectWord.effect)
            {
                case TextEffect.Wiggle:
                    WiggleWord(effectWord.startIndex, effectWord.length);
                    break;
                case TextEffect.Shake:
                    ShakeWord(effectWord.startIndex, effectWord.length);
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void WiggleWord(int startIndex, int length)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        textComponent.ForceMeshUpdate();

        for (int i = startIndex; i < startIndex + length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] verts = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;

            float wave = Mathf.Sin(Time.time * 10f + i) * 2f;
            verts[vertexIndex + 0] += new Vector3(0, wave, 0);
            verts[vertexIndex + 1] += new Vector3(0, wave, 0);
            verts[vertexIndex + 2] += new Vector3(0, wave, 0);
            verts[vertexIndex + 3] += new Vector3(0, wave, 0);
        }

        textComponent.UpdateVertexData();
    }

    private void ShakeWord(int startIndex, int length)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        textComponent.ForceMeshUpdate();

        for (int i = startIndex; i < startIndex + length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] verts = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;

            float shake = Random.Range(-2f, 2f);
            verts[vertexIndex + 0] += new Vector3(shake, shake, 0);
            verts[vertexIndex + 1] += new Vector3(shake, shake, 0);
            verts[vertexIndex + 2] += new Vector3(shake, shake, 0);
            verts[vertexIndex + 3] += new Vector3(shake, shake, 0);
        }

        textComponent.UpdateVertexData();
    }

    public void SkipTyping()
    {
        StopAllCoroutines();
        isTyping = false;

        // Reveal full text
        textComponent.maxVisibleCharacters = fullText.Length;

        // Apply all effects immediately
        foreach (var effectWord in effectWords)
        {
            StartCoroutine(ApplyWordEffect(effectWord));
        }
    }

    private void ApplyWordEffectInstantly(EffectWord effectWord)
    {
        switch (effectWord.effect)
        {
            case TextEffect.Wiggle:
                WiggleWord(effectWord.startIndex, effectWord.length); // Apply wiggle 
                break;
            case TextEffect.Shake:
                ShakeWord(effectWord.startIndex, effectWord.length); // Apply shake 
                break;
        }
    }


    public bool IsTyping()
    {
        return isTyping;
    }
}

public enum TextEffect
{
    None,
    Wiggle,
    Shake
}

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
