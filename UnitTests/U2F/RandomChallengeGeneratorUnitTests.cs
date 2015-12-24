using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Crypto;

namespace UnitTests
{
    [TestClass]
    public class RandomChallengeGeneratorUnitTests
    {
        [TestMethod]
        public void RandomChallengeGenerator_RandomChanlleges()
        {
            RandomChallengeGenerator randomChallengeGenerator = new RandomChallengeGenerator();
            byte[] challenge1 = randomChallengeGenerator.GenerateChallenge();
            byte[] challenge2 = randomChallengeGenerator.GenerateChallenge();

            Assert.AreEqual(challenge1.Length, challenge2.Length);
            Assert.IsFalse(challenge1.SequenceEqual(challenge2));
        }
    }
}