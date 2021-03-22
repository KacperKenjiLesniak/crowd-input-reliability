using System.Linq;
using DefaultNamespace;
using NUnit.Framework;
using UnityEngine;

public class CrowdInputTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void CrowdInputReliabilitySimplePasses()
    {
        ICrowdInputReliability crowdInputReliability = new CrowdInputReliability(3, 3, 0.1f, 0.5f);

        int[] commands = {0, 0, 1};
        int[] commands2 = {1, 0, 1};

        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[0], 1);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[1], 1);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[2], 1);

        Assert.AreEqual(crowdInputReliability.IssueCommands(commands), 0);
        Assert.AreEqual(crowdInputReliability.IssueCommands(commands2), 1);
    }

    [Test]
    public void ShouldUpdateReliabilities()
    {
        ICrowdInputReliability crowdInputReliability = new CrowdInputReliability(10, 3, 0.1f, 0.5f);

        int[] commands = { 0, 0, 0, 0, 0, 1, 1, 1, 2, 2 };

        Assert.AreEqual(crowdInputReliability.IssueCommands(commands), 0);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[0], 1.073f, 0.001f);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[5], 1.044f, 0.001f);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[8], 0.75f, 0.001f);

        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities().Sum(), 10, 0.001f);
    }

    [Test]
    public void ShouldNotUpdateReliabilities()
    {
        ICrowdInputReliability crowdInputReliability = new CrowdInputReliability(10, 3, 0.1f, 0.3f);

        int[] commands = { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };

        Assert.AreEqual(crowdInputReliability.IssueCommands(commands), 0);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[0], 1.0f);
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[6], 1.0f);
    } 
    
    
    [Test]
    public void ReliabilitiesShouldBeLowerBounded()
    {
        ICrowdInputReliability crowdInputReliability = new CrowdInputReliability(10, 3, 0.05f, 0.5f);

        int[] commands = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };

        for (var i = 0; i < 100; i++)
        {
            crowdInputReliability.IssueCommands(commands);
            Debug.Log(crowdInputReliability.GetPlayerReliabilities()[9]);
        }
        
        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities()[9], -1f, 0.001f);

        Assert.AreEqual(crowdInputReliability.GetPlayerReliabilities().Sum(), 10, 0.001f);
    }
}