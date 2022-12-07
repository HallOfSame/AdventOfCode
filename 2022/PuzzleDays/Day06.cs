using Helpers.Structure;

namespace PuzzleDays
{
    public class Day06 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            return FindMarker(4);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            return FindMarker(14);
        }

        private string FindMarker(int numberOfUniqueCharacters)
        {
            // Init our pointers
            var left = 0;
            var right = 0;

            // Hash set to track what characters we've used
            var usedChar = new HashSet<char>();

            // Loop through the message
            while (right < message.Length)
            {
                // Process the next char
                var nextChar = message[right];

                // If it is a duplicate
                if (usedChar.Contains(nextChar))
                {
                    // Shift left until it is not
                    while (usedChar.Contains(nextChar))
                    {
                        var leftChar = message[left];

                        usedChar.Remove(leftChar);

                        left++;
                    }
                }

                // Add current char to the tracker
                usedChar.Add(nextChar);

                // If the count is right we've found a solution
                if (usedChar.Count == numberOfUniqueCharacters)
                {
                    // Return right + 1 because we're 0 based indexing but the answer is how many characters or something
                    return (right + 1).ToString();
                }

                right++;
            }

            throw new Exception("Did not find a solution");
        }


        public override async Task ReadInput()
        {
            message = "zdnnfgfsgffgllwrwprwrgwwpssznzrnznllstszsttpdptdpdmdsdzsdscsmcmttdllbsbwwtwnwswcchshlhjhfhwfftcchnnfwwbqwqwrqqgmgzmmwzwfzwfzzzsmzzrczcmmhphzhbbbgdbgddmggwwbbttvmtvttfsfttjlttdfdsdqqczqzffbrfbbfbrrmdrrlslshllwzwrzrzzlqldqdjdjwjvjzjrjjcsszjjqfqnfqqsrqrccbhhwphwhbwwlzwwjwfjwfwzfwzffssvjsjddcsdslslrsrfsrffsggdffrcrcdcpprrzbrzbbtstvvqttbqqgfgsggtvtrvtvbbrqrsqrsqsvsbbzmbmgmvgmmrqrzzbbnjjlwjjfssdrdbrbffwrrrjjgcgtgvvjbjjjsqjsqqncntnndcdrcrhhsgstslldwdbwdbdtbdbggpnndhdvhhvrrlzzfjjffzszvvzgzhhqzqttdhdrrwdwzdwdbwwfsfwsfwfqqzwzbzmzwmmvgggvssvwswfwswhwzzqtqrtthhbbjggjppnpfnfmnnghgrhhtvtqtsttbpprzzwqqfhqfflttzffrprwpwspplzpztptgtltjlttwwsrsrprwrsswnwttcscqsqlsqqhshbhlblnnpznnzlnndrnrcncvcqcjqqvhvppjzjddzbzsztzqqlmlnlnblnlwnllswszwzrzddqhdqhqffhfhjjpbbhvbvmmfhmhcccjlclhcllbrlblddnpplggcmmvddmmqzzmqmppnjpjjjzllqjqccrwwhzhnnlmlhhbbtztvtltvtnvtvltlrtrllbttbzzfwfrrjzjbbmgghjhqhlljnjhhhjzjcjvcvwwzczwcwgcclflnnsvvcncbncnqqpjqjnjwjrrgqrrmqmfmmmjgjfgjfftggqdgqddgcdcdwdrdsswqqphpjhhdjjwswfwfnfqnqccvhvzvmzvzzplzljzjpzzhmzmqzqjjprpvrrhqqnbngnnpvpfvpvqvwwrhhdndqdppmcppzddbzzjcjdjfjnfnngqnnchcqqpllpwppgllcblcblbddzhhqsqbssjqqgmgbgzzvhvnhvnhvvpmpvvlddgppzrprmpmbpbjpbpfptpspddcgddqhdhhthrhjrhrvrlrffvbfbvfvbblssftfnfwwrwnnzdznnbwnbwnwlwttszzmmlzlhhpjhhjvvlvwwdnddzwdwgdgdssflfvfzvznzbzrbzzdssphhgttllcjcvcjjdrdqdhdnndlndldcllcnllslvlnvllmglgnnplpmpzzjwjtwtnntrtjtvjtjffhcfclltppftfwwprrwsrwwzdwzdztzccbmccfcfzczbcbsbqsstjtrrpnnfqqfmmchmmwmrwrwzwztzddgzdgzzfwzzrppcscrrgvgvgvtgtsslrsrvsrrdcdscscwcwqqsccwjcjgjvgvpvnvhhchrrgprpvrvsvsttgghdghhmphmmbvbcbsccdbcbnccbnnsjnjhnnzbnnpjbwdpczcvgjpgwfqrmnvwncflvnttwhfgmfqvngpdhbhvlglfhtdqmqtqcgjcqghzvbdghdgvjcsjrlpqvgcdnbpqrcrcvqqdlcpscqbfpsnhzcdbbcssslrjlzsqpprsbmtqhzblvwbswprhztmpcgfqfsgshchrhjmwwhpzsjzrmrvgdgwjrlwpgqhbzrmnmnnsnvzsrlhthgvlpljsjrpbhbzctdqgvdjcmrgtvqjqbcwsprnfmntzpbjcdtlchhjgwpmldmsstbtztfdbgbstgnlwbzrrzmvbrhnrlcwfgwwbfnntbjspqwngbjrvhdcnblqssgjlbcwbbgphhnmfcmdhqdhsnmvdjnwwwjlffswhsmwqrsprftjwtbtcvmpctgvfqvvcjpnwzqldglfbwfzpnqmdlrdpjmjptvwsctlmhmzzgvplglfgsvrfbqbmrhplczbvqpdjjhhvfqswhzhqfgzstwwpbtbsnnlgpshwqgppzbpsfpfvcntbbbzwdnfcgcwzbqwmhjrhpdfvpbzpmfnmllrcqlqhcbzfltzcgccwwqmtsmwchhvbqtdrnsbrchqqcmtfqpddcjplbvdhhtndrrmfdtmbpdvwthvgdccnrcqmpznlvzqzfjqmpvgjtfbtfjnrmlzhwhljrrqnbqzpfhcvncblfggrtbdfjqnlgpbrzmwcvrvjtjscfmcnfjgqzqsphldvhdbpvmghrvsdmvpmvvdmdhwdghtjltmlcmfhvrsvcvpblwhhfcfdqnrsjbcldgbwhtnjntmgvprhbjrcvsmhgtfphcwncpjtngqhvwrmgprstbtdstmttpzcntmzvncwslqlldpnjbtpmsfnwbpwpnlfgdvcqplvlqqjvfftnnvpcmwjrvwqhlrshftrbhcwnczzsnvtnjnrbzzgzfsqhnfwlcgzvvhqcgvqtmcpnhlvdlmwgsvtwbqgrdsrrddszvscbgtlpwpzjrbvwhjnrpprhtzmthbpfzvplzwfdtnwqwtctgjslmcczjvwplsqwgfnfbgdjbsdpwbgflttvvqlhzgmmpjsnwbqqtcdszfqbhgnmbbmrbrgnrzdmzwnjjzjqcwqcqfchjrzlspgbrchcbgwbhvggsqbvdpzbpnwdtqvcjwcwnbjdhsdfmbtwfbfhzwwtnqzhmtvtbfwrsqjzgssvlwszvlmvbslpncnhmsdhcqqfpftztpzbbhsgbnscddbjlgwgjjndgwbrhwmsfdmmsnlwgwdsdltwjfvwnczjrbgcvsfczppltdptlgcdfzgmqpjngstldqgmwhdmfrwwfqwdgswvfdrtsgtvttpcbnhzbscnchpvfjvbcszbwchnbmfrvsswslbzlhgwlvfchdbfthbpdbwwqtmlgwjqtjhzrjzzmrpdwnvfgrnqdcqmwtttmwjvgcmjsddvtlswldzhtppwvhmlghwlgblfttctnglwhtfvqgjmdjcnflsrjvpjwcjfftbdmmcbqvfwnnnzsltllncbstgnhtmpsltgztqzjbbrtqpcvdlnhpnhvmmztpfpplbqjlpqvfsdvhwvstdmqbtnpzrcbdhvdtghqwcppcfzjfjsfwvqrtfgcdzwgzjvrqqsjtnhlbjcmtjcnmtpffwcwhqqphwjsrhpqvnnhhrcnvztfdjzbjggwlgjprbpssgnmtcrvprwbsrfvvphrsgzgbrfnpgtqbbprhfphqntsglrmhzfnwqptlslnhtrhfprjpdcglcffsblnjwczmgwhmmtgsgwljmqlvdglqmzwmtqcvgcrmqjldlsnbssdvrrtltngvrsqbctqlsngqvcphjvhmwsssgwmvgzdctjcmjtpcjhvfcrfhbffdqfjjvpqwgvnlzhgfnfmlrrfvjrdvhzdcvdvmpncvtjbbnczpzmglfqnpbsrsjwgvszsnqvrnvlhmqjjnmsfngbdlpwbqllcptjtlbhrfdvhlrpdlznpvndjzjdtjflqqjdgjjpmnpmjgcglllgcqbfpvdtpbjdnvrclmnlfdrpbmwzgvdhgbzvbhwqfslhshbfcbwrnsjndgjgccllfbzgmcjqcmdnfftnccphqtwmgqgfqlvlwsrprctchqrscwvgpdrwgcfgzjwmzmmsmwzgtzsjtqfggcczcmghlqgnqqjvsrsfrrmwmnrnhbsszmwsqlrggsbdwzzfnhwcggjszfrlffplvcblvphqmzjnzwzdshhdprfrdbcrmbtztcfvgpzpmmgflswphvnvtwhbbhjwffsvqfjlfvzqmhmsmddwdwsqfnnplbqnptbvgjqgmflsbfdtpvdgbfnqmcqznhpqbpwtbfpqllvqwvcftdjjtlsvzbssbtcdzqqqvzlqhfpdthscqmvhpndmnztthvvzccqswswspnqcbncvszrgjshjhdsclrjdnjdczqmcjldbspclgrmwqdvcvpcsvjggfdqlrwlnzptfvcwjsgblpjzgcrrmjqptvdnwr";
        }

        private string message;
    }
}
