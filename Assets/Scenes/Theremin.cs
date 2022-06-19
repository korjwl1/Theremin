using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theremin : MonoBehaviour
{
    Camera Camera;

    AudioSource audioSource;
    int timeIndex = 0;

    public float sampleRate = 44100f;
    public float waveLengthInSeconds = 2.0f;

    [Range(1, 20000)]
    public float max_freq; // Hz ����

    [Range(1, 20000)]
    public float ref_freq; // Hz ����
    private float mod_freq;
    private float volume;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // ���콺 ������ ���� ��ġ ���ϴ� �ڵ�
        Vector2 MousePosition = Input.mousePosition;
        MousePosition = Camera.ScreenToWorldPoint(MousePosition);

        // ���콺�� ��Ŭ�� �ϴ� ���ȿ��� �����Ѵ�.
        if (Input.GetMouseButtonDown(0))
        {
            timeIndex = 0;
            audioSource.Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.Stop();
        }

        // ���� ���̸� ���ļ� �� ������ ��� ������Ʈ�Ѵ�.
        if (audioSource.isPlaying)
        {
            mod_freq = freq_antenna(freq_distance(MousePosition));
            volume = amp_antenna(amp_distance(MousePosition));
        }
    }

    // Audio ��� �Լ�
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i+=channels)
        {
            data[i] = volume * mixer();

            if (channels == 2)
            {
                data[i + 1] = volume * mixer();
            }

            timeIndex++;

            if (timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }

    // �Ÿ��� ���� ���ļ� �� ���
    float freq_antenna(float distance)
    {
        // ���� �������� ���� ���̴�
        // distance = 0�� �� max_freq -> y = max_freq - a*x
        float freq = max_freq - max_freq / 17.78f * distance;
        if (freq < 0) freq = 0;

        return freq;
    }

    float amp_antenna(float distance)
    {
        // ���� �������� ���� �����̴�
        // distance = 0�� �� volume = 0 -> max volume�� 1
        float v = distance / 10;
        if (v > 1f) v = 1f;

        return v;
    }

    float mixer()
    {
        return Mathf.Cos(2 * Mathf.PI * timeIndex * (mod_freq - ref_freq) / sampleRate);
    }

    float freq_distance(Vector2 mp)
    {
        // �Է±��� ������ ��ġ ���� �Ÿ� ���ϱ�
        // ���� �ٰ� Freq�� �ϰ� �ش� ��ġ�κ����� �Ÿ� ���ϱ�
        return Mathf.Abs(8.88f - mp.x);        
    }

    float amp_distance(Vector2 mp)
    {
        // �Է±��� ������ ��ġ ���� �Ÿ� ���ϱ�
        // ��� �ٰ� Amp
        return Mathf.Abs(5f - mp.y);
    }
}
