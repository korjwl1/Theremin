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
    public float max_freq; // Hz 단위

    [Range(1, 20000)]
    public float ref_freq; // Hz 단위
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
        // 마우스 포인터 현재 위치 구하는 코드
        Vector2 MousePosition = Input.mousePosition;
        MousePosition = Camera.ScreenToWorldPoint(MousePosition);

        // 마우스를 좌클릭 하는 동안에만 동작한다.
        if (Input.GetMouseButtonDown(0))
        {
            timeIndex = 0;
            audioSource.Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.Stop();
        }

        // 동작 중이면 주파수 및 볼륨을 계속 업데이트한다.
        if (audioSource.isPlaying)
        {
            mod_freq = freq_antenna(freq_distance(MousePosition));
            volume = amp_antenna(amp_distance(MousePosition));
        }
    }

    // Audio 재생 함수
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

    // 거리에 따른 주파수 값 계산
    float freq_antenna(float distance)
    {
        // 손이 가까울수록 높은 음이다
        // distance = 0일 때 max_freq -> y = max_freq - a*x
        float freq = max_freq - max_freq / 17.78f * distance;
        if (freq < 0) freq = 0;

        return freq;
    }

    float amp_antenna(float distance)
    {
        // 손이 가까울수록 낮은 볼륨이다
        // distance = 0일 때 volume = 0 -> max volume은 1
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
        // 입력기기와 설정된 위치 사이 거리 구하기
        // 우측 바가 Freq로 하고 해당 위치로부터의 거리 구하기
        return Mathf.Abs(8.88f - mp.x);        
    }

    float amp_distance(Vector2 mp)
    {
        // 입력기기와 설정된 위치 사이 거리 구하기
        // 상단 바가 Amp
        return Mathf.Abs(5f - mp.y);
    }
}
