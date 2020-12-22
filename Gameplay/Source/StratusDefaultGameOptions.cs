using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
    /// <summary>
    /// Base class for the data structure that holds game options
    /// </summary>
    public abstract class StratusGameOptions 
    {
        public abstract class Category
        {
            public abstract void Apply();
        }

        public abstract Category[] options { get; }

        /// <summary>
        /// Applies all (registered) options
        /// </summary>
        public void Apply()
        {
            foreach (var option in options)
            {
                option.Apply();
            }
        }

    }

    [Serializable]
    public class StratusGameOptions<VideoOptions, AudioOptions> : StratusGameOptions
        where VideoOptions : StratusVideoOptions, new()
        where AudioOptions : StratusAudioOptions, new()
    {
        public VideoOptions video = new VideoOptions();
        public AudioOptions audio = new AudioOptions();

        public override Category[] options
        {
            get
            {
                return new Category[]
                {
                    video,
                    audio
                };
            }
        }
    }

    /// <summary>
    /// Default game options
    /// </summary>
    [Serializable]
    public class StratusDefaultGameOptions : StratusGameOptions<StratusVideoOptions, StratusAudioOptions>
    {
    } 


    public enum StratusFrameratePreset
    {
        /// <summary>
        /// 30
        /// </summary>
        Low,
        /// <summary>
        /// 60
        /// </summary>
        Medium,
        /// <summary>
        /// 144
        /// </summary>
        High,
        /// <summary>
        /// Uncapped framerate
        /// </summary>
        Uncapped,
    }

    public enum StratusResolutionPreset
    {
        /// <summary>
        /// 1280 x 720p
        /// </summary>
        LowDefinition,        
        /// <summary>
        /// 1920 x 1080p
        /// </summary>
        HighDefinition,
        /// <summary>
        /// 2560 x 1440p
        /// </summary>
        WideQuadHighDefinition,
    }

    [Serializable]
    public class StratusVideoOptions : StratusGameOptions.Category
    {
        public StratusResolutionPreset resolution;
        public bool fullscreen = true;
        public StratusFrameratePreset framerate;
        public bool vsync = true;
        [Range(0f, 1f)]
        public float gammaCorrection = 0.5f;
        [Range(0f, 1f)]
        public float contrast = 0.5f;
        [Range(0f, 1f)]
        public float brightness = 0.5f;

        public override void Apply()
        {
            Vector2 res = resolution.ToPixels();
            Screen.SetResolution((int)res.x, (int)res.y, fullscreen);
            Application.targetFrameRate = framerate.ToFramerate();
        }
    }

    [Serializable]
    public class StratusAudioOptions: StratusGameOptions.Category
    {
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float bgmVolume = 1f;
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        public bool subtitles = true;
        public bool soundPlaysInBackground = false;

        public override void Apply()
        {
        }
    }

    [Serializable]
    public class StratusGameplayOptions : StratusGameOptions.Category
    {
        public bool tips = true;

        public override void Apply()
        {
        }
    }

    public static class StratusGameOptiosnExtensions
    {
        public static Vector2Int ToPixels(this StratusResolutionPreset resolutionPreset)
        {
            switch (resolutionPreset)
            {
                case StratusResolutionPreset.LowDefinition:
                    return new Vector2Int(1280, 720);
                case StratusResolutionPreset.HighDefinition:
                    return new Vector2Int(1920, 1080);
                case StratusResolutionPreset.WideQuadHighDefinition:
                    return new Vector2Int(2560, 1440);
            }
            throw new NotImplementedException($"No resolution defined for preset {resolutionPreset}");
        }

        public static int ToFramerate(this StratusFrameratePreset frameratePreset)
        {
            switch (frameratePreset)
            {
                case StratusFrameratePreset.Low:
                    return 30;
                case StratusFrameratePreset.Medium:
                    return 60;
                case StratusFrameratePreset.High:
                    return 144;
            }
            throw new NotImplementedException($"No framerate defined for preset {frameratePreset}");
        }
    }

}