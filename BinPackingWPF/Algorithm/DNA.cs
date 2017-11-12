using BinPackingWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingWPF.Algorithm
{
    public class DNA
    {
        public Fleet Fleet { get; private set; }
        public IList<Package> Packages { get; private set; }
        public float Fitness { get; private set; }

        private Random random;
        private Func<IList<Package>, Fleet> getRandomFleet;
        private Func<Fleet, float> fitnessFunction;

        public DNA(IList<Package> packages, Random random, Func<IList<Package>, Fleet> getRandomFleet, Func<Fleet, float> fitnessFunction, Fleet fleet = null, bool shouldInitGenes = true)
        {
            Fleet = fleet;
            Packages = packages;
            this.random = random;
            this.getRandomFleet = getRandomFleet;
            this.fitnessFunction = fitnessFunction;

            if (shouldInitGenes)
            {
                Fleet = getRandomFleet(Packages);
            }
        }

        public float CalculateFitness(Fleet fleet)
        {
            Fitness = fitnessFunction(fleet);
            return Fitness;
        }

        public DNA Crossover(DNA otherParent)
        {
            DNA child = new DNA(Packages, random, getRandomFleet, fitnessFunction, Fleet, shouldInitGenes: false);

            //for (int i = 0; i < Fleets.Length; i++)
            //{
            //    child.Fleets[i] = random.NextDouble() < 0.5 ? Fleets[i] : otherParent.Fleets[i];
            //}

            return child;
        }

        public void Mutate(float mutationRate)
        {
            //for (int i = 0; i < Fleets.Length; i++)
            //{
            //    if (random.NextDouble() < mutationRate)
            //    {
            //        Fleets[i] = getRandomFleet(Packages);
            //    }
            //}
        }
    }
}
