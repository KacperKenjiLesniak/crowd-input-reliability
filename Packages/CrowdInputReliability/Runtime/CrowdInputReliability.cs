using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrowdInputReliability
{
    public List<float> playerReliabilities = new List<float>();
    public int numberOfPlayers { get; }
    
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

        var maxNumberOfCommands = commandFrequencies.Max();

        for (var i = 0; i < numberOfPlayers; i++)
        {
            playerCommandViabilities.Add(
                commandFrequencies[commands[i]] >= agreementThreshold * maxNumberOfCommands);
        }

        if (playerCommandViabilities.All(x => x))
        {
            return reliabilityChanges;
        }

        var nonViableCommandsNormalization = 0.0f;

        for (var i = 0; i < numberOfPlayers; i++)
        {
            if (playerCommandViabilities[i])
            {
                reliabilityChanges[i] = (float) commandFrequencies[commands[i]] / numberOfPlayers *
                                        reliabilityCoefficient;
            }
            else
            {
                nonViableCommandsNormalization += (float) maxNumberOfCommands / commandFrequencies[commands[i]];
            }
        }

        var sumOfViableReliabilityChanges = reliabilityChanges.Sum();

        var nonViableReliabilityChangeCoefficient =
            -maxNumberOfCommands / nonViableCommandsNormalization * sumOfViableReliabilityChanges;

        for (var i = 0; i < numberOfPlayers; i++)
        {
            if (!playerCommandViabilities[i])
            {
                reliabilityChanges[i] = nonViableReliabilityChangeCoefficient / commandFrequencies[commands[i]];
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

        return commandFrequencies.Select((n, i) => (Number: n, Index: i)).Max().Index;
    }
}
