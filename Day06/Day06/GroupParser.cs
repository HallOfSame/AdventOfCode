using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day06
{
    public class GroupParser
    {
        #region Instance Methods

        public async Task<List<GroupModel>> GetGroupModels(StreamReader streamReader)
        {
            var models = new List<GroupModel>();

            var questionsAnsweredYes = new HashSet<char>(26);

            void AddModelToList()
            {
                var model = new GroupModel(questionsAnsweredYes);
                models.Add(model);
                questionsAnsweredYes.Clear();
            }

            while (!streamReader.EndOfStream)
            {
                var nextLine = await streamReader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(nextLine))
                {
                    AddModelToList();
                    continue;
                }

                var questionsInLine = nextLine.ToCharArray()
                                              .ToHashSet();

                questionsAnsweredYes.UnionWith(questionsInLine);
            }

            // Add the final model. Our file reading seems to ignore the final blank line in the file
            AddModelToList();

            return models;
        }

        #endregion
    }
}