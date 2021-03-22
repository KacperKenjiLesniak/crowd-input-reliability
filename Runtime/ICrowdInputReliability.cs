using System.Collections.Generic;

namespace DefaultNamespace
{
    public interface ICrowdInputReliability
    {
        public int IssueCommands(int[] commands);
        public List<float> GetPlayerReliabilities();
    }
}