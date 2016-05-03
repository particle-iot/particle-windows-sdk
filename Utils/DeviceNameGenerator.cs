using System;
using System.Collections.Generic;

namespace Particle.SDK.Utils
{
    /// <summary>
    /// Simple class to generate unique names for Particle devices
    /// </summary>
    public class DeviceNameGenerator
    {
        private static Random random = new Random();

        private static String[] trochees = new String[]{"aardvark", "bacon", "badger", "banjo",
            "bobcat", "boomer", "captain", "chicken", "cowboy", "maker", "splendid", "useful",
            "dentist", "doctor", "dozen", "easter", "ferret", "gerbil", "hacker", "hamster",
            "sparkling", "hobbit", "hoosier", "hunter", "jester", "jetpack", "kitty", "laser", "lawyer",
            "mighty", "monkey", "morphing", "mutant", "narwhal", "ninja", "normal", "penguin",
            "pirate", "pizza", "plumber", "power", "puppy", "ranger", "raptor", "robot", "scraper",
            "spark", "station", "tasty", "trochee", "turkey", "turtle", "vampire", "wombat",
            "zombie"};

        public static string GenerateUniqueName(HashSet<string> existingNames)
        {
            string uniqueName = null;
            while (uniqueName == null)
            {
                string part1 = GetRandomName();
                string part2 = GetRandomName();
                string candidate = part1 + "_" + part2;
                if (!existingNames.Contains(candidate) && !part1.Equals(part2))
                    uniqueName = candidate;
            }
            return uniqueName;
        }

        private static String GetRandomName()
        {
            int randomIndex = random.Next(0, trochees.Length);
            return trochees[randomIndex];
        }
    }
}
