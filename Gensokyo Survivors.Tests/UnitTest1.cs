using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using System;

namespace Gensokyo_Survivors.Tests
{
    public class Tests
    {
        public static uint GetLevelUpExperienceRequirement(uint level)
        {
            return 20 + (level * 10);
        }

        private static void CalculateLevelUpInfo(uint exp, uint lev, out uint newLevel, out uint expRemainder)
        {
            var nextLevelRequirement = GetLevelUpExperienceRequirement(lev + 1);
            while (nextLevelRequirement <= exp)
            {
                lev += 1;
                exp -= nextLevelRequirement;
            } 

            newLevel = lev;
            expRemainder = exp;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void LevelUp_ExactThreshold_ShouldLevelUpOnce()
        {
            uint currentLevel = 1;
            uint exp = GetLevelUpExperienceRequirement(currentLevel + 1); // Exact exp for level 2
            CalculateLevelUpInfo(exp, currentLevel, out uint newLevel, out uint expRemainder);

            newLevel.ShouldBe(currentLevel + 1);
            expRemainder.ShouldBe<uint>(0);
            // Assert.AreEqual(currentLevel + 1, newLevel);
            // Assert.AreEqual(0, expRemainder);
        }

        [Test]
        public void LevelUp_MoreThanOneLevel_ShouldSkipMultipleLevels()
        {
            uint currentLevel = 1;
            uint exp = 200; // Should level up beyond level 3
            CalculateLevelUpInfo(exp, currentLevel, out uint newLevel, out uint expRemainder);

            newLevel.ShouldBe(currentLevel + 5);
            expRemainder.ShouldBeLessThan(GetLevelUpExperienceRequirement(newLevel));
        }

        [Test]
        public void LevelUp_LowExp_ShouldNotLevelUp()
        {
            uint currentLevel = 1;
            uint exp = 10; // Not enough to level up
            CalculateLevelUpInfo(exp, currentLevel, out uint newLevel, out uint expRemainder);

            newLevel.ShouldBe(currentLevel);
            expRemainder.ShouldBe(10u);
        }

        [Test]
        public void LevelUp_HighExp_ShouldResultInCorrectRemainingExp()
        {
            uint currentLevel = 2;
            uint exp = 85; // Should go past level 3
            CalculateLevelUpInfo(exp, currentLevel, out uint newLevel, out uint expRemainder);

            uint requiredForNext = GetLevelUpExperienceRequirement(newLevel);
            expRemainder.ShouldBeLessThan(requiredForNext);
            expRemainder.ShouldBe(exp - requiredForNext );
        }
    }
}
