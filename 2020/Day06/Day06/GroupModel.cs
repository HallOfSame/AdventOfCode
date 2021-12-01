using System.Collections.Generic;
using System.Linq;

namespace Day06
{
    public class GroupModel
    {
        #region Fields

        private readonly List<IEnumerable<char>> linesOfGroup;

        #endregion

        #region Constructors

        public GroupModel(IEnumerable<IEnumerable<char>> groupLines)
        {
            linesOfGroup = groupLines.ToList();
        }

        #endregion

        #region Instance Properties

        public int NumberOfQuestionsAnsweredYesByAll
        {
            get
            {
                return QuestionsAnsweredYesByAll.Length;
            }
        }

        public int NumberOfQuestionsAnsweredYesByAny
        {
            get
            {
                return QuestionsAnsweredYesByAny.Length;
            }
        }

        public string QuestionsAnsweredYesByAll
        {
            get
            {
                var firstResponse = linesOfGroup.First();

                var hash = new HashSet<char>(firstResponse);

                linesOfGroup.Skip(1)
                            .ToList()
                            .ForEach(x => hash.IntersectWith(x));

                return new string(hash.OrderBy(x => x)
                                      .ToArray());
            }
        }

        public string QuestionsAnsweredYesByAny
        {
            get
            {
                var hash = new HashSet<char>();

                linesOfGroup.ForEach(x => hash.UnionWith(x));

                return new string(hash.OrderBy(x => x)
                                      .ToArray());
            }
        }

        #endregion
    }
}