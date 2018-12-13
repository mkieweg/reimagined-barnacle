﻿using UnityEngine;

namespace _Scripts
{
    public class Audio : MonoBehaviour
    {
        private AudioSource _audioSource;
        private float[] _samplesLeft = new float[512];
        private float[] _samplesRight = new float[512];
        private float[] _frequencyBand = new float[8];
        private float[] _bandBuffer = new float[8];
        public static float AverageFrequency;
        private float[] _bufferDecrease = new float[8];
        
        private float[] _freqBandHighest = new float[8];
        public static float[] AudioBand = new float[8];
        public static float[] AudioBandBuffer = new float[8];
        
        public int sampleRate;

        // Use this for initialization
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            GetSpectrumData();
            MakeFrequencyBand();
            BandBufferMethod();
            CreateAudioBands();
            sampleRate = AudioSettings.outputSampleRate;
        }

        void GetSpectrumData()
        {
            _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.BlackmanHarris);
            _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.BlackmanHarris);
        }

        void BandBufferMethod()
        {
            for (int i = 0; i < 8; i++)
            {
                if (_frequencyBand[i] > _bandBuffer[i])
                {
                    _bandBuffer[i] = _frequencyBand[i];
                    _bufferDecrease[i] = 0.005f;
                }

                if (_frequencyBand[i] < _bandBuffer[i])
                {
                    _bandBuffer[i] -= _bufferDecrease[i];
                    _bufferDecrease[i] *= 1.2f;
                }
            }
        }

        void MakeFrequencyBand()
        {
            int count = 0;

            for (int i = 0; i < 8; i++)
            {
                float average = 0;
                int sampleCount = (int) Mathf.Pow(2, i) * 2;
                if (i == 7)
                {
                    sampleCount += 2;
                }

                for (int j = 0; j < sampleCount; j++)
                {
                    average += _samplesLeft[count] + _samplesRight[count] * (count + 1);
                    count++;
                }

                average /= count;

                _frequencyBand[i] = average * 10;
            }
        }

        void CalculateAverageFrequency()
        {
        }

        void CreateAudioBands()
        {
            for (int i = 0; i < 8; i++)
            {
                if (_frequencyBand[i] > _freqBandHighest[i])
                {
                    _freqBandHighest[i] = _frequencyBand[i];
                }

                AudioBand[i] = _frequencyBand[i] / _freqBandHighest[i];
                AudioBandBuffer[i] = _bandBuffer[i] / _freqBandHighest[i];
            }
        }
    }
}