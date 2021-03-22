using System.Collections.Generic;

namespace Reliability
{
    public interface ICrowdInputReliability
    {
        public int IssueCommands(int[] commands);
        public List<float> GetPlayerReliabilities();
    }
}