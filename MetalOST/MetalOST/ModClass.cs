using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UObject = UnityEngine.Object;
using WavLib;
using UnityEngine.Rendering;
using System.Diagnostics;

namespace MetalOST
{
    public class MetalOST : Mod
    {
        private readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private readonly Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();
        private readonly string audiolocation = "MetalOST.Resources.AudioFiles.";

        public MetalOST() : base("Hollow knight but metal")
        {
            On.AudioManager.BeginApplyMusicCue += OnAudioManagerBeginApplyMusicCue;
        }
        public override string GetVersion() => "0.1";
        internal static MetalOST Instance;


        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            foreach (string audiofilelocation in assembly.GetManifestResourceNames())
            {
                if (audiofilelocation.StartsWith(audiolocation))
                {
                    string name = audiofilelocation.Substring(audiolocation.Length);
                    name = name.Replace(".wav", "");
                    Log("initial name =" + name);
                    AudioClip clip = GetAudioClip(name);
                    if (clip != null)
                        audioCache.Add(name, clip);
                    else
                    {
                        Log("ERROR WITH INITIALIZING AUDIOFILES");
                    }
                }
            }
            Instance = this;

            Log("Initialized");

        }

        private IEnumerator OnAudioManagerBeginApplyMusicCue(On.AudioManager.orig_BeginApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
        {
            Log($"[Audio] MusicCue = {musicCue}");
            MusicCue.MusicChannelInfo[] infos = ReflectionHelper.GetField<MusicCue, MusicCue.MusicChannelInfo[]>(musicCue, "channelInfos");
            foreach (MusicCue.MusicChannelInfo info in infos)
            {
                AudioClip origAudio = ReflectionHelper.GetField<MusicCue.MusicChannelInfo, AudioClip>(info, "clip");
                if (origAudio != null)
                {
                    Log($"[Audio] Orignal audio name = {origAudio.name}");
                    AudioClip possibleReplace = null;
                    if (audioCache.ContainsKey(origAudio.name))
                    {
                        Log($"[Audio] Cache hit for: {origAudio.name}");
                        possibleReplace = audioCache[origAudio.name];
                        //possibleReplace.UnloadAudioData();
                    }
                    else
                    {
                        Log($"[Audio] no cache, attempting to load from file");
                        possibleReplace = GetAudioClip(origAudio.name);
                        audioCache.Add(origAudio.name, possibleReplace);
                    }
                    if (possibleReplace != null)
                    {
                        // Change Audio Clip
                        ReflectionHelper.SetField<MusicCue.MusicChannelInfo, AudioClip>(info, "clip", possibleReplace);
                        ReflectionHelper.SetField<MusicCue, MusicCue.MusicChannelInfo[]>(musicCue, "channelInfos", infos);
                    }
                }
            }
            return orig(self, musicCue, delayTime, transitionTime, applySnapshot);
        }

        private AudioClip GetAudioClip(string origName)
        {
            var filefinder = audiolocation + origName + ".wav";

            Stream STREAM = assembly.GetManifestResourceStream(filefinder);
            if (STREAM != null)
            {
                Log($"[Audio] Loading file: {origName}");
                WavData.Inspect(STREAM, null);
                WavData wavData = new WavData();
                wavData.Parse(STREAM, null);
                STREAM.Close();
                float[] wavSoundData = wavData.GetSamples();
                AudioClip audioClip = AudioClip.Create(origName, wavSoundData.Length / wavData.FormatChunk.NumChannels, wavData.FormatChunk.NumChannels, (int)wavData.FormatChunk.SampleRate, false);
                audioClip.SetData(wavSoundData, 0);
                return audioClip;
            }

            Log($"[Audio] Using original for: {origName}");
            return null;
        }
    }
}