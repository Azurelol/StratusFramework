using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Stratus
{
  /// <summary>
  /// Plays a video on the specified RawImage using the VideoPlayer
  /// </summary>
  public class VideoEvent : Triggerable
  {
    [Header("Output")]
    [Tooltip("Where the image will be displayed as a raw texture")]
    public RawImage target;

    [Header("Video")]
    public VideoClip clip;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    [Tooltip("Number of bits in the depth buffer")]
    public int depth = 16;
    public RenderTextureFormat format = RenderTextureFormat.ARGB32;

    protected override void OnAwake()
    {
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      PrepareVideo();
      PlayVideo();
    }

    private void PrepareVideo()
    {
      if (logging)
        Trace.Script($"Now playing {clip.name}", this);

      // Set the clip
      videoPlayer.clip = clip;

      // Read info from the video file
      int width = (int)videoPlayer.clip.width;
      int height = (int)videoPlayer.clip.height;

      // Make the texture
      RenderTexture renderTexture = new RenderTexture(width, height, depth, format);
      renderTexture.Create();

      // Input the texture
      videoPlayer.targetTexture = renderTexture;

      // Output the texture
      target.texture = renderTexture;

      videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    private void PlayVideo()
    {
      // Now play
      videoPlayer.Play();

    }

  } 
}
