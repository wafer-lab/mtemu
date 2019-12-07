using System.Collections.Generic;

namespace mtemu
{
    class EasterEgg
    {
        private class Egg
        {
            private byte[] data_;
            private bool found_;

            public Egg(byte[] data)
            {
                data_ = data;
                found_ = false;
            }

            public void SetFound()
            {
                found_ = true;
            }

            public bool IsFound()
            {
                return found_;
            }

            public byte[] GetData()
            {
                return data_;
            }
        }

        private static int founded_ = 0;
        private static bool notified_ = false;

        private static int GetEggNumber_(int led3, int led2, int led1, int led0)
        {
            return led3 << 6 | led2 << 4 | led1 << 2 | led0;
        }

        // Max sum of led clicks = 4
        private static Dictionary<int, Egg> easterEggs_ = new Dictionary<int, Egg> {
            { GetEggNumber_(0, 3, 2, 1), new Egg(Properties.Resources.egg_01_3333) },
            { GetEggNumber_(3, 3, 3, 3), new Egg(Properties.Resources.egg_02_3130) },
            { GetEggNumber_(3, 1, 3, 0), new Egg(Properties.Resources.egg_03_1213) },
            { GetEggNumber_(1, 2, 1, 3), new Egg(Properties.Resources.egg_04_2301) },
            { GetEggNumber_(2, 3, 0, 1), new Egg(Properties.Resources.egg_05_1323) },
            { GetEggNumber_(1, 3, 2, 3), new Egg(Properties.Resources.egg_06_1200) },
            { GetEggNumber_(1, 2, 0, 0), new Egg(Properties.Resources.egg_07_1330) },
            { GetEggNumber_(1, 3, 3, 0), new Egg(Properties.Resources.egg_08_3023) },
            { GetEggNumber_(3, 0, 2, 3), new Egg(Properties.Resources.egg_09_2213) },
            { GetEggNumber_(2, 2, 1, 3), new Egg(Properties.Resources.egg_10_3121) },
            { GetEggNumber_(3, 1, 2, 1), new Egg(Properties.Resources.egg_11_2031) },
            { GetEggNumber_(2, 0, 3, 1), new Egg(Properties.Resources.egg_12_3110) },
            { GetEggNumber_(3, 1, 1, 0), new Egg(Properties.Resources.egg_13_2311) },
            { GetEggNumber_(2, 3, 1, 1), new Egg(Properties.Resources.egg_14_1210) },
            { GetEggNumber_(1, 2, 1, 0), new Egg(Properties.Resources.egg_15_final) },
        };

        public static byte[] GetData(int number)
        {
            if (easterEggs_.ContainsKey(number)) {
                if (!easterEggs_[number].IsFound()) {
                    easterEggs_[number].SetFound();
                    ++founded_;
                }
                return easterEggs_[number].GetData();
            }
            return null;
        }

        public static int FoundEggsCount()
        {
            return founded_;
        }

        public static int EggsCount()
        {
            return easterEggs_.Count;
        }

        public static void SetNotified()
        {
            notified_ = true;
        }

        public static bool IsNotified()
        {
            return notified_;
        }
    }
}
