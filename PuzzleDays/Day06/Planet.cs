using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day06
{
    [DebuggerDisplay("{Name} {DirectOrbits.Count}")]
    public class Planet
    {
        #region Constructors

        public Planet(string name,
                      Planet directParent)
        {
            Name = name;
            DirectParent = directParent;
            DirectOrbits = new List<Planet>();
        }

        #endregion

        #region Instance Properties

        public List<Planet> DirectOrbits { get; }

        public Planet DirectParent { get; set; }

        public List<Planet> IndirectOrbits
        {
            get
            {
                return DirectOrbits.SelectMany(d => d.DirectOrbits.Concat(d.IndirectOrbits))
                                   .ToList();
            }
        }

        public string Name { get; }

        #endregion
    }
}