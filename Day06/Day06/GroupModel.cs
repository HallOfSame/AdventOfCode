using System.Collections.Generic;
using System.Linq;

namespace Day06
{
    public class GroupModel
    {
        #region Constructors

        public GroupModel(IEnumerable<char> questionsAnsweredYes)
        {
            QuestionsAnsweredYes = new string(questionsAnsweredYes.OrderBy(x => x)
                                                                  .ToArray());
        }

        #endregion

        #region Instance Properties

        public int NumberOfQuestionsAnsweredYes
        {
            get
            {
                return QuestionsAnsweredYes.Length;
            }
        }

        public string QuestionsAnsweredYes { get; }

        #endregion
    }
}