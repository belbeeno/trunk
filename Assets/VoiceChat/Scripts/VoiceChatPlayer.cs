using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceChat
{
    public class VoiceChatPlayer : MonoBehaviour
    {
        public event Action PlayerStarted;
        public CardboardAudioSource source = null;
        public AudioSource oldSource = null;

        public AudioSource GetAudioSource()
        {
            if (source == null)
            {
                return oldSource;
            }
            return source.Audio;
        }

        float lastTime = 0;
        double played = 0;
        double received = 0;
        int index = 0;
        float[] data;
        float playDelay = 0;
        bool shouldPlay = false;
        float lastRecvTime = 0;
        NSpeex.SpeexDecoder speexDec = new NSpeex.SpeexDecoder(NSpeex.BandMode.Narrow);

        [SerializeField]
        [Range(.1f, 5f)]
        float playbackDelay = .5f;

        [SerializeField]
        [Range(1, 32)]
        int packetBufferSize = 10;

        SortedList<ulong, VoiceChatPacket> packetsToPlay = new SortedList<ulong, VoiceChatPacket>();

        public float LastRecvTime
        {
            get { return lastRecvTime; }
        }

        void Start()
        {
            int size = VoiceChatSettings.Instance.Frequency * 10;

            if (source != null)
            {
                source.loop = true;
                source.clip = AudioClip.Create("VoiceChat", size, 1, VoiceChatSettings.Instance.Frequency, false);
            }
            else
            {
                oldSource.loop = true;
                oldSource.clip = AudioClip.Create("VoiceChat", size, 1, VoiceChatSettings.Instance.Frequency, false);
            }
            data = new float[size];

            if (VoiceChatSettings.Instance.LocalDebug)
            {
                VoiceChatRecorder.Instance.NewSample += OnNewSample;
            }

            if(PlayerStarted != null)
            {
                PlayerStarted();
            }
        }

        void Update()
        {
            if ((source != null && source.isPlaying) 
                || (oldSource != null && oldSource.isPlaying))
            {
                float currTime = GetAudioSource().time;
                // Wrapped around
                if (lastTime > currTime)
                {
                    played += GetAudioSource().clip.length;
                }

                lastTime = currTime;

                // Check if we've played to far
                if (played + currTime >= received)
                {
                    Stop();
                    shouldPlay = false;
                }
            }
            else
            {
                if (shouldPlay)
                {
                    playDelay -= Time.deltaTime;

                    if (playDelay <= 0)
                    {
                        source.Play();
                    }
                }
            }
        }

        void Stop()
        {
            if (source != null)
            {
                source.Stop();
                source.Audio.time = 0;
            }
            if (oldSource != null)
            {
                oldSource.Stop();
                oldSource.time = 0;
            }
            index = 0;
            played = 0;
            received = 0;
            lastTime = 0;
        }

        public void OnNewSample(VoiceChatPacket newPacket)
        {
            // Set last time we got something
            lastRecvTime = Time.time;

            if (packetsToPlay.ContainsKey(newPacket.PacketId))
            {
                return;
            }

            packetsToPlay.Add(newPacket.PacketId, newPacket);

            if (packetsToPlay.Count < packetBufferSize)
            {
                return;
            }

            var pair = packetsToPlay.First();
            var packet = pair.Value;
            packetsToPlay.Remove(pair.Key);

            // Decompress
            float[] sample = null;
            int length = VoiceChatUtils.Decompress(speexDec, packet, out sample);

            // Add more time to received
            received += VoiceChatSettings.Instance.SampleTime;

            // Push data to buffer
            Array.Copy(sample, 0, data, index, length);

            // Increase index
            index += length;

            // Handle wrap-around
            if (index >= (source == null ? source.clip.samples : oldSource.clip.samples))
            {
                index = 0;
            }

            // Set data
            if (source != null)
                source.clip.SetData(data, 0);
            else
                oldSource.clip.SetData(data, 0);
            
            // If we're not playing
            if (!GetAudioSource().isPlaying)
            {
                // Set that we should be playing
                shouldPlay = true;

                // And if we have no delay set, set it.
                if (playDelay <= 0)
                {
                    playDelay = (float)VoiceChatSettings.Instance.SampleTime * playbackDelay;
                }
            }

            VoiceChatFloatPool.Instance.Return(sample);
        }
    } 
}