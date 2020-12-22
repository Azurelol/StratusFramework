using UnityEngine;
using System;
using System.Collections;

namespace Stratus
{
	public enum StratusImageEncoding
	{
		PNG,
		JPG,
	}

	public static partial class Extensions
	{
		/// <summary>
		/// Checks if the specified renderer is being seen by a specific Camera. 
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}



		public static IEnumerator GetToTexture2DRoutine(this RenderTexture renderTexture, Action<Texture2D> callback, TextureFormat format = TextureFormat.RGB24)
		{
			yield return new WaitForEndOfFrame();
			Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, format, false);
			texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			texture.Apply();
			callback(texture);
		}

		public static string ToExtension(this StratusImageEncoding encoding)
		{
			switch (encoding)
			{
				case StratusImageEncoding.PNG:
					return ".png";
				case StratusImageEncoding.JPG:
					return ".jpg";
				default:
					break;
			}
			throw new NotSupportedException($"Unsupported image encoding {encoding}");
		}
	}
}
