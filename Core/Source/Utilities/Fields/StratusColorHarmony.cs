using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Provides color combinations for use in UI designs
	/// </summary>
	public sealed class StratusColorHarmony
	{
		/// <summary>
		/// Complementary color schemes use colors that are opposite each other on the color wheel.
		/// Provides a high contrast and high impact color combination. (e.g: red and green)
		/// </summary>
		public class Complementary
		{
			public Complementary(Color first, Color second)
			{
				this.first = first;
				this.second = second;
			}

			public Color first { get; private set; }
			public Color second { get; private set; }
		}

		/// <summary>
		/// Analogous color schemes use colors that are next to each other on the color wheel. 
		/// Uses one dominant color, a second to support and an accent.
		/// as accents.
		/// </summary>
		public class Analogous
		{
			public Analogous(Color dominant, Color support, Color accent)
			{
				this.dominant = dominant;
				this.support = support;
				this.accent = accent;
			}

			public Color dominant { get; private set; }
			public Color support { get; private set; }
			public Color accent { get; private set; }
		}

		/// <summary>
		/// Triadic coolor schemes use 3 colors that are evenly spaced around the color wheel.
		/// This combination creates bold, vibrant color palettes.
		/// 
		/// </summary>
		public class Triad
		{
			public Triad(Color first, Color second, Color third)
			{
				this.first = first;
				this.second = second;
				this.third = third;
			}

			public Color first { get; private set; }
			public Color second { get; private set; }
			public Color third { get; private set; }
		}

		/// <summary>
		/// Tetadric coolor schemes use 4 colors that are evenly spaced around the color wheel.
		/// This combination is bold and works best if you let one color be dominant and use the others
		/// as accents.
		/// </summary>
		public class Tetradic
		{
			public Tetradic(Color first, Color second, Color third, Color fourth)
			{
				this.first = first;
				this.second = second;
				this.third = third;
				this.fourth = fourth;
			}

			public Color first { get; private set; }
			public Color second { get; private set; }
			public Color third { get; private set; }
			public Color fourth { get; private set; }
		}
	}

}