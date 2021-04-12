using System.Collections.Generic;
using System.Linq;

namespace Reliability
{
    public class CrowdInputReliability : ICrowdInputReliability
    {
        private const float RELIABILITY_LOWER_BOUND = -0.5f;

        private List<float> playerReliabilities = new List<float>();
        private int numberOfPlayers { get; }

        private int numberOfCommands;
        private float reliabilityCoefficient;
        private float agreementThreshold;

        public CrowdInputReliability(int numberOfPlayers, int numberOfCommands, float reliabilityCoefficient,
            float agreementThreshold)
        {
            this.numberOfPlayers = numberOfPlayers;
            this.numberOfCommands = numberOfCommands;
            this.reliabilityCoefficient = reliabilityCoefficient;
            this.agreementThreshold = agreementThreshold;
            for (var i = 0; i < numberOfPlayers; i++)
            {
                playerReliabilities.Add(1.0f);
            }
        }

        private List<float> CalculateReliabilityUpdates(int[] commands, List<int> commandFrequencies)
        {
            var playerCommandViabilities = new List<bool>();

            var reliabilityChanges = new List<float>(Enumerable.Repeat(0f, numberOfPlayers));

            int maxNumberOfCommands = commandFrequencies.Max();

            for (var i = 0; i < numberOfPlayers; i++)
            {
                playerCommandViabilities.Add(
                    commandFrequencies[commands[i]] >= agreementThreshold * maxNumberOfCommands);
            }

            if (playerCommandViabilities.All(x => x))
            {
                return reliabilityChanges;
            }

            var viableCommandsNormalization = 0.0f;

            for (var i = 0; i < numberOfPlayers; i++)
            {
                if (!playerCommandViabilities[i])
                {
                    reliabilityChanges[i] = -(float) maxNumberOfCommands / commandFrequencies[commands[i]] *
                                            reliabilityCoefficient;

                    if (playerReliabilities[i] + reliabilityChanges[i] <= RELIABILITY_LOWER_BOUND)
                    {
                        reliabilityChanges[i] = -playerReliabilities[i] + RELIABILITY_LOWER_BOUND;
                    }
                }
                else
                {
                    viableCommandsNormalization += (float) commandFrequencies[commands[i]] / numberOfPlayers;
                }
            }

            float sumOfNonViableReliabilityChanges = reliabilityChanges.Sum();

            float viableReliabilityChangeCoefficient =
                -1 / viableCommandsNormalization / numberOfPlayers * sumOfNonViableReliabilityChanges;

            for (var i = 0; i < numberOfPlayers; i++)
            {
                if (playerCommandViabilities[i])
                {
                    reliabilityChanges[i] = viableReliabilityChangeCoefficient * commandFrequencies[commands[i]];
                }
            }

            return new List<float>(reliabilityChanges);
        }

        public int IssueCommands(int[] commands)
        {
            var commandFrequencies = new List<int>(Enumerable.Repeat(0, numberOfCommands));

            for (var i = 0; i < numberOfPlayers; i++)
            {
                commandFrequencies[commands[i]] += 1;
            }

            var reliabilityUpdates = CalculateReliabilityUpdates(commands, commandFrequencies);

            for (var i = 0; i < numberOfPlayers; i++)
            {
                playerReliabilities[i] += reliabilityUpdates[i];
            }

            var weightedCommandFrequencies = new List<float>(Enumerable.Repeat(0f, numberOfCommands));
            for (var i = 0; i < numberOfPlayers; i++)
            {
                weightedCommandFrequencies[commands[i]] += playerReliabilities[i];
            }

            return weightedCommandFrequencies.Select((n, i) => (Number: n, Index: i)).Max().Index;
        }

        public List<float> GetPlayerReliabilities()
        {
            return playerReliabilities;
        }
    }
}