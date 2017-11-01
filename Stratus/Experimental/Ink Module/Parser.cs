using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// Base class for all parsers used by the StoryReader.
      /// Any derived parsers need to define how a line is parsed
      /// </summary>
      public abstract class Parser
      {
        /// <summary>
        /// Parses a line of ink dialog, using whatever parses have been set
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public abstract ParsedLine Parse(string line, List<string> tags);

      }

    }
  }
}
