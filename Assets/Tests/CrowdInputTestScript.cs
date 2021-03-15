using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class CrowdInputTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void CrowdInputReliabilitySimplePasses()
    {
        var crowdInputReliability = new CrowdInputReliability(3, 3, 0.1f, 0.5f);

        int[] commands = {0, 0, 1};
        int[] commands2 = {1, 0, 1};

        Assert.AreEqual(crowdInputReliability.playerReliabilities[0], 1);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[1], 1);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[2], 1);

        var reliabilities = crowdInputReliability.playerReliabilities;
        Assert.AreEqual(reliabilities[0], 1);

        Assert.AreEqual(crowdInputReliability.IssueCommands(commands), 0);
        Assert.AreEqual(crowdInputReliability.IssueCommands(commands2), 1);
    }

    [Test]
    public void ShouldUpdateReliabilities()
    {
        var crowdInputReliability = new CrowdInputReliability(10, 3, 0.1f, 0.5f);

        int[] commands = { 0, 0, 0, 0, 0, 1, 1, 1, 2, 2 };

        Assert.AreEqual(crowdInputReliability.IssueCommands(commands), 0);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[0], 1.05f);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[5], 1.03f);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[8], 0.83f);

        var reliabilities = crowdInputReliability.playerReliabilities;
        Assert.AreEqual(reliabilities[0], 1.05f);
    }

    [Test]
    public void ShouldNotUpdateReliabilities()
    {
        var crowdInputReliability = new CrowdInputReliability(10, 3, 0.1f, 0.3f);

        int[] commands = { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };

        Assert.AreEqual(crowdInputReliability.IssueCommands(commands), 0);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[0], 1.0f);
        Assert.AreEqual(crowdInputReliability.playerReliabilities[6], 1.0f);

        var reliabilities = crowdInputReliability.playerReliabilities;
        Assert.AreEqual(reliabilities[0], 1.0f);
    } 
}